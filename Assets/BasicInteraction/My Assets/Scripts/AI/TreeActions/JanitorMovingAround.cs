using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

namespace Cyens.ReInherit
{
    public class JanitorMovingAround : ActionNode
    {
        private Transform m_garbagePointsParent;
        private GarbageManager m_garbageManager;
        private List<Vector3> m_JanitorPoints;
        private List<bool> m_pointVisited;
        private bool m_hasDestination;
        private Vector3 m_nextPos;

        protected override void OnStart()
        {
            m_garbagePointsParent = GameObject.Find("JanitorPoints").transform;
            m_garbageManager = GameObject.Find("GarbageParent").GetComponent<GarbageManager>();
            m_JanitorPoints = new List<Vector3>(m_garbagePointsParent.childCount);
            foreach (Transform p in m_garbagePointsParent) {
                m_JanitorPoints.Add(p.position);
            }
        }

        protected override void OnStop() {
        }

        private bool IsPointInFront(Vector3 point)
        {
            Vector3 pointVector = (point - context.transform.position).normalized;

            if (Vector3.Dot(context.transform.forward, pointVector) >= -0.3f)
                return true;
            
            return false;
        }

        private void SetNewDestination()
        {
            bool destinationFound = false;
            int counter = 0;
            while(destinationFound == false) {
                int rdIndex = UnityEngine.Random.Range(0, m_JanitorPoints.Count);
                Vector3 point = m_JanitorPoints[rdIndex];

                if (IsPointInFront(point) == true) {
                    m_nextPos = point;
                    destinationFound = true;
                }

                //if janitor stack and none of the points is in front, select a random point
                counter += 1;
                if (counter >= m_JanitorPoints.Count - 1) {
                    m_nextPos = point;
                    destinationFound = true;
                }
            }
            
            blackboard.moveToPosition.x = m_nextPos.x;
            blackboard.moveToPosition.y = m_nextPos.y;
            blackboard.moveToPosition.z = m_nextPos.z;
        }
        
        private void CheckForVisibleGarbage()
        {
            if (m_garbageManager.GetGarbage.Count == 0) {
                blackboard.garbage = null;
                return;
            }

            int index = -1;
            for(int i=0; i<m_garbageManager.GetGarbage.Count; i++) {
                Vector3 point = m_garbageManager.GetGarbage[i].transform.position;
                
                //Single ray to garbage origin(will expand this later for beter discovery)
                RaycastHit hit;
                if (Physics.Linecast(context.transform.position, point, out hit)) {
                    if (hit.transform.tag == "Garbage") {
                        Debug.Log("Garbage Found!");
                        index = i;
                    }
                }
            }
            
            if(index >= 0) {
                Vector3 nextGarbagePos = m_garbageManager.GetGarbage[index].gameObject.transform.position;
                blackboard.moveToPosition.x = nextGarbagePos.x;
                blackboard.moveToPosition.y = nextGarbagePos.y;
                blackboard.moveToPosition.z = nextGarbagePos.z;
                blackboard.garbage = m_garbageManager.GetGarbage[index].gameObject;    
            }else
                blackboard.garbage = null;
        }
        
        protected override State OnUpdate() {
            if(m_nextPos == null || (m_nextPos != blackboard.moveToPosition))
                SetNewDestination();
            CheckForVisibleGarbage();

            return State.Success;
        }
    }
}
