using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pathfinding;

namespace Cyens.ReInherit
{
    [ExecuteInEditMode]
    public class RoomExhibit : MonoBehaviour
    {
        [SerializeField] private List<Vector3> m_points;
        private List<bool> m_pointsAvailability;

        //Get a random available point index around exhibit
        public int GetAvailablePointIndex()
        {
            List<int> temp = new List<int>();
            for (int i = 0; i < m_pointsAvailability.Count; i++) {
                if (m_pointsAvailability[i]) {
                    temp.Add(i);
                }
            }

            if (temp.Count == 0)
                return -1;

            int rd = UnityEngine.Random.Range(0, temp.Count);
            return temp[rd];
        }

        //Get point based on index and reserve point
        public Vector3 GetIndexPoint(int index)
        {
            m_pointsAvailability[index] = false;
            return m_points[index];
        }
        
        //Release point after agent leave
        public void ReleasePoint(int index)
        {
            m_pointsAvailability[index] = true;
        }
        
        // Start is called before the first frame update
        void Start()
        {
            m_points = new List<Vector3>();
            m_pointsAvailability = new List<bool>();
            CreatePoints();
            RemovePointsNotOnNavmesh();
        }

        private void Update()
        {
            if (!Application.isPlaying) {
                CreatePoints();
            }
        }

        //Find exhibit points
        private void CreatePoints()
        {
            m_points.Clear();
            m_pointsAvailability.Clear();
            foreach (Transform point in transform) {
                m_points.Add(point.position);
                m_pointsAvailability.Add(true);
            }
            m_points = m_points.OrderBy(
                x => Vector3.Distance(transform.position,x)
            ).ToList();
        }

        //Remove points that are not on navmesh.
        private void RemovePointsNotOnNavmesh()
        {
            var graph = AstarPath.active.data.recastGraph;
            if (graph != null) {
                for (int i = 0; i < m_points.Count; i++) {
                    if (graph.PointOnNavmesh(m_points[i], null) == null) {
                        m_pointsAvailability[i] = false;
                    }
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            // Draw a semitransparent red cube at the transforms position
            for (int i = 0; i < m_points.Count; i++) {
                if (m_pointsAvailability[i]) {
                    Gizmos.color = new Color(0, 0, 1, 1f);
                    Gizmos.DrawCube(m_points[i], new Vector3(0.2f, 0.1f, 0.2f));
                }
                else {
                    Gizmos.color = new Color(1, 0, 0, 1f);
                    Gizmos.DrawCube(m_points[i], new Vector3(0.2f, 0.1f, 0.2f));
                }
            }
        }
    }
}
