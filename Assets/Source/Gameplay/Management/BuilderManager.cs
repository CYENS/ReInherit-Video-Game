using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cyens.ReInherit.Patterns;
using UnityEditor;
using UnityEngine;
using Pathfinding;

namespace Cyens.ReInherit.Gameplay.Management
{
    public class BuilderManager : Singleton<BuilderManager>
    {
        public Queue<Builder.Task> m_builderTasks = new Queue<Builder.Task>();
        [SerializeField] private Transform m_buildersParent;
        private List<Builder> m_builders;
        private List<Vector3> m_lastQueue;
        [SerializeField] [Tooltip("Number of task left for the builders.")]
        private int m_tasksLeft = 0;
        [SerializeField] private Transform m_basePosition;
        [SerializeField] private Transform m_RoomTestPoints;
        [SerializeField] [Tooltip("Seconds to wait for building")]
        private float m_buildingDelay;
        [SerializeField] [Tooltip("Seconds to wait/idling until next task.")]
        private float m_idleDelay;

        public float GetBuildingDelay()
        {
            return m_buildingDelay;
        }

        public float GetIdleDelay()
        {
            return m_idleDelay;
        }

        public bool CheckAllBuildersSaneState(Builder.State state)
        {
            foreach (var builder in m_builders) {
                if (builder.GetState() != state)
                    return false;
            }
            return true;
        }

        public bool CheckAllBuildersRemainingDistance(float threshold)
        {
            foreach (var builder in m_builders) {
                if (builder.GetComponent<AIPath>().remainingDistance > threshold)
                    return false;
            }
            return true;
        }

        public void SetLastQueue()
        {
            foreach (var builder in m_builders) {
                m_lastQueue[builder.GetBuilderIndex()] = builder.transform.position;
            }
            m_lastQueue.Reverse();
        }

        public Vector3 GetLastQueuePos(int index)
        {
            return m_lastQueue[index];
        }

        private void AddNewTask(Vector3 roomCenter, List<Vector3> roomPoints, Builder.Goal goal)
        {
            var task = new Builder.Task();
            task.room = roomCenter;
            task.roomPoints = roomPoints;
            task.goal = goal;

            m_builderTasks.Enqueue(task);
            m_tasksLeft += 1;
        }

        public void AddBuildTask(Vector3 target, List<Vector3> points) => AddNewTask(target, points, Builder.Goal.BuildNewRoom);


        public bool IsNewTaskAvailable()
        {
            if (m_builderTasks.Count > 0)
                return true;
            return false;
        }
        
        public Builder.Task GetNextTask()
        {
            m_tasksLeft -= 1;
            if(m_builderTasks.Count > 0)
                return m_builderTasks.Dequeue();
            return null;
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
            m_lastQueue = new List<Vector3>(m_buildersParent.childCount);
            for (int i = 0; i < m_buildersParent.childCount; i++) {
                m_lastQueue.Add(Vector3.zero);
            }
            m_builders = new List<Builder>(m_buildersParent.childCount);
            foreach (Transform builder in m_buildersParent) {
                m_builders.Add(builder.GetComponent<Builder>());
            }
        }

        private void Update()
        {
            //--TESTING, add active points from hierarchy to the task queue
            foreach (Transform room in m_RoomTestPoints) {
                if (room.gameObject.activeInHierarchy) {
                    List<Vector3> points = new List<Vector3>(5);
                    foreach (Transform point in room)
                        points.Add(point.position);
                    AddBuildTask(room.position, points);
                    Destroy(room.gameObject);
                }
            }
        }
    }
}
