using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using Cyens.ReInherit.Managers;

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
        public enum Emotion { Happy = 0, Neutral = 1, Angry = 2, Disgusted = 3, Bored = 4};

        [SerializeField] private int m_id;
        [SerializeField] private bool m_inSpawnArtifact;
        private NavMeshAgent m_navAgent;
        private Animator m_animator;
        private VisitorChat m_chat;
        
        // Ghosting effect
        private Ghostify[] m_ghosties;
        private float m_alpha;
        private float m_alphaSpeed;
        
        [SerializeField] private State m_state;
        [SerializeField] private Behavior m_behavior;
        [SerializeField] private Emotion m_emotion;
        
        [Tooltip("The exhibit the visitor focuses on")]
        [SerializeField] private ExhibitVisitorHandler m_exhibit;
        private int m_viewSlotID;
        [SerializeField] private Visitor m_talkVisitor;
        [Tooltip("How impressed the visitor was from the last visit")]
        [SerializeField] [Range(0.0f, 1.0f)] private float m_impression;
        [SerializeField] private float m_boredom;
        [SerializeField] private float m_lookDuration;
        [SerializeField] private float m_talkDuration;
        public List<ExhibitVisitorHandler> visitedExhibits;
        
        [Header("Move Animator Parameters")]
        [SerializeField] private string m_motionSpeedParam = "MotionSpeed";
        [SerializeField] private string m_motionParam = "Speed";
        [Tooltip("An adjustment value in case movement speed parameter isn't a value from 0 to 1")]
        [SerializeField] private float m_motionMult = 3.0f;
        private Quaternion m_lookDirection;

        public int ID { get => m_id; set => m_id = value; }
        public bool InSpawnArtifact { get => m_inSpawnArtifact; set => m_inSpawnArtifact = value; }
        public NavMeshAgent NavAgent { get => m_navAgent; }
        public Animator Animator { get => m_animator; }
        public float Impression { get => m_impression; set => m_impression = value; }
        public State VisitorState { get => m_state; set => m_state = value; }
        public Behavior VisitorBehavior { get => m_behavior; set => m_behavior = value; }
        public Emotion VisitorEmotion { get => m_emotion; set => m_emotion = value; }
        public float LookDuration { get => m_lookDuration; }
        public float TalkDuration { get => m_talkDuration; }
        public Visitor TalkVisitor { get => m_talkVisitor; set => m_talkVisitor = value; }
        public float Boredome { get => m_boredom; set => m_boredom = value; }
        public VisitorChat ChatBubble { get => m_chat; }

        public void SetExhibit(ExhibitVisitorHandler exhibit, int viewSlotID)
        {
            m_exhibit = exhibit;
            m_viewSlotID = viewSlotID;
        }

        // Free up the reserved slot around the artifact
        public void ReleaseArtifactSlot()
        {
            if (m_exhibit != null) {
                m_exhibit.UnuseViewSpot(m_viewSlotID);
                m_exhibit = null;
                m_viewSlotID = -1;
            }
        }

        public void GiveCoins(int numberOfCoins)
        {
            CoinManager.Instance.AddCoins(transform.position, numberOfCoins);
            GameManager.Funds += numberOfCoins;
        }

        private void Awake()
        {
            m_animator = GetComponent<Animator>();
            m_navAgent = GetComponent<NavMeshAgent>();
            visitedExhibits = new List<ExhibitVisitorHandler>();
            m_ghosties = GetComponentsInChildren<Ghostify>(true);
            m_lookDuration = UnityEngine.Random.Range(7.5f, 12f);
            m_talkDuration = 10f;
            m_chat = transform.Find("VisitorsChat").GetChild(0).GetComponent<VisitorChat>();
        }

        void Start()
        {
            m_alphaSpeed = Random.Range(0.25f,0.5f);
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
            else if (m_animator.GetBool("Watch") && m_exhibit != null)
                targetPos = m_exhibit.transform.position;

            Vector3 myPos = transform.position;
            Vector3 lookDir = targetPos - myPos;
            lookDir.y = 0.0f;
            m_lookDirection = Quaternion.LookRotation(lookDir);
        }

        private void UpdateMoveSpeed()
        {
            m_animator.SetFloat(m_motionSpeedParam, 1.0f);
            Vector3 velocity = NavAgent.velocity;
            float maxSpeed = NavAgent.speed;
            // This will make the character walk/run based on the animator speed
            moveSpeed = velocity.magnitude / maxSpeed;
            m_animator.SetFloat(m_motionParam, moveSpeed * m_motionMult);

            // Increase priority of rvo when visitor trying to arrive at its slot.
            if (m_navAgent.remainingDistance < 3f && moveSpeed > 0)
                m_navAgent.avoidancePriority = 1;
            else
                m_navAgent.avoidancePriority= 50;
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

        private void OnDrawGizmosSelected()
        {
            if(m_navAgent != null)
                Debug.DrawLine(transform.position, m_navAgent.destination, Color.red);
        }
    }
}