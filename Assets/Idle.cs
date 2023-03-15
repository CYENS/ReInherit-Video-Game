using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class Idle : StateMachineBehaviour
    {
        [Range(0.0f, 10.0f)] [SerializeField] private float m_waitTime = 5f;
        private float m_timeCounter;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_timeCounter = m_waitTime;
            
            float random = UnityEngine.Random.Range(0f, 1f);
            if(random <= 1.0f)
                animator.SetBool("Watch", true);
            else
                animator.SetBool("Talk", true);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        { 
            m_timeCounter -= Time.deltaTime;
            if (m_timeCounter <= 0)
            {
                m_timeCounter = m_waitTime;
                animator.SetBool("Walk", true);
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("Idle", false);
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}
