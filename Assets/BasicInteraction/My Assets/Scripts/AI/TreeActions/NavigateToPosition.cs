using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

namespace  Cyens.ReInherit.Test.Example
{
    public class NavigateToPosition : ActionNode
    {
        [SerializeField] private float m_speed = 5;
        [SerializeField] private float m_stoppingDistance = 0.1f;
        [SerializeField] private bool m_updateRotation = true;
        [SerializeField] private float m_acceleration = 40.0f;
        [SerializeField] private float m_tolerance = 1.0f;

        private float SetRandomSpeed()
        {
            return m_speed * UnityEngine.Random.Range(0.5f, 1.5f);
        }

        protected override void OnStart()
        {
            if (context.agent != null) {
                context.agent.stoppingDistance = m_stoppingDistance;
                context.agent.speed = SetRandomSpeed();
                context.agent.destination = blackboard.moveToPosition;
                context.agent.updateRotation = m_updateRotation;
                context.agent.acceleration = m_acceleration;
            }
            else {
                context.agentAstar.maxSpeed = SetRandomSpeed();
                context.agentAstar.destination = blackboard.moveToPosition;
            }
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate() {
            if (context.agent != null) {
                if (context.agent.pathPending) {
                    return State.Running;
                }

                if (context.agent.remainingDistance < m_tolerance) {
                    return State.Success;
                }

                if (context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid) {
                    return State.Failure;
                }
            }
            else {
                if (context.agentAstar.pathPending) {
                    return State.Running;
                }

                if (context.agentAstar.remainingDistance < m_tolerance) {
                    return State.Success;
                }
            }

            return State.Running;
        }
    }
}
