using System;
using System.Collections;
using System.Collections.Generic;
using Cyens.ReInherit.Patterns;
using UnityEngine;

namespace Cyens.ReInherit.Managers
{
    public class KeeperManager : Singleton<KeeperManager>
    {


        public Queue<Keeper.Task> m_keeperTasks = new Queue<Keeper.Task>();
        public HashSet<Keeper> m_keeperWorking = new HashSet<Keeper>();

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
        
        public void AddNewTask(Artifact target, Keeper.Goal goal )
        {
            var task = new Keeper.Task();
            task.target = target;
            task.goal = goal;

            m_keeperTasks.Enqueue(task);
            m_tasksLeft += 1;
        }

        public void AddPlaceTask(Artifact target) => AddNewTask(target, Keeper.Goal.PlaceExhibit);
        public void AddUpgradeTask(Artifact target) => AddNewTask(target, Keeper.Goal.UpgradeExhibit);


        public bool IsNewTaskAvailable()
        {
            if (m_keeperTasks.Count > 0)
                return true;
            return false;
        }
        
        public Keeper.Task GetNextTask( Keeper worker )
        {
            m_tasksLeft -= 1;
            if (m_keeperTasks.Count > 0)
            {
                m_keeperWorking.Add(worker);
                return m_keeperTasks.Dequeue();
            }

            return null;
        }

        public void DoneWorking( Keeper worker )
        {
            m_keeperWorking.Remove(worker);
        }

        public bool AllTasksFinished()
        {
            if (IsNewTaskAvailable()) return false;
            if (m_keeperWorking.Count > 0) return false;
            return true;
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
                    //if (m_keeperPoints.Contains(child.position) == false) {
                    //    AddNewTask(child.position);
                    //    Destroy(child.gameObject);
                    //}

                }
            }
        }

        private void Start()
        {
        }
        
        void OnDrawGizmos()
        {
            foreach (var task in m_keeperTasks) {
                Gizmos.color = new Color(1, 0, 0, 1f);
                Gizmos.DrawCube(task.position, new Vector3(0.5f, 0.2f, 0.5f));
            }
        }
    }
}