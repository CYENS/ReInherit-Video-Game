using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

namespace Cyens.ReInherit
{
    /// <summary>
    /// Makes the agent head towards the exit
    /// </summary>
    public class GoToExit : ActionNode
    {

        protected Vector3 exitPoint;

        [SerializeField] private bool destroyOnExit = true;
        [SerializeField] private bool verbose = false;
        

        protected override void OnStart()
        {
            GameObject exitDoor = GameObject.FindGameObjectWithTag("Exit");

            // Find exit
            exitPoint = exitDoor.transform.position;

            if(verbose) Debug.Log(""+context.gameObject.name + " is heading towards exit");
        }

        protected override void OnStop()
        {
            // Make the agent dissapear?
            if(destroyOnExit)
                Destroy(context.gameObject);
        }

        protected override State OnUpdate()
        {
            context.agent.SetDestination(exitPoint);
            float distance = context.agent.remainingDistance;
            
            if( distance <= float.Epsilon ) return State.Success;
            return State.Running;
        }

    }
}
