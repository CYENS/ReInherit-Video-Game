using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Cyens.ReInherit.Patterns;

namespace Cyens.ReInherit
{
    public class NavMeshManager : Singleton<NavMeshManager>
    {
        
        private NavMeshSurface m_surface;
        
        [SerializeField]
        private float m_refreshRate = 0.5f;

        private float m_timer = 0.0f;


        // Start is called before the first frame update
        void Start()
        {
            m_surface = GetComponent<NavMeshSurface>();
        }

        public static void Bake() => Instance.Refresh();
        private void Refresh() 
        {
            m_timer = m_refreshRate;
        }

        private void Update() 
        {
            if( m_timer < float.Epsilon )
            {
                return;
            }

            m_timer -= Time.deltaTime;
            if( m_timer < float.Epsilon )
            {
                m_timer = 0.0f;
                Instance.m_surface.BuildNavMesh();
            }
        }
    }
}
