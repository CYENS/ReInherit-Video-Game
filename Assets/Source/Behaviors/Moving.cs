using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Cyens.ReInherit
{
    public class Moving : StateMachineBehaviour
    {
        private Visitor m_visitor;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_visitor = animator.GetComponent<Visitor>();
            //m_visitor.SetCurrentStandingNodePenalty(0);
            if(m_visitor.VisitorBehavior != Visitor.Behavior.Exiting)
                m_visitor.VisitorBehavior = Visitor.Behavior.Walking;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (m_visitor.NavAgent.remainingDistance < 0.1f) {
                animator.SetBool("DestinationReached", true);
                // If destination is the exit, despawn agent
                if(m_visitor.VisitorBehavior == Visitor.Behavior.Exiting)
                    m_visitor.DeSpawn();
                //m_visitor.SetCurrentStandingNodePenalty(10000);
                m_visitor.RotateVisitorTowardsDestination();
                animator.SetBool("Walk", false);
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
    }
}