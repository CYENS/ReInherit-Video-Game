using System;
using System.Collections;
using System.Collections.Generic;
using Cyens.ReInherit.Patterns;
using Cyens.ReInherit.Exhibition;
using UnityEngine;
using UnityEngine.AI;

namespace Cyens.ReInherit.Managers
{
    /// <summary>
    /// Manages and coordinates a group of workers known as Janitors.
    /// </summary>
    public class JanitorManager : CollectionManager<JanitorManager, Janitor>
    {
        [System.Serializable]
        public struct JanitorTask 
        {
            public Garbage garbage;
        }

        protected Janitor[] m_janitors;

        [Header("Janitor Tasks")]
        public Queue<JanitorTask> m_janitorTasks = new Queue<JanitorTask>();
        public HashSet<Janitor> m_janitorWorking = new HashSet<Janitor>();

        [SerializeField] [Tooltip("Number of task left for the keeper.")]
        private int m_tasksLeft = 0;
        [SerializeField] private Transform m_basePosition;

        public override void Awake()
        {
            base.Awake();
        }

        protected new void Start() 
        {
            base.Start();
            m_janitors = m_collection.Get<Janitor>();
        }

        protected void AddNewTask( ref JanitorTask task )
        {
            m_janitorTasks.Enqueue(task);
            m_tasksLeft += 1;
        }
        
        public void AddCleanTask( Garbage garbage )
        {
            var task = new JanitorTask();
            task.garbage = garbage;
            AddNewTask( ref task );
        }

        public bool IsNewTaskAvailable()
        {
            if (m_janitorTasks.Count > 0)
                return true;
            return false;
        }
        
        public JanitorTask GetNextTask( Janitor worker )
        {
            m_tasksLeft -= 1;
            if (m_janitorTasks.Count > 0)
            {
                m_janitorWorking.Add(worker);
                return m_janitorTasks.Dequeue();
            }

            return new JanitorTask();
        }

        public void DoneWorking( Janitor worker )
        {
            m_janitorWorking.Remove(worker);
        }

        public bool AllTasksFinished()
        {
            if (IsNewTaskAvailable()) return false;
            if (m_janitorWorking.Count > 0) return false;
            return true;
        }

        public Vector3 GetBasePosition()
        {
            return m_basePosition.position;
        }
        
        public Janitor GetIdleJanitor()
        {
            foreach( var janitor in m_janitors )
            {
                if( janitor.isIdle || janitor.returningToBase)
                {
                    return janitor;
                }
            }
            return null;
        }

        protected new void Update()
        {
            base.Update();
            Janitor janitor;
            
            // No new tasks
            if( IsNewTaskAvailable() == false )
            {
                // Check if 
                janitor = GetIdleJanitor();
                if( janitor == null )
                {
                    return;
                }
                if(janitor.InBase() == false && janitor.returningToBase == false)
                    janitor.AddReturnTask();
                return;
            }

            // Get a janitor that isn't currently busy
            janitor = GetIdleJanitor();
            if( janitor == null )
            {
                return;
            }

            // Assign tasks to non-busy workers
            Debug.Log("Assigning new task to janitor " + janitor);
            janitor.gameObject.SetActive(true);
            var task = GetNextTask( janitor );
            janitor.AddCleanTask(task.garbage);
        }
    }
}