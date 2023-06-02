using System;
using System.Collections;
using System.Collections.Generic;
using Cyens.ReInherit.Patterns;
using Cyens.ReInherit.Exhibition;
using UnityEngine;


namespace Cyens.ReInherit.Managers
{
    /// <summary>
    /// Manages and coordinates a group of workers known as keepers.
    /// It assigns them various tasks concerning the exhibits.
    /// Such as storage, placement, replacement, movement, etc.
    /// </summary>
    public class KeeperManager : CollectionManager<KeeperManager, Keeper>
    {

        [System.Serializable]
        public struct KeeperTask 
        {
            public enum Type 
            {
                Place = 0,
                Upgrade = 1,
                Remove = 2,
                Move = 3
            }
            public Type type;
            public Exhibit exhibit;
            public Vector3 point;
        }

        protected Keeper[] m_keepers;


        [Header("Keeper Tasks")]

        public Queue<KeeperTask> m_keeperTasks = new Queue<KeeperTask>();
        public HashSet<Keeper> m_keeperWorking = new HashSet<Keeper>();

        [SerializeField] [Tooltip("Number of task left for the keeper.")]
        private int m_tasksLeft = 0;
        [SerializeField] private Transform m_basePosition;
        [SerializeField] private Transform m_ExhibitTestPoints;
        [SerializeField] 
        [Tooltip("Seconds to wait for placing exhibit.")]
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
        


        public override void Awake()
        {
            base.Awake();
        }

        protected new void Start() 
        {
            base.Start();
            m_keepers = m_collection.Get<Keeper>();
        }


        protected void AddNewTask( ref KeeperTask task )
        {
            m_keeperTasks.Enqueue(task);
            m_tasksLeft += 1;
        }

        public void AddPlaceTask( Exhibit target )
        {
            var task = new KeeperTask();
            task.type = KeeperTask.Type.Place;
            task.exhibit = target;
            AddNewTask( ref task );
        }

        public void AddUpgradeTask( Exhibit target )
        {
            var task = new KeeperTask();
            task.type = KeeperTask.Type.Upgrade;
            task.exhibit = target;
            AddNewTask( ref task );
        }

        public void AddRemoveTask( Exhibit target )
        {
            var task = new KeeperTask();
            task.type = KeeperTask.Type.Remove;
            task.exhibit = target;
            AddNewTask( ref task );
        }

        public void AddMoveTask( Exhibit target, Vector3 newPosition )
        {
            var task = new KeeperTask();
            task.type = KeeperTask.Type.Move;
            task.exhibit = target;
            task.point = newPosition;
            AddNewTask( ref task );
        }
        

        public bool IsNewTaskAvailable()
        {
            if (m_keeperTasks.Count > 0)
                return true;
            return false;
        }
        
        public KeeperTask GetNextTask( Keeper worker )
        {
            m_tasksLeft -= 1;
            if (m_keeperTasks.Count > 0)
            {
                m_keeperWorking.Add(worker);
                return m_keeperTasks.Dequeue();
            }

            return new KeeperTask();
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

        
        public Keeper GetIdleKeeper()
        {
            foreach( var keeper in m_keepers )
            {
                if( keeper.isIdle )
                {
                    return keeper;
                }
            }

            return null;
        }


        protected new void Update()
        {
            base.Update();

            #if UNITY_EDITOR
            //--TESTING, add active points from hierarchy to the task queue
            foreach (Transform child in m_ExhibitTestPoints) {
                if (child.gameObject.activeInHierarchy){
                    //if (m_keeperPoints.Contains(child.position) == false) {
                    //    AddNewTask(child.position);
                    //    Destroy(child.gameObject);
                    //}

                }
            }
            #endif

            // No new tasks
            if( IsNewTaskAvailable() == false )
            {
                return;
            }

            // Get a keeper that isn't currently busy
            var keeper = GetIdleKeeper();
            if( keeper == null )
            {
                return;
            }

            // Assign tasks to non-busy workers
            Debug.Log("Assigning new task to keeper "+keeper);
            keeper.gameObject.SetActive(true);
            var task = GetNextTask( keeper );
            switch( task.type )
            {
                case KeeperTask.Type.Place:
                    keeper.AddPlaceTask(task.exhibit);
                break;
                case KeeperTask.Type.Upgrade:
                    keeper.AddPlaceTask(task.exhibit);
                break;
                case KeeperTask.Type.Move:

                break;
                case KeeperTask.Type.Remove:
                    keeper.AddRemoveTask(task.exhibit);
                break;
            }


        }

    
        void OnDrawGizmos()
        {
            // foreach (var task in m_keeperTasks) {
            //     Gizmos.color = new Color(1, 0, 0, 1f);
            //     Gizmos.DrawCube(task.position, new Vector3(0.5f, 0.2f, 0.5f));
            // }
        }
    }
}