using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

namespace Cyens.ReInherit
{
    public class CheckForGarbage : ActionNode
    {
        private GarbageManager m_garbageManager;
        private Transform m_janitorBase;
        
        protected override void OnStart() {
            m_garbageManager = GameObject.Find("GarbageParent").GetComponent<GarbageManager>();
            m_janitorBase = GameObject.Find("JanitorBase").transform;
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate() {
            if (m_garbageManager.GetGarbage.Count == 0) {
                blackboard.moveToPosition.x = m_janitorBase.position.x;
                blackboard.moveToPosition.z = m_janitorBase.position.z;
                blackboard.garbage = null;
            }
            else {
                Vector3 nextGarbagePos = m_garbageManager.GetGarbage[0].gameObject.transform.position;
                blackboard.moveToPosition.x = nextGarbagePos.x;
                blackboard.moveToPosition.z = nextGarbagePos.z;
                blackboard.garbage = m_garbageManager.GetGarbage[0].gameObject;
            }
            
            return State.Success;
        }
    }
}
