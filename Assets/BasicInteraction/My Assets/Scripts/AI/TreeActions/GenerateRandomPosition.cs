using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TheKiwiCoder;
using Random = UnityEngine.Random;
using UnityEngine.AI;

namespace Cyens.ReInherit.Test.Example
{
    public class GenerateRandomPosition : ActionNode
    {
        public Vector2 min = Vector2.one * -1;
        public Vector2 max = Vector2.one * 1;
        // A value in range [0,1], higher values mean higher chance to assign a POI as destination 
        public float m_poiWeight = 0f;
        public bool m_randomPoiWeight = false;
        private Transform m_PoiParent;
        private Transform m_randomPointsParent;

        protected override void OnStart()
        {
            m_PoiParent = GameObject.Find("POIs").transform;
            m_randomPointsParent = GameObject.Find("RandomNavmeshPoints").transform;
            if (m_randomPoiWeight) m_poiWeight = UnityEngine.Random.Range(0f, 1.0f);
        }

        protected override void OnStop() {
        }

        // Get the number of crowd agents around a POI position
        private float GetDensityInRadius(Vector3 poiPos, GameObject[] crowdAgents, float radius)
        {
            int agentsInRadius = GameObject.FindGameObjectsWithTag("CrowdCharacter")
                .Select(go => go.transform)
                .Where(agent => Vector3.Distance(agent.position, poiPos) < radius)
                .ToArray().Length;
            return agentsInRadius;
        }

        // Calculate best POI based on distance and density
        private int CalculateBestPoint(List<float> distance, List<float> density)
        {
            float distanceWeight = 0.3f;
            float densityWeight = 0.7f;
            int index = 0;
            float bestVal = Single.PositiveInfinity;
            float tempVal = 0;
            
            for (int i = 0; i < distance.Count; i++) {
                float tempDis;
                if(distance[i] <= 3f)
                    tempDis = Single.PositiveInfinity;
                else
                    tempDis = distance[i];
                tempVal = (distanceWeight * tempDis) + (densityWeight * density[i]);
                if (tempVal < bestVal) {
                    bestVal = tempVal;
                    index = i;
                }
            }

            return index;
        }

        // Generate a random point around a POI position
        private Vector3 GenerateRandomPointAroundPoint(Vector3 center)
        {
            float angle = UnityEngine.Random.Range(0f, 1f) * 360f;
            bool isOnNavMesh = false;
            float startingDis = 1f;
            Vector3 testPos;
            do {
                NavMeshHit hit;
                
                float x = startingDis * Mathf.Cos(angle * Mathf.Deg2Rad);
                float z = startingDis * Mathf.Sin(angle * Mathf.Deg2Rad);
                testPos.x = center.x;
                testPos.y = center.y;
                testPos.z = center.z;
                testPos.x += x;
                testPos.z += z;
                isOnNavMesh = NavMesh.SamplePosition(testPos, out hit, 30f, NavMesh.AllAreas);
                startingDis += 1;
            } while (isOnNavMesh == false);

            return testPos;
        }

        private Vector3 GetRandomPointOnNavmesh()
        {
            int rdIndex = UnityEngine.Random.Range(0, m_randomPointsParent.childCount);
            Vector3 rdPoint = m_randomPointsParent.GetChild(rdIndex).position;

            return GenerateRandomPointAroundPoint(rdPoint);
        }
        
        // Get the next position of the agent. Is either a completely random position, or
        // a POI position
        private Vector3 GetNextPoint()
        {
            if (m_poiWeight == 0)
                return GetRandomPointOnNavmesh();

            float rd = UnityEngine.Random.Range(0f, 1f);
            if (rd <= m_poiWeight) {
                //GET POIs and select one of them as destination based on density around it and distance
                List<float> density = new List<float>(m_PoiParent.childCount);
                List<float> distance = new List<float>(m_PoiParent.childCount);
                GameObject[] crowdAgents = GameObject.FindGameObjectsWithTag("CrowdCharacter");

                foreach (Transform c in m_PoiParent) {
                    distance.Add(Vector3.Distance(c.position, context.transform.position));
                    density.Add(GetDensityInRadius(c.position, crowdAgents, 5f));
                }

                int poiIndex = CalculateBestPoint(distance, density);
                Vector2 retPoint = GenerateRandomPointAroundPoint(m_PoiParent.GetChild(poiIndex).transform.position);
                return retPoint;
            }

            return GetRandomPointOnNavmesh();
        }
        
        protected override State OnUpdate()
        {
            Vector3 nextPoint = GetNextPoint();
            blackboard.moveToPosition.x = nextPoint.x;
            blackboard.moveToPosition.y = nextPoint.y;
            blackboard.moveToPosition.z = nextPoint.z;
            return State.Success;
        }
    }
}