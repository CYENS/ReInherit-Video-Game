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
        private bool m_donationGiven = false;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_visitor = animator.GetComponent<Visitor>();
            m_visitor.VisitorBehavior = Visitor.Behavior.Watching;
            
            // If enter the state from idle state, set timer and
            // add a little randomness to watching time.
            // Then set destination for the next artifact
            if (animator.GetFloat("WatchTimer") <= 0f) {
                m_timeCounter = m_visitor.LookDuration * UnityEngine.Random.Range(0.5f, 1.25f);
                animator.SetFloat("WatchTimer", m_timeCounter);
                animator.SetBool("DestinationReached", false);
                // If visitor is view its spawn artifact wait for timer before select next artifact
                if (m_visitor.InSpawnArtifact) {
                    m_visitor.InSpawnArtifact = false;
                    m_timeCounter /= UnityEngine.Random.Range(1.5f, 2f);
                    animator.SetFloat("WatchTimer", m_timeCounter);
                    animator.SetBool("DestinationReached", true);
                }
                else {
                    m_visitor.NavAgent.destination = CalculateDestination(animator);
                    m_visitor.NavAgent.SearchPath();
                    animator.SetBool("Walk", true);
                }
            }
            // Check if visitor could talk to other visitor
            else if(animator.GetBool("OpenToTalk"))
                CheckTalkAvailability(animator);
            // Talk ended, rotate back to artifact
            else
                m_visitor.RotateVisitorTowardsDestination();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // If visitor arrived at the artifact, start decreasing the time counter
            if (animator.GetBool("DestinationReached")) {
                m_timeCounter -= Time.deltaTime;
                
                // If timer is 0, return to release artifact slot and return to idle state
                if (m_timeCounter <= 0) {
                    m_visitor.ReleaseArtifactSlot();
                    
                    // Give donation, at the time random for debugging
                    float random = UnityEngine.Random.Range(0f, 1f);
                    if (!m_donationGiven && random <= 0.5f) {
                        m_visitor.GiveCoins(UnityEngine.Random.Range(5, 7));
                        m_donationGiven = true;
                    }

                    animator.SetBool("Idle", true);
                }
            }
            animator.SetFloat("WatchTimer", m_timeCounter);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
        

        private void GoToExit(Animator animator)
        {
            animator.SetBool("Exit", true);
            m_visitor.VisitorBehavior = Visitor.Behavior.Exiting;
        }

        // Finds next artifact to visit and get free slot around that artifact
        private Vector3 CalculateDestination(Animator animator)
        {
            VisitorManager visitorManager = VisitorManager.Instance;
            // Get artifacts and sort them based on distance
            ArtifactVisitorHandler[] handlers = visitorManager.GetVisitorHandlers();

            handlers = handlers.OrderBy((d) => (d.transform.position - animator.transform.position).sqrMagnitude).ToArray();

            // If agent visited all artifacts or boredome threshold reached, move to exit
            if (handlers.Length == m_visitor.visitedArtifacts.Count || m_visitor.Boredome >= 1) {
                GoToExit(animator);
                return visitorManager.GetExitPosition();
            }

            Vector3 freeSlot = animator.transform.position;
            int index = 1;
            bool loop = true;
            ArtifactVisitorHandler nextArtifact;
            do {

                if( index >= handlers.Length )
                {
                    GoToExit(animator);
                    return visitorManager.GetExitPosition();
                }

                nextArtifact = handlers[index];
                if (m_visitor.visitedArtifacts.Contains(nextArtifact) == false) {
                    
                    if( nextArtifact.TryGetFreeSpot(out freeSlot) )
                    {
                        loop = false;
                    }
                        
                }
                index += 1;
            } while (loop);
            
            m_visitor.visitedArtifacts.Add(nextArtifact);
            m_visitor.SetArtifact(nextArtifact, (int)freeSlot.y);
            freeSlot.y = 0f;
            return freeSlot;
        }

        private void CheckTalkAvailability(Animator animator)
        {
            Visitor closer = VisitorManager.Instance.FindCloserAgent(m_visitor, 3f);
            if (closer != null && closer.Animator.GetBool("OpenToTalk") &&
                closer.Animator.GetBool("Walk") == false && closer.TalkVisitor == null 
                && closer.Animator.GetFloat("WatchTimer") >= 2f) {
                closer.TalkVisitor = m_visitor;
                m_visitor.TalkVisitor = closer;
                animator.SetBool("Talk", true);
                closer.Animator.SetBool("Talk", true);
            }
        }
    }
}
