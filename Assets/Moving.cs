using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class Moving : StateMachineBehaviour
    {
        private Visitor m_visitor;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("DestinationReached", false);
            animator.SetBool("Walk", true);
            m_visitor = animator.GetComponent<Visitor>();
            if(m_visitor.VisitorBehavior != Visitor.Behavior.Exiting)
                m_visitor.VisitorBehavior = Visitor.Behavior.Walking;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (m_visitor.NavAgent.reachedEndOfPath) {
                if(m_visitor.VisitorBehavior == Visitor.Behavior.Exiting)
                    m_visitor.DeSpawn();
                animator.SetBool("DestinationReached", true);   
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("Walk", false);
        }
    }
}
