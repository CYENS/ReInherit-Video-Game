using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class Moving : StateMachineBehaviour
    {
        [Header("Animator Parameters")]
        [SerializeField] private string m_motionSpeedParam = "MotionSpeed";
        [SerializeField] private string m_motionParam = "Speed";
        [Tooltip("An adjustment value in case movement speed parameter isn't a value from 0 to 1")]
        [SerializeField] private float m_motionMult = 3.0f;
        protected IAstarAI m_agentAstar;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("Idle", false);
            m_agentAstar = animator.GetComponent<IAstarAI>();
            m_agentAstar.destination = new Vector3(2, 0, 50);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (m_agentAstar.reachedDestination) {
                animator.SetBool("Idle", true);
                animator.Play("Idle");
            }

            animator.SetFloat(m_motionSpeedParam, 1.0f);
            Vector3 velocity = m_agentAstar.velocity;
            float maxSpeed = m_agentAstar.maxSpeed;
            // This will make the character walk/run based on the animator speed
            float moveSpeed = velocity.magnitude / maxSpeed;
            animator.SetFloat(m_motionParam, moveSpeed * m_motionMult);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}
