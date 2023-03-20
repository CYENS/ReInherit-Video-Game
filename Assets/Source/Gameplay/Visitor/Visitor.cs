using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Pathfinding.RVO;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Cyens.ReInherit
{
    /// <summary>
    /// Controls the visitor that looks at exhibits and rates your museum
    /// </summary>
    public class Visitor : MonoBehaviour
    {
        public float moveSpeed;
        public enum State { Appear = 0, Alive = 1, Disappear = 2}
        public enum Behavior { Walking = 0, Watching = 1, Talking = 2, Exiting = 3 };
        public enum Emotion { Excited = 0, Bored = 1, Angry = 2 };

        [SerializeField] private int m_id;
        private IAstarAI m_agentAstar;
        private RVOController m_rvoController;
        private Animator m_animator;
        
        // Ghosting effect
        private Ghostify[] m_ghosties;
        private float m_alpha;
        private float m_alphaSpeed;
        
        [SerializeField] private State m_state;
        [SerializeField] private Behavior m_behavior;
        [SerializeField] private Emotion m_emotion;
        
        [Tooltip("The exhibit the visitor focuses on")]
        [SerializeField] private ArtifactVisitorHandler m_artifact;
        private int m_viewSlotID;
        [SerializeField] private Visitor m_talkVisitor;
        [Tooltip("How impressed the visitor was from the last visit")]
        [SerializeField] [Range(0.0f, 1.0f)] private float m_impression;
        [SerializeField] private float m_boredom;
        [SerializeField] private float m_lookDuration;
        [SerializeField] private float m_talkDuration;
        //[SerializeField] private Visitor m_talkTarget;
        public List<ArtifactVisitorHandler> visitedArtifacts;
        
        [Header("Move Animator Parameters")]
        [SerializeField] private string m_motionSpeedParam = "MotionSpeed";
        [SerializeField] private string m_motionParam = "Speed";
        [Tooltip("An adjustment value in case movement speed parameter isn't a value from 0 to 1")]
        [SerializeField] private float m_motionMult = 3.0f;
        private Quaternion m_lookDirection;
        
        public int ID { get => m_id; set => m_id = value; }
        public IAstarAI NavAgent { get => m_agentAstar; }
        public Animator Animator { get => m_animator; }
        public float Impression { get => m_impression; set => m_impression = value; }
        public State VisitorState { get => m_state; set => m_state = value; }
        public Behavior VisitorBehavior { get => m_behavior; set => m_behavior = value; }
        public Emotion VisitorEmotion { get => m_emotion; set => m_emotion = value; }
        public float LookDuration { get => m_lookDuration; }
        public float TalkDuration { get => m_talkDuration; }
        public Visitor TalkVisitor { get => m_talkVisitor; set => m_talkVisitor = value; }
        public float Boredome { get => m_boredom; set => m_boredom = value; }

        public void SetArtifact(ArtifactVisitorHandler artifact, int viewSlotID)
        {
            m_artifact = artifact;
            m_viewSlotID = viewSlotID;
        }

        // Free up the reserved slot around the artifact
        public void ReleaseArtifactSlot()
        {
            if (m_artifact != null) {
                m_artifact.UnuseViewSpot(m_viewSlotID);
                m_artifact = null;
                m_viewSlotID = -1;
            }
        }

        private void Awake()
        {
            m_animator = GetComponent<Animator>();
            m_rvoController = GetComponent<RVOController>();
            m_agentAstar = GetComponent<IAstarAI>();
            visitedArtifacts = new List<ArtifactVisitorHandler>();
            m_ghosties = GetComponentsInChildren<Ghostify>(true);
            m_lookDuration = UnityEngine.Random.Range(7.5f, 12f);
            m_talkDuration = 10f;
        }

        void Start()
        {
            m_alphaSpeed = Random.Range(0.25f,1.0f);
            m_alpha = Random.Range(-m_alphaSpeed,0.0f);
            SetOpacity(m_alpha);
            m_impression = 0.0f;
        }

        private void SetOpacity(float alpha)
        {
            if (m_ghosties == null) return;

            foreach( var ghostie in m_ghosties)
            {
                ghostie.enabled = alpha < 1.0f - float.Epsilon;
                ghostie.SetAlpha(alpha);
            }
        }

        // Set look rotation of the agent
        public void RotateVisitorTowardsDestination()
        {
            Vector3 targetPos = transform.position + transform.forward;
            
            if (m_animator.GetBool("Talk") && m_talkVisitor != null)
                targetPos = m_talkVisitor.transform.position;
            else if (m_animator.GetBool("Watch") && m_artifact != null)
                targetPos = m_artifact.transform.position;

            Vector3 myPos = transform.position;
            Vector3 lookDir = targetPos - myPos;
            lookDir.y = 0.0f;
            m_lookDirection = Quaternion.LookRotation(lookDir);
        }

        private void UpdateMoveSpeed()
        {
            m_animator.SetFloat(m_motionSpeedParam, 1.0f);
            Vector3 velocity = NavAgent.velocity;
            float maxSpeed = NavAgent.maxSpeed;
            // This will make the character walk/run based on the animator speed
            moveSpeed = velocity.magnitude / maxSpeed;
            m_animator.SetFloat(m_motionParam, moveSpeed * m_motionMult);
            
            // Increase priority of rvo when visitor trying to arrive at its slot.
            if (m_agentAstar.remainingDistance < 3f && moveSpeed > 0)
                m_rvoController.priority = 1f;
            else
                m_rvoController.priority = 0.1f;
        }

        // Smoothly rotate visitor towards look direction
        private void UpdateLookDirection()
        {
            if (m_animator.GetBool("Walk") == false) {
                var step = 300f * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, m_lookDirection, step);
            }
        }        
        
        // Update is called once per frame
        void Update()
        {
            UpdateMoveSpeed();
            UpdateLookDirection();
            
            switch (m_state)
            {
                case State.Appear:
                    m_alpha += Time.deltaTime * m_alphaSpeed;
                    SetOpacity(m_alpha);

                    if( m_alpha >= 1.0f )
                    {
                        SetOpacity(1.0f);
                        m_alpha = 1.0f;
                        m_state = State.Alive;
                    }
                    break;
                case State.Disappear:
                    m_alpha -= Time.deltaTime * m_alphaSpeed;
                    SetOpacity(m_alpha);

                    if( m_alpha < float.Epsilon)
                    {
                        SetOpacity(m_alpha);
                        Destroy(gameObject);
                    }
                    break;
            }
        }

        public void DeSpawn()
        {
            m_state = State.Disappear;
            m_alpha = 1.0f;
        }
    }
}
