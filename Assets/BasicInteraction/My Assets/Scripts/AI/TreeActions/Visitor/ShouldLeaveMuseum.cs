using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

namespace Cyens.ReInherit
{
    /// <summary>
    /// A conditional node.
    /// Decides whether the agent (visitor) should leave the museum.
    /// </summary>
    public class ShouldLeaveMuseum : ActionNode
    {
        [SerializeField] private bool verbose = false;

        protected override void OnStart()
        {
            
        }

        protected override void OnStop()
        {
            
        }

        protected override State OnUpdate()
        {
            bool disgusted = blackboard.GetValue("disgust") >= 1.0f;
            bool bored = blackboard.GetValue("boredom") >= 1.0f;
            bool tired = blackboard.GetValue("tiredness") >= 1.0f;

            if(verbose && disgusted) Debug.Log(""+context.gameObject.name + " is disgusted");
            if(verbose && bored) Debug.Log(""+context.gameObject.name + " is bored");
            if(verbose && tired) Debug.Log(""+context.gameObject.name + " is tired");


            if( tired || bored || disgusted )
                return State.Success;

            return State.Failure;
        }

        
    }
}
