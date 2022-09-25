using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class MoveToGarbage : ActionNode
{
    public float m_speed = 3;
    public float m_stoppingDistance = 0.1f;
    public bool m_updateRotation = true;
    public float m_acceleration = 25.0f;
    public float m_tolerance = 1.0f;

    protected override void OnStart() {
        if (context.agent != null) {
            context.agent.stoppingDistance = m_stoppingDistance;
            context.agent.speed = m_speed;
            context.agent.destination = blackboard.moveToPosition;
            context.agent.updateRotation = m_updateRotation;
            context.agent.acceleration = m_acceleration;
        }
        else {
            context.agentAstar.maxSpeed = m_speed;
            context.agentAstar.destination = blackboard.moveToPosition;
        }
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        //If garbage collected by player or other janitor, terminate action node to look for next destination
        if (blackboard.garbage == null)
            return State.Success;
        
        if(!blackboard.garbage.activeSelf)
            return State.Success;

        if (context.agent != null) {
            if (context.agent.remainingDistance < m_tolerance) {
                return State.Success;
            }
        }
        else {
            if (context.agentAstar.remainingDistance < m_tolerance) {
                return State.Success;
            }
        }

        return State.Running;
    }
}

