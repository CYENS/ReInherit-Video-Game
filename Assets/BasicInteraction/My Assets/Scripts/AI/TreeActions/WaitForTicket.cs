using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using UnityEngine.ProBuilder.MeshOperations;

namespace Cyens.ReInherit
{
    public class WaitForTicket : ActionNode
    {
        [SerializeField] private float m_duration = 1.0f;
        private float m_startTime;
        private EntranceController m_entranceController;

        private float GenerateRandomDuration()
        {
            return UnityEngine.Random.Range(5f, 15f);
        }

        protected override void OnStart()
        {
            context.agentAstar.canMove = false;
            blackboard.waitingForTicket = true;
            m_entranceController = GameObject.Find("Entrance").GetComponent<EntranceController>();
            m_startTime = Time.time;
            m_duration = GenerateRandomDuration();
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (Time.time - m_startTime > m_duration) {
                Vector3 destination = m_entranceController.GetRandomGoalPoint();
                blackboard.moveToPosition.x = destination.x;
                blackboard.moveToPosition.y = destination.y;
                blackboard.moveToPosition.z = destination.z;
                context.agentAstar.canMove = true;
                blackboard.waitingForTicket = false;

                context.gameObject.GetComponent<EntranceAgent>().enabled = false;
                
                return State.Success;
            }

            return State.Running;
        }
    }
}
