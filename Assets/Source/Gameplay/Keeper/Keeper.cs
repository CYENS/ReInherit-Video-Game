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
        private enum State {Idle, Ready, Carry, Place, Upgrade, Return }
        public enum Goal { PlaceExhibit=0, UpgradeExhibit=1 }




        private KeeperManager m_keeperManager;
        private List<Renderer> m_renderers;
        private AIPath m_aiPath;
        [SerializeField] private Task currentTask;
        [SerializeField] private GameObject m_carryBox;
        [SerializeField] private State m_state = State.Ready;
        [SerializeField] private float m_placingTimer = 0f;
        [SerializeField] private float m_idleTimer = 0;
        [SerializeField] private GameObject m_showcasePrefab;
        


        [System.Serializable]
        public class Task
        {
            public Artifact target;
            public Goal goal;

            public Vector3 position => target.transform.position;
        }



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
        /// Helper function to make usage more convenient.
        /// </summary>
        private void EnableRenderers() => EnableDisableRenderers(true);

        /// <summary>
        /// Helper function to make usage more convenient.
        /// </summary>
        private void DisableRenderers() => EnableDisableRenderers(false);




        /// <summary>
        /// Execute keeper logic
        /// </summary>
        private void ExecuteKeeperLogic()
        {
            switch (m_state) {
                //Keeper is ready to carry next exhibit; check for new task
                case State.Ready: {
                    if (m_keeperManager.IsNewTaskAvailable()) {

                        currentTask = m_keeperManager.GetNextTask();
                        SetMovePosition(currentTask.position);
                        m_state = State.Carry;
                        EnableRenderers();
                        m_carryBox.SetActive(true);
                    }
                    break;
                }
                //Keeper is carrying a crate; check if arrived at place
                case State.Carry: {
                    if (m_aiPath.remainingDistance < 1f && m_aiPath.pathPending == false) {

                        switch( currentTask.goal )
                        {
                            case Goal.PlaceExhibit: m_state = State.Place; break;
                            case Goal.UpgradeExhibit: m_state = State.Upgrade; break;

                        }
                        m_placingTimer = 0f;
                        m_carryBox.SetActive(false);
                    }
                    break;
                }
                //keeper places the exhibit
                case State.Place: {

                    // TODO: Play animation for placing the crate
                    if (m_placingTimer >= m_keeperManager.GetPlacingDelay()) {
                        Vector3 basePoint = m_keeperManager.GetBasePosition();
                        SetMovePosition(basePoint);
                        m_state = State.Return;
                    }
                    break;
                }
                //Keeper is returning to base; check if arrived
                case State.Return: {
                    if (m_aiPath.remainingDistance < 0.5f  && m_aiPath.pathPending == false) {
                        m_state = State.Idle;
                        m_idleTimer = 0f;
                        DisableRenderers();
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
            if (m_state == State.Place)
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
