using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cyens.ReInherit;
using Pathfinding;
using UnityEngine;
using TheKiwiCoder;

public class FindNextCheckpoint : ActionNode
{
    private EntranceController m_entranceController;
    private List<GameObject> m_rooms;
    public GameObject m_currentRoom;
    public List<Vector3> m_exhibitsList;
    private bool m_ready = false;
    
    protected override void OnStart()
    {
        m_entranceController = GameObject.Find("Entrance").GetComponent<EntranceController>();
        m_rooms = GameObject.FindGameObjectsWithTag("Room").ToList();
        if (blackboard.visitedRooms == null)
            blackboard.visitedRooms = new List<int>();
        
        Vector3 destination = Vector3.zero;
        bool leaveRoom = true;
        GameObject room = null;
        if (blackboard.exhibitsVisited == 0)
            room = FindNextRoom();
        else 
            room = FindCurrentRoom();
        
        if (room.transform.Find("Exhibits") != null) {
            leaveRoom = false;
            RecastGraph graph = AstarPath.active.data.recastGraph;
            if(GameObject.ReferenceEquals(room, m_currentRoom) == false && !IsRoomVisited(room)){
                blackboard.exhibitsVisited = 0;
                blackboard.exhibitIndex = GetNearestExhibit(room);
                blackboard.exhibitsVisited += 1;
                destination = (Vector3)graph.GetNearest(m_exhibitsList[blackboard.exhibitIndex]).node.position;
            }
            else {
                blackboard.exhibitIndex += 1;
                blackboard.exhibitsVisited += 1;
                if (blackboard.exhibitsVisited >= m_exhibitsList.Count) {
                    leaveRoom = true;
                    blackboard.exhibitsVisited = 0;
                }else
                    destination = (Vector3)graph.GetNearest(m_exhibitsList[blackboard.exhibitIndex % m_exhibitsList.Count]).node.position;    
            }
            m_currentRoom = room;
        }
        
        if(leaveRoom) {
            m_currentRoom = FindCurrentRoom();
            if (m_currentRoom.transform.Find("ExitSign") != null && IsRoomVisited(m_currentRoom) 
                                                                 && blackboard.visitedRooms.Count > 2) {
                destination = m_entranceController.GetExitRowPoint();
                blackboard.destroyEnabled = true;
            }
            else {
                destination = m_currentRoom.GetComponent<RoomManager>().Checkpoint();
            }
        }
        if(!blackboard.visitedRooms.Contains(m_currentRoom.GetInstanceID()))
            blackboard.visitedRooms.Add(m_currentRoom.GetInstanceID());

        blackboard.moveToPosition.x = destination.x;
        blackboard.moveToPosition.y = destination.y;
        blackboard.moveToPosition.z = destination.z;
        m_ready = true;
    }

    private int GetNearestExhibit(GameObject room)
    {
        m_exhibitsList = new List<Vector3>(room.GetComponent<RoomManager>().GetExhibits());
        int index = 0;
        float dis = Single.PositiveInfinity;
        for (int i = 0; i < m_exhibitsList.Count; i++) {
            float disTemp = Vector3.Distance(context.transform.position, m_exhibitsList[i]);
            if (disTemp < dis) {
                dis = disTemp;
                index = i;
            }
        }

        return index;
    }

    private bool IsRoomVisited(GameObject room)
    {
        if (blackboard.visitedRooms.Contains(room.GetInstanceID()))
            return true;
        return false;
    }

    //Get next nearest room
    private GameObject FindCurrentRoom()
    {
        float distance = Single.PositiveInfinity;
        GameObject nearestRoom = m_rooms[0];
        foreach (var room in m_rooms) {
            float tempDis = Vector3.Distance(room.transform.position, context.transform.position);
            if (tempDis < distance) {
                distance = tempDis;
                nearestRoom = room;
            }
        }
        return nearestRoom;
    }

    private GameObject FindNextRoom()
    {
        float distance = Single.PositiveInfinity;
        GameObject nearestRoom = m_rooms[0];
        foreach (var room in m_rooms) {
            float tempDis = Vector3.Distance(room.transform.position, context.transform.position);
            if (tempDis < distance && GameObject.ReferenceEquals( m_currentRoom, room) == false) {
                distance = tempDis;
                nearestRoom = room;
            }
        }
        return nearestRoom;
    }

    protected override void OnStop() {
    }
    
    protected override State OnUpdate()
    {
        if(m_ready)
            return State.Success;
        return State.Running;
    }
}
