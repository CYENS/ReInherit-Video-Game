using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace Cyens.ReInherit
{
    public class BaseUnit : MonoBehaviour
    {
        private AIPath m_aiPath;

        public GameObject target;
        
        
        protected void Awake()
        {
            m_aiPath = GetComponent<AIPath>();

            //m_aiPath.destination = target.transform.position;
        }

        public void SetTarget(GameObject newTarget)
        {
            WayPoint wayPoint = newTarget.GetComponent<WayPoint>();
            if (wayPoint != null) {

                if (target != null) {
                    Destroy(target);
                }
                
                target = newTarget;
                m_aiPath.destination = wayPoint.transform.position;
                wayPoint.owner = gameObject;
            }
        }
        
        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
