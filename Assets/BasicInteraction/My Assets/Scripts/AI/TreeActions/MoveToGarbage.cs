using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class MoveToGarbage : ActionNode
{
    public float speed = 3;
    public float stoppingDistance = 0.1f;
    public bool updateRotation = true;
    public float acceleration = 25.0f;
    public float tolerance = 1.0f;

    protected override void OnStart() {
        context.agent.stoppingDistance = stoppingDistance;
        context.agent.speed = speed;
        context.agent.destination = blackboard.moveToPosition;
        context.agent.updateRotation = updateRotation;
        context.agent.acceleration = acceleration;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        //If garbage collected by player, terminate action node to look for next destination
        if(blackboard.garbage != null && !blackboard.garbage.activeSelf)
            return State.Success;
        
        if(blackboard.garbage == null)
            return State.Success;
        
        if (context.agent.remainingDistance < tolerance) {
            return State.Success;
        }

        if (Vector3.Distance(context.agent.transform.position, blackboard.garbage.transform.position) <= 3f)
            return State.Success;

        return State.Running;
    }
}

