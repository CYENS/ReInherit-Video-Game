using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    [ExecuteInEditMode]
    public class RoomExhibit : MonoBehaviour
    {
        [SerializeField] private List<Vector3> m_points;
        [SerializeField] private float m_radius = 1.5f;
        [SerializeField] private float m_interPointsDistance = 1f;
        
        public List<Vector3> GetPoints()
        {
            return m_points;
        }
        
        // Start is called before the first frame update
        void Start()
        {
            m_points = new List<Vector3>();
            CreatePoints();
        }

        private void CreatePoints()
        {
            if(m_radius <= 0 || m_interPointsDistance <= 0)
                return;
            
            m_points.Clear();

            float xDistance = (transform.localScale.x + m_radius);
            float zDistance = (transform.localScale.z + m_radius);

            float xMin = -1f * (xDistance / 2f);
            float zMin = -1f * (zDistance / 2f);

            float x = xMin;
            float z = zMin;
            while(x <= -xMin){
                z = zMin;
                while(z <= -zMin){
                    m_points.Add(transform.TransformPoint(new Vector3(x / transform.localScale.x, 0, z / transform.localScale.z)));
                    z += m_interPointsDistance;
                }
                x += m_interPointsDistance;
            }
        }

        private void Update()
        {
            if (!Application.isPlaying) {
                CreatePoints();
            }
        }

        private void OnValidate()
        {
            CreatePoints();
        }

        void OnDrawGizmosSelected()
        {
            // Draw a semitransparent red cube at the transforms position
            Gizmos.color = new Color(1, 0, 0, 1f);
            foreach (var point in m_points) {
                Gizmos.DrawCube(point, new Vector3(0.2f, 0.1f, 0.2f));
            }
        }
    }
}
