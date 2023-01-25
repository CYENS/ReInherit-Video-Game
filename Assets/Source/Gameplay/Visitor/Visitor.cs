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

        public enum State { Appear = 0, Spectate = 1, Leave = 2, Dissapear = 3, Dead = 4 }

        [SerializeField]
        private State state;

        private Ghostify[] m_ghosties;
        private float m_alpha;
        private float m_alphaSpeed;


        [SerializeField]
        [Tooltip("The exhibit the visitor focuses on")]
        private Exhibit m_exhibit;



        public void SetExhibit( Exhibit exhibit )
        {
            m_exhibit = exhibit;
        }

        void Start()
        {
            m_ghosties = GetComponentsInChildren<Ghostify>(true);

            m_alphaSpeed = Random.Range(0.25f,1.0f);
            m_alpha = Random.Range(-m_alphaSpeed,0.0f);
            SetOpacity(m_alpha);

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

            if( m_exhibit != null )
            {
                Vector3 targetPos = m_exhibit.transform.position;
                Vector3 myPos = transform.position;
                Vector3 lookDir = targetPos - myPos;
                lookDir.y = 0.0f;

                transform.rotation = Quaternion.LookRotation(lookDir);
            }

            switch (state)
            {
                case State.Appear:

                    m_alpha += Time.deltaTime * m_alphaSpeed;
                    SetOpacity(m_alpha);

                    if( m_alpha >= 1.0f )
                    {
                        SetOpacity(1.0f);
                        m_alpha = 1.0f;
                        state = State.Spectate;
                    }

                    break;

                case State.Spectate:


                    break;

                case State.Dissapear:
                    m_alpha -= Time.deltaTime * m_alphaSpeed;
                    SetOpacity(m_alpha);

                    if( m_alpha < float.Epsilon)
                    {
                        SetOpacity(m_alpha);
                        Destroy(gameObject);
                        state = State.Dead;
                    }
                    break;
            }


        }

        public void DeSpawn()
        {
            state = State.Dissapear;
            m_alpha = 1.0f;
        }
    }
}
