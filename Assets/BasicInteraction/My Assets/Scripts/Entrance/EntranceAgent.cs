using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.VisualScripting;

namespace Cyens.ReInherit
{
    public class EntranceAgent : MonoBehaviour
    {
        private IAstarAI m_astar;
        private AIBase m_ai;
        private float m_currentTime;
        private float m_moveTime;
        private bool m_blocked = false;
        
        // Start is called before the first frame update
        void Start()
        {
            m_astar = gameObject.GetComponent<IAstarAI>();
            m_ai = gameObject.GetComponent<AIBase>();
        }

        private void FixedUpdate()
        {
            if (m_blocked && Time.time >= m_moveTime) {
                m_ai.canMove = true;
                m_blocked = false;
            }
        }

        private void Update()
        {
            if(m_astar.remainingDistance <= 1f)
                Destroy(this.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            //If arrive at the ticket shop, stop for some seconds
            if (other.gameObject.CompareTag("TicketCollider")) {
                m_astar.canMove = false;
                m_blocked = true;
                m_moveTime = Time.time + UnityEngine.Random.Range(5f, 15f);
            }

            //If is not first in row, stop moving
            if (other.gameObject.CompareTag("CrowdCharacter") && m_astar.canMove == true) {
                if (other.GetComponent<IAstarAI>().canMove == false) {
                    m_astar.canMove = false;
                }
            }
        }
        
        private void OnTriggerStay(Collider other)
        {
            //Stop repath if agent already entered a queue
            if (other.gameObject.CompareTag("EntranceCollider"))
                m_ai.autoRepath.mode = AutoRepathPolicy.Mode.Never;
            if (other.gameObject.CompareTag("CrowdCharacter") && m_astar.canMove == true) {
                if (other.GetComponent<IAstarAI>().canMove == false && m_astar.velocity.magnitude <= 1.5f) {
                    m_astar.canMove = false;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            //If front character moved, move to its place in queue
            if (other.gameObject.CompareTag("CrowdCharacter") && m_astar.canMove == false) {
                m_astar.canMove = true;
            }
            //If exit entrance area, continue auto repath
            if (other.gameObject.CompareTag("EntranceCollider"))
                m_ai.autoRepath.mode = AutoRepathPolicy.Mode.Dynamic;
        }
    }
}
