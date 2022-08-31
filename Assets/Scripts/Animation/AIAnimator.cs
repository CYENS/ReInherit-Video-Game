using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Cyens.ReInherit
{
    public class AIAnimator : MonoBehaviour
    {
        protected Animator animator;
        protected NavMeshAgent agent;


        [Header("Animator Parameters")]
        [SerializeField] private string motionSpeedParam = "MotionSpeed";

        [SerializeField] private string motionParam = "Speed";

        [Tooltip("An adjustment value in case movement speed parameter isn't a value from 0 to 1")]
        [SerializeField] private float motionMult = 6.0f;

        void Start()
        {
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
        }

        // Update is called once per frame
        void Update()
        {
            animator.SetFloat(motionSpeedParam, 1.0f);

            // This will make the character walk/run based on the animator speed
            Vector3 velocity = agent.velocity;
            float maxSpeed = agent.speed;
            float moveSpeed = velocity.magnitude / maxSpeed;
            animator.SetFloat(motionParam, moveSpeed*motionMult);
        }
    }
}
