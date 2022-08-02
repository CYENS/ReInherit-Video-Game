using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Cyens.ReInherit
{
    public class CollectGarbageBehavior : MonoBehaviour
    {
        private Vector3 m_TargetPosition;
        private NavMeshAgent m_NavMeshAgent;

        private void OnEnable()
        {
            m_NavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
            GetNewDestination();
        }

        private void GetNewDestination()
        {
            m_TargetPosition = new Vector3(UnityEngine.Random.Range(0f, 24f), 0f, UnityEngine.Random.Range(0f, 24f));
            while (!m_NavMeshAgent.SetDestination(m_TargetPosition)) {
                m_TargetPosition = new Vector3(UnityEngine.Random.Range(0f, 24f), 0f, UnityEngine.Random.Range(0f, 24f));
            }
        }

        private bool CheckIfArrived()
        {
            if (!m_NavMeshAgent.pathPending) {
                if (m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance) {
                    if (!m_NavMeshAgent.hasPath || m_NavMeshAgent.velocity.sqrMagnitude == 0f) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
