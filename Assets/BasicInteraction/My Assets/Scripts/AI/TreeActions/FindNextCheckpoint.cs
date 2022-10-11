using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cyens.ReInherit;
using UnityEngine;
using TheKiwiCoder;

public class FindNextCheckpoint : ActionNode
{
    private List<GameObject> m_rooms;
    private GameObject m_currentRoom;
    private bool m_ready = false;
    
    protected override void OnStart()
    {
        m_rooms = GameObject.FindGameObjectsWithTag("Room").ToList();
        FindCurrentRoom();

        Vector3 destination;
        Transform exit = m_currentRoom.transform.Find("Exit");
        if (exit != null) {
            destination = GetRandomGoalPoint(exit.GetComponent<BoxCollider>());
            blackboard.destroyEnabled = true;
        }
        else 
            destination = m_currentRoom.GetComponent<RoomManager>().Checkpoint();
        
        blackboard.moveToPosition.x = destination.x;
        blackboard.moveToPosition.y = destination.y;
        blackboard.moveToPosition.z = destination.z;
        m_ready = true;
    }

    //Get next nearest room
    private void FindCurrentRoom()
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
        m_currentRoom = nearestRoom;
    }
    
    //Return random point inside a box collider
    public Vector3 GetRandomGoalPoint(BoxCollider box)
    {
        return new Vector3(
            UnityEngine.Random.Range(box.bounds.min.x, box.bounds.max.x),
            0,
            UnityEngine.Random.Range(box.bounds.min.z, box.bounds.max.z)
        );
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
