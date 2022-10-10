using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;
using Pathfinding;

namespace Cyens.ReInherit
{
    public class EntranceAgent : MonoBehaviour
    {
        private IAstarAI m_astar;
        
        // Start is called before the first frame update
        void Start()
        {
            m_astar = gameObject.GetComponent<IAstarAI>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            //If is not first in row, stop moving
            if (other.gameObject.CompareTag("CrowdCharacter") && m_astar.canMove == true) {
                if(other.GetComponent<BehaviourTreeRunner>().tree.blackboard.waitingForTicket ||
                   other.GetComponent<IAstarAI>().canMove == false)
                    m_astar.canMove = false;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            //If front character moved, move to its place in queue
            if (other.gameObject.CompareTag("CrowdCharacter") && m_astar.canMove == false) {
                StartCoroutine(EnableMovement(UnityEngine.Random.Range(0.5f, 1f)));
            }
        }
        
        IEnumerator EnableMovement(float time)
        {
            yield return new WaitForSeconds(time);
            m_astar.canMove = true;
        }
    }
}
