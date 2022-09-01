using System;
using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cyens.ReInherit
{
    public class Preprocessing : MonoBehaviour
    {
        [SerializeField] private Transform m_janitorParent;
        private Vector3[] m_janitorMovePoints;
        [SerializeField] private bool m_showClustered = true;
        private int m_clustersCount;
        private KMeansResults result;
        private int iterations = 200;


        // Start is called before the first frame update
        private void Awake()
        {
            m_janitorMovePoints = new Vector3[m_janitorParent.GetChild(0).childCount];
            int i = 0;
            foreach (Transform p in m_janitorParent.GetChild(0).transform) {
                m_janitorMovePoints[i] = p.position;
                i += 1;
            }

            m_clustersCount = m_janitorParent.childCount - 1;
            ClusterPoints();
            AssignPointsToJanitors();
        }

        private void ClusterPoints()
        {
            result = KMeans.Cluster(m_janitorMovePoints, m_clustersCount, iterations, 0);
        }

        private Vector3 CalculateJanitorStartingPosition(List<Vector3> points)
        {
            float totalX = 0f;
            float totalY = 0f;
            float totalZ = 0f;
            foreach(Vector3 p in points)
            {
                totalX += p.x;
                totalY += p.y;
                totalZ += p.z;
            }
            float centerX = totalX / points.Count;
            float centerY = totalY / points.Count;
            float centerZ = totalZ / points.Count;

            Vector3 center = new Vector3(centerX, centerY, centerZ);
            
            float dis = Single.PositiveInfinity;
            int index = 0;
            for (int i = 0; i < points.Count; i++) {
                float tempDis = Vector3.Distance(points[i], center);
                if (tempDis < dis) {
                    dis = tempDis;
                    index = i;
                }
            }
            print(points[index]);
            return points[index];
        }

        private void AssignPointsToJanitors()
        {
            for (int i = 0; i < result.clusters.Length; i++) {
                Transform janitor = m_janitorParent.GetChild(i + 1);
                List<Vector3> janitorPoints = new List<Vector3>(result.clusters[i].Length);
                for (int j = 0; j < result.clusters[i].Length; j++) {
                    janitorPoints.Add(m_janitorMovePoints[result.clusters[i][j]]);
                }
                
                janitor.GetComponent<Janitor>().SetMovingPoints(janitorPoints);
                janitor.position = CalculateJanitorStartingPosition(janitorPoints);
            }
        }

        void OnDrawGizmos()
        {
            if (result == null)
                return;

            if (m_showClustered) {
                Gizmos.color = Color.yellow;
                for (int i = 0; i < result.clusters.Length; i++) {
                    Color color = m_janitorParent.GetChild(i + 1).transform.Find("blank_body_Sphere")
                        .GetComponent<Renderer>().material.color;
                    Gizmos.color = color;
                    for (int j = 0; j < result.clusters[i].Length; j++) {
                        Gizmos.DrawSphere(m_janitorMovePoints[result.clusters[i][j]], 0.5f);
                    }
                    DrawConnectedLines(i);
                }
            }
        }

        private void DrawConnectedLines(int index)
        {
            Color color = m_janitorParent.GetChild(index + 1).transform.Find("blank_body_Sphere")
                .GetComponent<Renderer>().material.color;
            Gizmos.color = color;
            color.a = 1f;
            for (int j = 0; j < result.clusters[index].Length; j++) {
                for (int k = 0; k < result.clusters[index].Length; k++) {
                    Gizmos.DrawLine(m_janitorMovePoints[result.clusters[index][j]], m_janitorMovePoints[result.clusters[index][k]]);
                }
            }
        }
    }
}
