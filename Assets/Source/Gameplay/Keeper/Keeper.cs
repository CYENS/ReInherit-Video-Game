using System;
using System.Collections;
using System.Collections.Generic;
using Cyens.ReInherit.Gameplay.Management;
using UnityEngine;
using Pathfinding;

namespace Cyens.ReInherit
{
    public class Keeper : MonoBehaviour
    {
        [SerializeField]
        private enum State {Idle, Ready, CarryExhibit, PlacingExhibit, Returning }

        private KeeperManager m_keeperManager;
        private List<Renderer> m_renderers;
        private AIPath m_aiPath;
        [SerializeField] private GameObject m_carryBox;
        [SerializeField] private State m_state = State.Ready;
        [SerializeField] private float m_placingTimer = 0f;
        [SerializeField] private float m_idleTimer = 0;


        // Start is called before the first frame update
        void Start()
        {
            m_keeperManager = KeeperManager.Instance; 
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
                    //Do not add box carry Renderer also
                    if(!GameObject.ReferenceEquals(m_carryBox, child.gameObject))
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
        /// E
        /// </summary>
        private void ExecuteKeeperLogic()
        {
            switch (m_state) {
                //Keeper is ready to carry next exhibit; check for new task
                case State.Ready: {
                    if (m_keeperManager.IsNewTaskAvailable()) {
                        Vector3 exhibitPoint = m_keeperManager.GetNextTask();
                        SetMovePosition(exhibitPoint);
                        m_state = State.CarryExhibit;
                        EnableDisableRenderers(true);
                        m_carryBox.SetActive(true);
                    }
                    break;
                }
                //Keeper is carrying an exhibit; check if arrived at place
                case State.CarryExhibit: {
                    if (m_aiPath.remainingDistance < 1f && m_aiPath.pathPending == false) {
                        m_state = State.PlacingExhibit;
                        m_placingTimer = 0f;
                        m_carryBox.SetActive(false);
                    }
                    break;
                }
                //keeper is placing the exhibit; check if done
                case State.PlacingExhibit: {
                    if (m_placingTimer >= m_keeperManager.GetPlacingDelay()) {
                        Vector3 basePoint = m_keeperManager.GetBasePosition();
                        SetMovePosition(basePoint);
                        m_state = State.Returning;
                    }
                    break;
                }
                //Keeper is returning to base; check if arrived
                case State.Returning: {
                    if (m_aiPath.remainingDistance < 1f  && m_aiPath.pathPending == false) {
                        m_state = State.Idle;
                        m_idleTimer = 0f;
                        EnableDisableRenderers(false);
                    }
                    break;
                }
                //Keeper is idling; check if timer passed for becoming ready
                case State.Idle: {
                    if (m_idleTimer >= m_keeperManager.GetIdleDelay()) {
                        m_state = State.Ready;
                    }
                    break;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            ExecuteKeeperLogic();
        }

        private void FixedUpdate()
        {
            //Timers
            if (m_state == State.PlacingExhibit)
                m_placingTimer += Time.fixedDeltaTime;
            else
                m_placingTimer = 0f;
            
            if (m_state == State.Idle)
                m_idleTimer += Time.fixedDeltaTime;
            else
                m_idleTimer = 0f;
        }

        private void SetMovePosition(Vector3 position)
        {
            m_aiPath.destination = position;
            m_aiPath.SearchPath();
        }
    }
}
