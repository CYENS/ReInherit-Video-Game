using System;
using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;
using Pathfinding;
using UnityEngine.ProBuilder.MeshOperations;

namespace Cyens.ReInherit
{
    public class EntranceAgent : MonoBehaviour
    {
        private IAstarAI m_astar;
        private BehaviourTreeRunner m_treeRunner;
        
        // Start is called before the first frame update
        void Start()
        {
            m_astar = gameObject.GetComponent<IAstarAI>();
            m_treeRunner = gameObject.GetComponent<BehaviourTreeRunner>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            //If is not first in row, stop moving
            if (other.gameObject.CompareTag("CrowdCharacter") && m_astar.canMove == true) {
                if(other.GetComponent<BehaviourTreeRunner>().tree.blackboard.waitingForTicket ||
                   other.GetComponent<BehaviourTreeRunner>().tree.blackboard.inTicketRow)
                    m_astar.canMove = false;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Entrance"))
                m_treeRunner.tree.blackboard.inTicketRow = true;
        }

        private void OnTriggerExit(Collider other)
        {
            //If front character moved, move to its place in queue
            if (other.gameObject.CompareTag("CrowdCharacter") && m_astar.canMove == false) {
                StartCoroutine(EnableMovement(UnityEngine.Random.Range(0.75f, 1f)));
            }
            
            if (other.CompareTag("Entrance"))
                m_treeRunner.tree.blackboard.inTicketRow = false;
        }
        
        IEnumerator EnableMovement(float time)
        {
            yield return new WaitForSeconds(time);
            m_astar.canMove = true;
        }
    }
}
