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
    private bool m_ready = false;
    
    protected override void OnStart()
    {
        //If first time node running, initalize lists
        if(blackboard.rooms == null || blackboard.rooms.Count == 0)
            blackboard.rooms = GameObject.FindGameObjectsWithTag("Room").ToList();
        if (blackboard.visitedRooms == null)
            blackboard.visitedRooms = new List<int>();

        Vector3 destination = Vector3.zero;
        blackboard.currentRoom = FindCurrentRoom();
        blackboard.nextRoom = FindNextRoom();
        //if the current room has an exit sign and is not first time visited, navigate to exit
        if (blackboard.currentRoom.transform.Find("ExitSign") != null 
            && IsRoomVisited(blackboard.currentRoom) && blackboard.visitedRooms.Count > 2) {
            m_entranceController = GameObject.Find("Entrance").GetComponent<EntranceController>();
            destination = m_entranceController.GetExitRowPoint();
            blackboard.moveToExit = true;
        }
        else {
            destination = blackboard.currentRoom.GetComponent<RoomManager>().Checkpoint();
        }
        
        //Add current room to visited rooms list
        if(!blackboard.visitedRooms.Contains(blackboard.currentRoom.GetInstanceID()))
            blackboard.visitedRooms.Add(blackboard.currentRoom.GetInstanceID());

        //Set next point the exit door of the room.
        //If agent has to see exhibits in current room, will manage that
        //the next node in tree sequence
        blackboard.moveToPosition.x = destination.x;
        blackboard.moveToPosition.y = destination.y;
        blackboard.moveToPosition.z = destination.z;
        m_ready = true;
    }

    //Find current room the agent is in
    private GameObject FindCurrentRoom()
    {
        float distance = Single.PositiveInfinity;
        GameObject nearestRoom = blackboard.rooms[0];
        foreach (var room in blackboard.rooms) {
            float tempDis = Vector3.Distance(room.transform.position, context.transform.position);
            if (tempDis < distance) {
                distance = tempDis;
                nearestRoom = room;
            }
        }
        return nearestRoom;
    }

    //Get next nearest room that agent will enter
    private GameObject FindNextRoom()
    {
        float distance = Single.PositiveInfinity;
        GameObject nearestRoom = blackboard.rooms[0];
        Vector3 checkpoint = blackboard.currentRoom.GetComponent<RoomManager>().Checkpoint();
        foreach (var room in blackboard.rooms) {
            float tempDis = Vector3.Distance(room.transform.position, checkpoint);
            if (tempDis < distance && GameObject.ReferenceEquals(blackboard.currentRoom, room) == false) {
                distance = tempDis;
                nearestRoom = room;
            }
        }
        return nearestRoom;
    }

    //Check if room is already visited
    private bool IsRoomVisited(GameObject room)
    {
        if (blackboard.visitedRooms.Contains(room.GetInstanceID()))
            return true;
        return false;
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
