using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Cyens.ReInherit
{
    public class Watch : StateMachineBehaviour
    {
        private float m_timeCounter;
        private Visitor m_visitor;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_visitor = animator.GetComponent<Visitor>();
            m_visitor.VisitorBehavior = Visitor.Behavior.Watching;
            // Add a little randomness to watching time
            if (animator.GetFloat("WatchTimer") <= 0f) {
                m_timeCounter = m_visitor.LookDuration * UnityEngine.Random.Range(0.75f, 1.25f);
                animator.SetFloat("WatchTimer", m_timeCounter);
            }

            // Set destination artifact only the first time that enter this state
            if(animator.GetBool("DestinationReached") == false)
                m_visitor.NavAgent.destination = CalculateDestination(animator);
            
            m_visitor.RotateVisitorTowardsDestination();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // If visitor arrived at the artifact, start decreasing the time counter
            if (animator.GetBool("DestinationReached")) {
                
                // Check if visitor could talk to an other visitor
                if (animator.GetBool("OpenToTalk")) {
                    CheckTalkAvailability(animator);
                }

                if (animator.GetBool("Walk") == false) {
                    m_timeCounter -= Time.deltaTime;
                }

                if (m_timeCounter <= 0) {
                    m_visitor.ReleaseArtifactSlot();
                    animator.SetBool("Idle", true);
                }
            }
            animator.SetFloat("WatchTimer", m_timeCounter);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("DestinationReached", false);
        }
        
        // Finds next artifact to visit and get free slot around that artifact
        private Vector3 CalculateDestination(Animator animator)
        {
            VisitorManager visitorManager = VisitorManager.Instance;
            ArtifactVisitorHandler[] artifacts = visitorManager.GetArtifacts();
            artifacts = artifacts.OrderBy((d) => (d.transform.position - animator.transform.position).sqrMagnitude).ToArray();

            if (artifacts.Length == m_visitor.visitedArtifacts.Count || m_visitor.Boredome >= 1) {
                animator.SetBool("Exit", true);
                m_visitor.VisitorBehavior = Visitor.Behavior.Exiting;
                return visitorManager.GetExitPosition();
            }

            Vector3 freeSlot = animator.transform.position;
            int index = 1;
            bool loop = true;
            ArtifactVisitorHandler nextArtifact;
            do {
                nextArtifact = artifacts[index];
                if (m_visitor.visitedArtifacts.Contains(nextArtifact) == false) {
                    freeSlot = nextArtifact.GetFreeViewSpot();
                    if (freeSlot != Vector3.zero)
                        loop = false;
                }
                index += 1;
            } while (loop);
            
            m_visitor.visitedArtifacts.Add(nextArtifact);
            m_visitor.SetArtifact(nextArtifact, (int)freeSlot.y);
            return freeSlot;
        }

        private void CheckTalkAvailability(Animator animator)
        {
            Visitor closer = VisitorManager.Instance.FindCloserAgent(m_visitor, 4f);
            if (closer != null && closer.Animator.GetBool("OpenToTalk") &&
                closer.Animator.GetBool("Walk") == false && closer.TalkVisitor == null) {
                closer.TalkVisitor = m_visitor;
                m_visitor.TalkVisitor = closer;
                animator.SetBool("Talk", true);
                closer.Animator.SetBool("Talk", true);
            }
        }
    }
}
