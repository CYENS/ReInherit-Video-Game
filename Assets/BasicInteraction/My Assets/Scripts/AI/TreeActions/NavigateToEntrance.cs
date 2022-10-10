using System.Collections;
using System.Collections.Generic;
using Cyens.ReInherit;
using UnityEngine;
using TheKiwiCoder;
using Pathfinding;
using TreeEditor;

namespace Cyens.ReInherit
{
    public class NavigateToEntrance : ActionNode
    {
        [SerializeField] private float m_tolerance = 0.5f;
        private EntranceController m_entranceController;

        protected override void OnStart()
        {
            m_entranceController = GameObject.Find("Entrance").GetComponent<EntranceController>();
            context.agentAstar.destination = blackboard.moveToPosition;
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (context.agentAstar.pathPending) {
                return State.Running;
            }

            if (context.agentAstar.remainingDistance <= m_tolerance) {
                m_entranceController.DecreaseDesnity(blackboard.entranceRowId);
                return State.Success;
            }

            return State.Running;
        }
    }
}
