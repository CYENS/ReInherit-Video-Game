using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class Idle : StateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("DestinationReached", false);
            animator.SetBool("Talk", false);
            animator.SetBool("OpenToTalk", false);
            
            // Decide if visitor will be open to talk in the next artifact visit
            float random = UnityEngine.Random.Range(0f, 1f);
            if(random <= 0.8f)
                animator.SetBool("OpenToTalk", true);

            animator.SetBool("Idle", false);
            
            animator.SetFloat("WatchTimer", 0f);
            animator.SetBool("Watch", true);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
    }
}