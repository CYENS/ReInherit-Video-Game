using System;
using System.Collections;
using System.Collections.Generic;
using Cyens.ReInherit.Gameplay.Management;
using UnityEngine;
using Pathfinding;

namespace Cyens.ReInherit
{
    public class Builder : MonoBehaviour
    {
        public enum State {Idle, Ready, MovingToRoom, MovingToRoomPoint, Building, Returning }
        private enum BuilderType {Carpenter, Concreter, Plumber, Electrician, Welder}

        private BuilderManager m_builderManager;
        private List<Renderer> m_renderers;
        private AIPath m_aiPath;
        [SerializeField] private State m_state = State.Ready;
        [SerializeField] private BuilderType m_builderType;
        [SerializeField] private float m_buildingTimer = 0f;
        [SerializeField] private float m_idleTimer = 0;
        private Vector3 m_roomEndPoint;

        // Start is called before the first frame update
        void Start()
        {
            m_builderManager = BuilderManager.Instance; 
            m_aiPath = GetComponent<AIPath>();
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
        /// Execute builder logic
        /// </summary>
        private void ExecuteBuilderLogic()
        {
            switch (m_state) {
                //Builder is ready to be assign to a room; check for new task
                case State.Ready: {
                    break;
                }
                //Builder moving towards room; check if arrived at place
                case State.MovingToRoom: {
                    EnableDisableRenderers(true);
                    if (m_aiPath.remainingDistance < 4f && m_aiPath.pathPending == false) {
                        SetMovePosition(m_roomEndPoint);
                        m_state = State.MovingToRoomPoint;
                    }
                    break;
                }
                //Builder moving specific point in room; check if arrived at place
                case State.MovingToRoomPoint: {
                    if (m_aiPath.remainingDistance < 0.5f && m_aiPath.pathPending == false) {
                        m_state = State.Building;
                        m_buildingTimer = 0f;
                    }
                    break;
                }
                //Builder is currenlty building; check if done
                case State.Building: {
                    if (m_buildingTimer >= m_builderManager.GetBuildingDelay()) {
                        Vector3 basePoint = m_builderManager.GetBasePosition();
                        SetMovePosition(basePoint);
                        m_state = State.Returning;
                    }
                    break;
                }
                //Builder is returning to base; check if arrived
                case State.Returning: {
                    if (m_aiPath.remainingDistance < 0.5f  && m_aiPath.pathPending == false) {
                        m_state = State.Idle;
                        m_idleTimer = 0f;
                        EnableDisableRenderers(false);
                    }
                    break;
                }
                //Builder is idling; check if timer passed for becoming ready
                case State.Idle: {
                    if (m_idleTimer >= m_builderManager.GetIdleDelay()) {
                        m_state = State.Ready;
                    }
                    break;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            ExecuteBuilderLogic();
        }

        private void FixedUpdate()
        {
            //Timers
            if (m_state == State.Building)
                m_buildingTimer += Time.fixedDeltaTime;
            else
                m_buildingTimer = 0f;
            
            if (m_state == State.Idle)
                m_idleTimer += Time.fixedDeltaTime;
            else
                m_idleTimer = 0f;
        }

        public void SetState(State state)
        {
            m_state = state;
        }

        public State GetState()
        {
            return m_state;
        }

        public void SetRoomEndPoint(Vector3 point)
        {
            m_roomEndPoint = point;
        }

        private void SetMovePosition(Vector3 position)
        {
            m_aiPath.destination = position;
            m_aiPath.SearchPath();
        }
        
        public IEnumerator SetMovePosition(Vector3 position, float seconds, State state)
        {
            yield return new WaitForSeconds(seconds);
            m_aiPath.destination = position;
            m_aiPath.SearchPath();
            m_state = state;
        }
    }
}
