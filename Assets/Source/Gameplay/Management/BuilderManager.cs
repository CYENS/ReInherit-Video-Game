using System;
using System.Collections;
using System.Collections.Generic;
using Cyens.ReInherit.Patterns;
using UnityEngine;

namespace Cyens.ReInherit.Gameplay.Management
{
    public class BuilderManager : Singleton<BuilderManager>
    {
        public Queue<List<Vector3>> m_roomPoints = new Queue<List<Vector3>>();
        [SerializeField] private Transform m_buildersParent;
        private List<Builder> m_builders;
        [SerializeField] [Tooltip("Number of rooms left to build.")]
        private int m_tasksLeft = 0;
        [SerializeField] private Transform m_basePosition;
        [SerializeField] private Transform m_RoomTestPoints;
        [SerializeField] [Tooltip("Seconds to wait for building the room.")]
        private float m_buildingDelay;
        [SerializeField] [Tooltip("Seconds to wait/idling until next task.")]
        private float m_idleDelay;
        [SerializeField] [Tooltip("Seconds to wait between each builder to start moving")]
        private float m_spawnDelay;

        public float GetBuildingDelay()
        {
            return m_buildingDelay;
        }

        public float GetIdleDelay()
        {
            return m_idleDelay;
        }
        
        public void AddNewTask(List<Vector3> points)
        {
            m_roomPoints.Enqueue(points);
            m_tasksLeft += 1;
        }
        
        public Vector3 GetBasePosition()
        {
            return m_basePosition.position;
        }

        public override void Awake()
        {
            base.Awake();
        }
        
        private void Start()
        {
            m_builders = new List<Builder>(m_buildersParent.childCount);
            foreach (Transform child in m_buildersParent) {
                m_builders.Add(child.GetComponent<Builder>());
            }
        }

        /// <summary>
        /// Check if all builders are ready to start a new task
        /// </summary>
        private bool AllBuildersReady()
        {
            foreach (Builder b in m_builders) {
                if (b.GetState() != Builder.State.Ready)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Run Builder logic. First set the destination of all builders to the
        /// first point in list(Room center). Then each builder will go to a specific point in the room.
        /// This is to ensure that all builder will follow same path to arrive at the room.
        /// ALso add a small spawn delay so they move in a queue.
        /// </summary>
        private void RunBuilderManagerLogic()
        {
            if (m_tasksLeft <= 0) 
                return; 
            if (AllBuildersReady() == false)
                return;
            
            m_tasksLeft -= 1;
            List<Vector3> points = m_roomPoints.Dequeue();
            for (int i = 0; i < m_builders.Count; i++) {
                Builder b = m_builders[i];
                b.SetRoomEndPoint(points[i + 1]);
                StartCoroutine(b.SetMovePosition(points[0], m_spawnDelay * i, Builder.State.MovingToRoom));
            }
        }

        private void Update()
        {
            RunBuilderManagerLogic();
            
            //--TESTING, add active points from hierarchy to the task queue
            foreach (Transform child in m_RoomTestPoints) {
                if (child.gameObject.activeInHierarchy) {
                    List<Vector3> points = new List<Vector3>(child.childCount);
                    points.Add(child.position);
                    foreach (Transform point in child) {
                        points.Add(point.position);
                    }
                    AddNewTask(points);
                    Destroy(child.gameObject);
                }
            }
        }

        void OnDrawGizmos()
        {
            foreach (Transform points in m_RoomTestPoints) {
                Gizmos.color = new Color(0, 1, 0 , 1f);
                foreach (Transform point in points)
                    Gizmos.DrawCube(point.position, new Vector3(0.5f, 0.2f, 0.5f));
            }
        }
    }
}