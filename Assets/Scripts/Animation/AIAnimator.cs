using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;

namespace Cyens.ReInherit
{
    public class AIAnimator : MonoBehaviour
    {
        public bool isAstar;
        protected Animator animator;
        protected NavMeshAgent agent;
        protected IAstarAI agentAstar;


        [Header("Animator Parameters")]
        [SerializeField] private string motionSpeedParam = "MotionSpeed";

        [SerializeField] private string motionParam = "Speed";

        [Tooltip("An adjustment value in case movement speed parameter isn't a value from 0 to 1")]
        [SerializeField] private float motionMult = 6.0f;

        void Start()
        {
            animator = GetComponent<Animator>();
            if (gameObject.TryGetComponent(out NavMeshAgent navAgent))
                agent = navAgent;
            if (gameObject.TryGetComponent(out IAstarAI navAgentAstar))
                agentAstar = navAgentAstar;
        }

        // Update is called once per frame
        void Update()
        {
            animator.SetFloat(motionSpeedParam, 1.0f);
            Vector3 velocity;
            float maxSpeed;
            
            if (isAstar) {
                velocity = agentAstar.velocity;
                maxSpeed = agentAstar.maxSpeed;
            }
            else {
                velocity = agent.velocity;
                maxSpeed = agent.speed;
            }
                // This will make the character walk/run based on the animator speed
            float moveSpeed = velocity.magnitude / maxSpeed;
            animator.SetFloat(motionParam, moveSpeed*motionMult);
        }
    }
}
