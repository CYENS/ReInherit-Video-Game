using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class ArtifactVisitorHandler : MonoBehaviour
    {
        private int m_numberOfViewPoints = 6;
        public List<Vector3> m_viewPoints;
        public List<bool> m_viewPointReserved;

        // Return a free spot around artifact. Y value represent spot ID;
        public Vector3 GetFreeViewSpot()
        {
            for (int i = 0; i < m_viewPoints.Count; i++) {
                if (m_viewPointReserved[i] == false) {
                    m_viewPointReserved[i] = true;
                    Vector3 ret = m_viewPoints[i];
                    ret.y = i;
                    return ret;
                }
            }
            return Vector3.zero;
        }

        public void UnuseViewSpot(int id)
        {
            m_viewPointReserved[id] = false;
        }
        
        // Start is called before the first frame update
        void Awake()
        {
            m_viewPoints = new List<Vector3>(m_numberOfViewPoints);
            m_viewPointReserved = new List<bool>(m_numberOfViewPoints);
            GenerateViewPoints();
        }

        private void GenerateViewPoints()
        {
            // Only Ground Layer
            int layerMask = 1 << 10;
            
            float angleStep = 360.0f / (float)m_numberOfViewPoints;
            float angle = 0f;
            for ( int i = 0; i < m_numberOfViewPoints; i++ )
            {
                Vector3 center = transform.position;
                Vector3 offset = Vector3.forward * 2f;
                offset = Quaternion.Euler(Vector3.up * angle) * offset;

                Vector3 pos = center + offset;
                
                // Check if spot is not blocked or outside the scene
                Vector3 rayPos = center + (offset * 1.5f);
                RaycastHit hit;
                if (Physics.Raycast(rayPos - new Vector3(0f, 5f, 0f), transform.TransformDirection(Vector3.up), out hit, 10f, layerMask)) {
                    m_viewPoints.Add(pos);
                    m_viewPointReserved.Add(false);
                }

                angle += angleStep;
            }
        }

        void OnDrawGizmosSelected()
        {
            if (Application.isPlaying) {
                for (int i = 0; i < m_viewPoints.Count; i++) {
                    Gizmos.color = new Color(0, 0, 0, 1f);
                    Gizmos.DrawCube(m_viewPoints[i], new Vector3(0.5f, 0.2f, 0.5f));
                }
            }
        }
    }
}
