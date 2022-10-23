using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cyens.ReInherit;
using UnityEngine;
using TheKiwiCoder;

public class CheckForExhibit : ActionNode
{
    public List<RoomExhibit> m_exhibitsList;
    private bool m_ready = false;
    
    protected override void OnStart()
    {
        blackboard.waitSeeExhibit = false;
        //If current room does not contains exhibits, return
        if (blackboard.currentRoom.GetComponent<RoomManager>().GetExhibits() == null) {
            m_ready = true;
            return;
        }

        //If agent is leaving, return
        if (blackboard.moveToExit) {
            m_ready = true;
            return;
        }

        //-------------Exhibits exist in room---------------
        //If agent has visited all the exhibits in room, return.
        if (blackboard.exhibitsVisited >= m_exhibitsList.Count && blackboard.firstExhibit == false) {
            blackboard.firstExhibit = true;
            m_ready = true;
            return;
        }

        blackboard.waitSeeExhibit = true;
        //If agent just entered the room, find the closest exhibit
        if (blackboard.firstExhibit) {
            blackboard.exhibitsVisited = 0;
            if (blackboard.exhibitInitial) {
                blackboard.exhibitInitial = false;
                m_exhibitsList = new List<RoomExhibit>(blackboard.currentRoom.GetComponent<RoomManager>().GetExhibits());
                blackboard.exhibitIndex = GetNearestExhibit(blackboard.currentRoom);
            }
            else {
                m_exhibitsList = new List<RoomExhibit>(blackboard.nextRoom.GetComponent<RoomManager>().GetExhibits());
                blackboard.exhibitIndex = GetNearestExhibit(blackboard.nextRoom);
            }
            blackboard.firstExhibit = false;
        }
        //Else visit the next exhibit in list
        else {
            m_exhibitsList = new List<RoomExhibit>(blackboard.currentRoom.GetComponent<RoomManager>().GetExhibits());
            m_exhibitsList[blackboard.exhibitIndex].ReleasePoint(blackboard.exhibitPosIndex);
            blackboard.exhibitIndex = (blackboard.exhibitIndex + 1) % m_exhibitsList.Count;
        }
        
        //Find a point around the exhibit, if exhibit is busy, move to the next one
        blackboard.exhibitPosIndex = m_exhibitsList[blackboard.exhibitIndex].GetAvailablePointIndex();
        while(blackboard.exhibitPosIndex == -1) {
            blackboard.exhibitIndex = (blackboard.exhibitIndex + 1) % m_exhibitsList.Count;
            blackboard.exhibitPosIndex = m_exhibitsList[blackboard.exhibitIndex].GetAvailablePointIndex();        
            blackboard.exhibitsVisited += 1;
        } 
        Vector3 poiPos = m_exhibitsList[blackboard.exhibitIndex].GetIndexPoint(blackboard.exhibitPosIndex);

        blackboard.exhibitsVisited += 1;
        
        blackboard.moveToPosition.x = poiPos.x;
        blackboard.moveToPosition.y = poiPos.y;
        blackboard.moveToPosition.z = poiPos.z;
        m_ready = true;
    }

    //Return the nearest exhibit
    private int GetNearestExhibit(GameObject room)
    {
        int index = 0;
        float dis = Single.PositiveInfinity;
        for (int i = 0; i < m_exhibitsList.Count; i++) {
            float disTemp = Vector3.Distance(context.transform.position, m_exhibitsList[i].transform.position);
            if (disTemp < dis) {
                dis = disTemp;
                index = i;
            }
        }
        return index;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if(m_ready)
            return State.Success;
        return State.Running;
    }
}
