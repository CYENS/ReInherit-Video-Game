using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Cyens.ReInherit
{
    public class CollectGarbageBehavior : MonoBehaviour
    {
        [SerializeField] private GameObject m_nextTarget;
        private NavMeshAgent m_NavMeshAgent;
        private GarbageManager m_garbageManager;
        private bool m_started = false;

        // For testing purposes only, we will use on enable with a behavior tree later
        private void Start()
        {
            StartCoroutine(ExampleCoroutine());
        }
        IEnumerator ExampleCoroutine()
        {
            yield return new WaitForSeconds(2);
            m_garbageManager = GameObject.Find("GarbageParent").GetComponent<GarbageManager>();
            m_NavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
            GetNextDestination();
            m_started = true;
        }

        /*private void OnEnable()
        {
            m_garbageManager = GameObject.Find("GarbageParent").GetComponent<GarbageManager>();
            m_NavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
            GetNextDestination();
        }*/

        private void GetNextDestination()
        {
            if (m_garbageManager.GetGarbage.Count == 0)
                return;
            m_nextTarget = m_garbageManager.GetGarbage[0].gameObject;
            m_NavMeshAgent.SetDestination(m_nextTarget.transform.position);
        }

        private void Update()
        {
            if(m_started)
                if (m_nextTarget == null || !m_nextTarget.activeInHierarchy)
                    GetNextDestination();
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
