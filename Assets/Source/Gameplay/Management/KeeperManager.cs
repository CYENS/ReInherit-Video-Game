using System;
using System.Collections;
using System.Collections.Generic;
using Cyens.ReInherit.Patterns;
using UnityEngine;

namespace Cyens.ReInherit.Gameplay.Management
{
    public class KeeperManager : Singleton<KeeperManager>
    {
        public Queue<Vector3> m_keeperPoints = new Queue<Vector3>();
        [SerializeField] [Tooltip("Number of task left for the keeper.")]
        private int m_tasksLeft = 0;
        [SerializeField] private Transform m_basePosition;
        [SerializeField] private Transform m_ExhibitTestPoints;
        [SerializeField] [Tooltip("Seconds to wait for placing exhibit.")]
        private float m_placingDelay;
        [SerializeField] [Tooltip("Seconds to wait/idling until next task.")]
        private float m_idleDelay;

        public float GetPlacingDelay()
        {
            return m_placingDelay;
        }

        public float GetIdleDelay()
        {
            return m_idleDelay;
        }
        
        public void AddNewTask(Vector3 point)
        {
            m_keeperPoints.Enqueue(point);
            m_tasksLeft += 1;
        }

        public bool IsNewTaskAvailable()
        {
            if (m_keeperPoints.Count > 0)
                return true;
            return false;
        }
        
        public Vector3 GetNextTask()
        {
            m_tasksLeft -= 1;
            if(m_keeperPoints.Count > 0)
                return m_keeperPoints.Dequeue();
            return Vector3.zero;
        }

        public Vector3 GetBasePosition()
        {
            return m_basePosition.position;
        }

        public override void Awake()
        {
            base.Awake();
        }

        private void Update()
        {
            //--TESTING, add active points from hierarchy to the task queue
            foreach (Transform child in m_ExhibitTestPoints) {
                if (child.gameObject.activeInHierarchy){
                    if (m_keeperPoints.Contains(child.position) == false) {
                        AddNewTask(child.position);
                        Destroy(child.gameObject);
                    }
                }
            }
        }

        private void Start()
        {
        }
        
        void OnDrawGizmos()
        {
            foreach (var point in m_keeperPoints) {
                Gizmos.color = new Color(1, 0, 0, 1f);
                Gizmos.DrawCube(point, new Vector3(0.5f, 0.2f, 0.5f));
            }
        }
    }
}