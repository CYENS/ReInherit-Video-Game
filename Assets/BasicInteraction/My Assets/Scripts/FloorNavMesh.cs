using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Cyens.ReInherit
{
    public class FloorNavMesh : MonoBehaviour
    {
        private NavMeshSurface m_navMesh;
        
        // Start is called before the first frame update
        void Start()
        {
            m_navMesh = gameObject.GetComponent<NavMeshSurface>();
        }

        public void RebakeNavmesh()
        {
            m_navMesh.BuildNavMesh();
        }
    }
}
