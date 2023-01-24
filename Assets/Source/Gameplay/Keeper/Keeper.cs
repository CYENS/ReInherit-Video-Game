using System;
using System.Collections;
using System.Collections.Generic;
using Cyens.ReInherit.Gameplay.Management;
using UnityEngine;
using Pathfinding;
using UnityEngine.Animations.Rigging;

namespace Cyens.ReInherit
{
    public class Keeper : MonoBehaviour
    {
        [SerializeField]
        private enum State {Idle, Ready, Carry, Work, Return }
        public enum Goal { PlaceExhibit=0, UpgradeExhibit=1 }




        private KeeperManager m_keeperManager;
        private List<Renderer> m_renderers;
        private AIPath m_aiPath;
        [SerializeField] private Task currentTask;
        [SerializeField] private GameObject m_carryBox;
        [SerializeField] private GameObject m_BoxDissolve;
        [SerializeField] private State m_state = State.Ready;
        [SerializeField] private float m_timer = 0f;
        [SerializeField] private GameObject m_showcasePrefab;
        [SerializeField] private GameObject m_RigHands;
        private GameObject newBox;
        private Rig m_WeightRigHand;
        private float changeTimeDissolve = 1f; // time it takes to change the value
        private float valueDissolve = 0f; // the value to change


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
            m_WeightRigHand = m_RigHands.GetComponent<Rig>();
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


        private void LookAt(Vector3 target, float turnSpeed = 10.0f)
        {
            Vector3 myPos = transform.position;
            Vector3 toTarget = target - myPos;
            toTarget.y = 0;

            transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    Quaternion.LookRotation(toTarget),
                    Time.deltaTime * turnSpeed
                );
        }

        /// <summary>
        /// Execute keeper logic
        /// </summary>
        private void ExecuteKeeperLogic()
        {
            switch (m_state) {
                //Keeper is ready to carry next exhibit; check for new task
                case State.Ready:
                    if (m_keeperManager.IsNewTaskAvailable() == false) return;

                    currentTask = m_keeperManager.GetNextTask(this);
                    SetMovePosition(currentTask.target.GetStandPoint());
                    m_state = State.Carry;
                    EnableRenderers();
                    m_carryBox.SetActive(true);
                    m_WeightRigHand.weight = 1;
                    break;
                
                //Keeper is carrying a crate; check if arrived at place
                case State.Carry:
                    if (m_aiPath.remainingDistance >= 0.1f) return;
                    if (m_aiPath.pathPending) return;

                    // TODO: Play an animation depending on the current goal/job
                    m_state = State.Work;
                    m_timer = m_keeperManager.GetPlacingDelay();
                    
                    break;
                
                //keeper places the exhibit
                case State.Work:


                    // Look directly at the target object
                    m_aiPath.enableRotation = false;
                    LookAt(currentTask.position);

                    if (m_timer >= float.Epsilon) return;

                    m_BoxDissolve.transform.position = m_carryBox.transform.position;
                    m_BoxDissolve.transform.rotation = m_carryBox.transform.rotation;
                    m_BoxDissolve.SetActive(true);
                    m_carryBox.SetActive(false);
                    StartCoroutine(ChangeValueDissolve());
                    Invoke("HideDissolveBox", 2f);

                    // Finalize task depending on the task goal
                    switch (currentTask.goal)
                    {
                        case Goal.PlaceExhibit:
                            currentTask.target.SetStatus(Artifact.Status.Exhibit);
                            break;
                        case Goal.UpgradeExhibit:
                            currentTask.target.FinalizeUpgrade();
                            break;

                    }
                    // Set destination back to the elevator
                    Vector3 basePoint = m_keeperManager.GetBasePosition();
                    SetMovePosition(basePoint);
                    m_state = State.Return;
                    
                    break;

                //Keeper is returning to base; check if arrived
                case State.Return:
                    m_aiPath.enableRotation = true;

                    // Signal the keeper manager that there the task is complete
                    m_keeperManager.DoneWorking(this);

                    if (m_aiPath.remainingDistance >= 0.5f) return;
                    if (m_aiPath.pathPending) return;
                    
                    m_state = State.Idle;
                    m_timer = m_keeperManager.GetIdleDelay();
                    DisableRenderers();
                    break;
                
                //Keeper is idling; check if timer passed for becoming ready
                case State.Idle:
                    if (m_timer >= float.Epsilon) return;

                    m_state = State.Ready;
                    break;
                
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Update timer
            m_timer = Mathf.Max(m_timer - Time.deltaTime, 0.0f);

            // Update Keeper Logic
            ExecuteKeeperLogic();
        }


        private void SetMovePosition(Vector3 position)
        {
            m_aiPath.destination = position;
            m_aiPath.SearchPath();
        }

        private void HideDissolveBox()
        {
            m_BoxDissolve.SetActive(false);
        }

        IEnumerator ChangeValueDissolve()
        {
            float t = 0f;
            while (t < changeTimeDissolve)
            {
                t += Time.deltaTime;
                valueDissolve = Mathf.Lerp(0f, 1f, t / changeTimeDissolve);
                m_BoxDissolve.GetComponent<MeshRenderer>().material.SetFloat("_Dissolve_Amount", valueDissolve);
                yield return null;
            }
        }
    }
}
