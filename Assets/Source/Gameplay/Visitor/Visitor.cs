using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    /// <summary>
    /// Controls the visitor that looks at exhibits and rates your museum
    /// </summary>
    public class Visitor : MonoBehaviour
    {
        public enum State { Appear = 0, Alive = 1, Disappear = 2}
        public enum Behavior { Walking = 0, Looking = 1, Talking = 2, Exiting = 3 };
        public enum Emotion { Excited = 0, Bored = 1, };

        private Ghostify[] m_ghosties;
        private float m_alpha;
        private float m_alphaSpeed;
        
        [SerializeField] private State m_state;
        [SerializeField] private Behavior m_behavior;
        [SerializeField] private Emotion m_emotion;
        [Tooltip("The exhibit the visitor focuses on")]
        [SerializeField] private Vector3 m_exhibitPosition;
        [Tooltip("How impressed the visitor was from the last visit")]
        [SerializeField] [Range(0.0f, 1.0f)] private float m_impression;
        [SerializeField] private float m_boredom;
        [SerializeField] private float m_lookDuration;
        [SerializeField] private float m_talkDuration;
        [SerializeField] private Visitor m_talkTarget;
        
        public float Impression { get => m_impression; }

        public void SetExhibit( Exhibit exhibit ) { m_exhibitPosition = exhibit.transform.position; }

        public void SetExhibit(Vector3 exhibitPos) { m_exhibitPosition = exhibitPos; }

        void Start()
        {
            m_ghosties = GetComponentsInChildren<Ghostify>(true);
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

        // Update is called once per frame
        void Update()
        {
            /*if( m_exhibitPosition != Vector3.zero )
            {
                Vector3 targetPos = m_exhibitPosition;
                Vector3 myPos = transform.position;
                Vector3 lookDir = targetPos - myPos;
                lookDir.y = 0.0f;
                transform.rotation = Quaternion.LookRotation(lookDir);
            }*/

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
