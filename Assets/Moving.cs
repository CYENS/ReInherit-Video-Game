using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using System.Linq;

namespace Cyens.ReInherit
{
    public class Moving : StateMachineBehaviour
    {
        [Header("Animator Parameters")]
        [SerializeField] private string m_motionSpeedParam = "MotionSpeed";
        [SerializeField] private string m_motionParam = "Speed";
        [Tooltip("An adjustment value in case movement speed parameter isn't a value from 0 to 1")]
        [SerializeField] private float m_motionMult = 3.0f;
        private IAstarAI m_agentAstar;
        private Visitor m_visitor;
        private Vector3 m_nextExhibitPos;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_agentAstar = animator.GetComponent<IAstarAI>();
            m_visitor = animator.GetComponent<Visitor>();
            CalculateDestination(animator);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (m_agentAstar.hasPath && m_agentAstar.remainingDistance <= 0.5f) {
                m_visitor.RotateVisitorTowardsExhibit();
                animator.SetBool("Idle", true);
            }

            animator.SetFloat(m_motionSpeedParam, 1.0f);
            Vector3 velocity = m_agentAstar.velocity;
            float maxSpeed = m_agentAstar.maxSpeed;
            // This will make the character walk/run based on the animator speed
            float moveSpeed = velocity.magnitude / maxSpeed;
            animator.SetFloat(m_motionParam, moveSpeed * m_motionMult);
        }

        private void CalculateDestination(Animator animator)
        {
            if (animator.GetBool("Watch")) {
                Transform[] exhibits = VisitorManagerTesting.Instance.GetExhibits();
                exhibits = exhibits.OrderBy((d) => (d.position - animator.transform.position).sqrMagnitude).ToArray();
                int index = 1;
                do {
                    m_nextExhibitPos = exhibits[index].position;
                    index += 1;
                } while (m_visitor.visitedExhibits.Contains(m_nextExhibitPos));
                m_visitor.visitedExhibits.Add(m_nextExhibitPos);
                m_visitor.SetExhibit(m_nextExhibitPos);
                m_agentAstar.destination = RandomPointOnCircleEdge(m_nextExhibitPos, new float[]{1.5f, 2.5f});
            }
            else if (animator.GetBool("Talk")) {
                
            }
        }
        
        private Vector3 RandomPointOnCircleEdge(Vector3 pos, float[] radius)
        {
            float r = UnityEngine.Random.Range(radius[0], radius[1]);
            var vector2 = Random.insideUnitCircle.normalized * r;
            return pos + new Vector3(vector2.x, 0, vector2.y);
        }
        

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("Walk", false);
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
