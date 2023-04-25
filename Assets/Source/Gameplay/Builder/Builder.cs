using System;
using System.Collections;
using System.Collections.Generic;
using Cyens.ReInherit.Managers;
using UnityEngine;
//using Pathfinding;
//using Pathfinding.RVO;
using Unity.VisualScripting;
using UnityEngine.AI;

namespace Cyens.ReInherit
{
    public class Builder : MonoBehaviour
    {
        public enum State {Idle, Ready, MovingToRoom, MovingToRoomPoint, Building, FormingQueue, Returning }
        private enum BuilderType {Carpenter, Concreter, Plumber, Electrician, Welder}

        private BuilderManager m_builderManager;
        private List<Renderer> m_renderers;
        private NavMeshAgent m_navAgent;
        [SerializeField] private int m_builderIndex;
        [SerializeField] private Task m_currentTask;
        [SerializeField] private bool m_isMainBuilder;
        [SerializeField] private Builder m_MainBuilder;
        [SerializeField] private Builder m_forntBuilder;
        [SerializeField] private State m_state = State.Ready;
        [SerializeField] private BuilderType m_builderType;
        [SerializeField] private float m_buildingTimer = 0f;
        [SerializeField] private float m_idleTimer = 0;

        public enum Goal { BuildNewRoom = 0  }
        public NavMeshAgent NavAgent { get => m_navAgent; }
        public Vector3 GetRoomPoint(int builderIndex)
        {
            return m_currentTask.roomPoints[builderIndex];
        }

        public State GetState()
        {
            return m_state;
        }
        public int GetBuilderIndex()
        {
            return m_builderIndex;
        }
        
        [System.Serializable]
        public class Task
        {
            public Goal goal;
            public Vector3 room;
            public List<Vector3> roomPoints;
        }
        
        // Start is called before the first frame update
        void Start()
        {
            m_builderManager = BuilderManager.Instance; 
            m_navAgent = GetComponent<NavMeshAgent>();
            FindRenderers();
            EnableDisableRenderers(false);
        }

        /// <summary>
        /// Find all child renderers of the model.
        /// </summary>
        private void FindRenderers()
        {
            m_renderers = new List<Renderer>();
            foreach (Transform child in transform) {
                if (child.TryGetComponent(out Renderer r)) {
                    m_renderers.Add(r);
                }
            }
        }

        /// <summary>
        /// Enables/Disables model's renderers.
        /// </summary>
        private void EnableDisableRenderers(bool enable)
        {
            if (enable) {
                foreach(Renderer r in m_renderers)
                    r.enabled = true;
            }else {
                foreach(Renderer r in m_renderers)
                    r.enabled = false;
            }
        }

        /// <summary>
        /// Helper function to make usage more convenient.
        /// </summary>
        private void EnableRenderers() => EnableDisableRenderers(true);

        /// <summary>
        /// Helper function to make usage more convenient.
        /// </summary>
        private void DisableRenderers() => EnableDisableRenderers(false);
        
        /// <summary>
        /// Execute builder logic
        /// </summary>
        private void ExecuteMainBuilderLogic()
        {
            switch (m_state) {
                //Builder is ready to be assigned to a room; check for new task
                case State.Ready: {
                    if (m_builderManager.IsNewTaskAvailable() == false) return;
                    if (m_builderManager.CheckAllBuildersSaneState(State.Ready) == false) return;
                    
                    m_currentTask = m_builderManager.GetNextTask();
                    SetMovePosition(m_currentTask.room);
                    m_state = State.MovingToRoom;
                    EnableRenderers();

                    break;
                }
                //Builder moving towards room; check if arrived at place
                case State.MovingToRoom: {
                    if (m_navAgent.remainingDistance < 2f && m_navAgent.pathPending == false) {
                        m_builderManager.SetLastQueue();
                        SetMovePosition(GetRoomPoint(m_builderIndex));
                        m_state = State.MovingToRoomPoint;
                    }
                    break;
                }
                //Builder moving specific point in room; check if arrived at place
                case State.MovingToRoomPoint: {
                    if (m_navAgent.remainingDistance < 0.5f && m_navAgent.pathPending == false) {
                        m_state = State.Building;
                        m_buildingTimer = 0f;
                    }
                    break;
                }
                //Builder is currenlty building; check if done
                case State.Building: {
                    if (m_buildingTimer >= m_builderManager.GetBuildingDelay()) {
                        Vector3 basePoint = m_builderManager.GetBasePosition();
                        SetMovePosition(m_builderManager.GetLastQueuePos(m_builderIndex));
                        m_state = State.FormingQueue;
                    }
                    break;
                }
                //Builder taking its place in queue to return
                case State.FormingQueue: {
                    if (m_navAgent.remainingDistance < 0.1f && m_navAgent.pathPending == false) {
                        if(m_builderManager.CheckAllBuildersRemainingDistance(0.2f)){
                            SetMovePosition(m_builderManager.GetBasePosition());
                            m_state = State.Returning;
                        }
                    }
                    break;
                }
                //Builder is returning to base; check if arrived
                case State.Returning: {
                    if (m_navAgent.remainingDistance < 0.2f && m_navAgent.pathPending == false) {
                        m_state = State.Idle;
                        m_idleTimer = 0f;
                    }
                    break;
                }
                //Builder is idling; check if timer passed for becoming ready
                case State.Idle: {
                    DisableRenderers();
                    if (m_idleTimer >= m_builderManager.GetIdleDelay()) {
                        m_state = State.Ready;
                    }
                    break;
                }
            }
        }

        private void ExecuteSimpleBuilderLogic()
        {
            if(m_MainBuilder.GetState() != State.Idle && m_MainBuilder.GetState() != State.Ready)
                m_state = m_MainBuilder.GetState();
            switch (m_state) {
                case State.Ready: {
                    DisableRenderers();
                    break;
                }
                //Builder moving towards room; check if arrived at place
                case State.MovingToRoom: {
                    float disToFront = Vector3.Distance(transform.position, m_forntBuilder.transform.position);
                    if (disToFront > 1f) {
                        EnableRenderers();
                        SetMovePosition(m_forntBuilder.transform.position);
                    }

                    break;
                }
                //Builder moving specific point in room; check if arrived at place
                case State.MovingToRoomPoint: {
                    SetMovePosition(m_MainBuilder.GetRoomPoint(m_builderIndex));
                    break;
                }
                //Builder is currently building; check if done
                case State.Building: {
                    break;
                }
                //Builder taking its place in queue to return
                case State.FormingQueue: {
                    SetMovePosition(m_builderManager.GetLastQueuePos(m_builderIndex));
                    break;
                }
                //Builder is returning to base; check if arrived
                case State.Returning: {
                    if (Vector3.Distance(transform.position, m_builderManager.GetBasePosition()) < 2.75f) {
                        m_state = State.Ready;
                        return;
                    }
                    SetMovePosition(m_forntBuilder.transform.position);
                    break;
                }
                case State.Idle: {
                    break;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (m_isMainBuilder)
                ExecuteMainBuilderLogic();
            else
                ExecuteSimpleBuilderLogic();
        }

        private void FixedUpdate()
        {
            if (m_isMainBuilder) {
                if (m_state == State.Building)
                    m_buildingTimer += Time.fixedDeltaTime;
                else
                    m_buildingTimer = 0f;

                if (m_state == State.Idle)
                    m_idleTimer += Time.fixedDeltaTime;
                else
                    m_idleTimer = 0f;
            }
        }

        private void SetMovePosition(Vector3 position)
        {
            m_navAgent.SetDestination(position);
        }
    }
}
