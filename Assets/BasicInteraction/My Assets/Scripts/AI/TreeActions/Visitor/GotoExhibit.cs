using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

namespace Cyens.ReInherit
{
    public class GotoExhibit : ActionNode
    {
        protected override void OnStart()
        {
            context.agent.isStopped = false;
            Debug.Log("Going to exhibit "+blackboard.exhibit);
        }

        protected override void OnStop()
        {
            context.agent.isStopped = true;
        }

        protected override State OnUpdate()
        {
            if( blackboard.exhibit == null ) 
                return State.Failure;
            
            context.agent.SetDestination( blackboard.exhibit.transform.position );

            bool reached = context.agent.remainingDistance < float.Epsilon;
            if( Vector3.Distance(context.transform.position, blackboard.exhibit.transform.position) > 3.0f )
                reached = false;

            return reached ? State.Success : State.Running;
        }
    }
}
