using System;
using System.Collections;
using System.Collections.Generic;
using Cyens.ReInherit.Managers;
using UnityEngine;
using UnityEngine.AI;

using Pathfinding;
using UnityEngine.Animations.Rigging;
using Cyens.ReInherit.Exhibition;

namespace Cyens.ReInherit
{
    public class Keeper : MonoBehaviour
    {
        [SerializeField]
        private enum State {Idle, Ready, Carry, Work, Return }
        public enum Goal { PlaceExhibit=0, UpgradeExhibit=1 }
        


        private KeeperManager m_keeperManager;
        //private List<Renderer> m_renderers;
        private Renderer[] m_renderers;

        private AIPath m_aiPath;
        [SerializeField] private Task currentTask;
        [SerializeField] private GameObject m_carryBox;
        [SerializeField] private State m_state = State.Ready;
        [SerializeField] private float m_timer = 0f;
        [SerializeField] private GameObject m_showcasePrefab;
        [SerializeField] private GameObject m_RigHands;
        private GameObject m_BoxDissolve;
        private GameObject newBox;
        private Rig m_WeightRigHand;
        private float changeTimeDissolve = 1f; // time it takes to change the value
        private float valueDissolve = 0f; // the value to change
        [SerializeField] private Material characterMaterial;
        [SerializeField] private Material trolleyMaterial1;
        [SerializeField] private Material trolleyMaterial2;
        [SerializeField] private Material trolleyMaterial3;
        private float changeTimeOpacity = 0.5f;
        private float valueOpacity = 1;


        private NavMeshAgent m_agent;


        [System.Serializable]
        public class Task
        {
            public Exhibit target;
            public Goal goal;

            public Vector3 position => target.transform.position;
        }

        // Start is called before the first frame update
        void Start()
        {
            m_keeperManager = KeeperManager.Instance;

            m_agent = GetComponent<NavMeshAgent>();

            //m_aiPath = GetComponent<AIPath>();
            FindRenderers();
            EnableDisableRenderers(false);
            m_WeightRigHand = m_RigHands.GetComponent<Rig>();
        }

        /// <summary>
        /// Find all child renderers of the model.
        /// </summary>
        private void FindRenderers()
        {
            m_renderers = GetComponentsInChildren<Renderer>();
            // m_renderers = new List<Renderer>();
            // foreach (Transform child in transform) {
            //     if (child.TryGetComponent(out Renderer r)) {
            //         Debug.Log("Renderer: "+r);
            //         //Do not add box carry Renderer also
            //         if(!GameObject.ReferenceEquals(m_carryBox, child.gameObject))
            //             m_renderers.Add(r);
            //     }
            // }
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
        /// Keeper is ready to carry next exhibit; check for new task
        /// </summary>
        private void ReadyLogic() 
        {
            // Wait for next task to be available
            if (m_keeperManager.IsNewTaskAvailable() == false) 
            {
                return;
            }

            // Get Next task
            currentTask = m_keeperManager.GetNextTask(this);

            Exhibit exhibit = currentTask.target;
            Vector3 standPoint = exhibit.ClosestStandPoint(transform.position);
            SetMovePosition(standPoint, true, 3f, 10);
            m_state = State.Carry;
            EnableRenderers();

            //m_BoxDissolve = currentTask.target.GetExhibit().GetBoxDissolve();

            //m_carryBox.transform.localScale = m_BoxDissolve.transform.localScale;
            //m_carryBox.transform.position = new Vector3(m_carryBox.transform.position.x, m_BoxDissolve.transform.position.y, m_carryBox.transform.position.z);
            //m_carryBox.SetActive(true);
            //m_WeightRigHand.weight = 1;
            
        }

        private void CarryLogic() 
        {
            if( m_agent.remainingDistance >= 0.1f ) return;
            if( m_agent.pathPending ) return;
            //if (m_aiPath.remainingDistance >= 0.1f) return;
            //if (m_aiPath.pathPending) return;

            // TODO: Play an animation depending on the current goal/job
            m_state = State.Work;
            m_timer = m_keeperManager.GetPlacingDelay();

            Exhibit exhibit = currentTask.target;
            exhibit.BeginDissolveEffect();
            m_carryBox.SetActive(false);
        }

        private void WorkLogic()
        {
            Exhibit exhibit = currentTask.target;
            LookAt(currentTask.position);
            if (m_timer >= float.Epsilon) return;

            // Finalize task depending on the task goal
            switch (currentTask.goal)
            {
                case Goal.PlaceExhibit:
                    exhibit.SetState(Exhibit.State.Display);
                    break;
                case Goal.UpgradeExhibit:
                    //exhibit.FinalizeUpgrade();
                    break;

            }
            // Set destination back to the elevator
            Vector3 basePoint = m_keeperManager.GetBasePosition();
            SetMovePosition(basePoint, true, 1.5f, 15);
            m_state = State.Return;
        }

        private void ReturnLogic()
        {
            //m_aiPath.enableRotation = true;

            // Signal the keeper manager that there the task is complete
            m_keeperManager.DoneWorking(this);


            if( m_agent.remainingDistance >= 0.1f ) return;
            if( m_agent.pathPending ) return;

            //if (m_aiPath.remainingDistance >= 0.5f) return;
            //if (m_aiPath.pathPending) return;
            
            m_state = State.Idle;
            m_timer = m_keeperManager.GetIdleDelay();
            DisableRenderers();
        }

        private void IdleLogic() 
        {
            if (m_timer >= float.Epsilon) return;
            m_state = State.Ready;
        }

        /// <summary>
        /// Execute keeper logic
        /// </summary>
        private void KeeperLogic()
        {
            

            switch (m_state) {

                // Keeper is ready to carry next exhibit; check for new task
                case State.Ready:
                    ReadyLogic();
                    break;
                
                //Keeper is carrying a crate; check if arrived at place
                case State.Carry:
                    CarryLogic();
                    break;
                
                //keeper places the exhibit
                case State.Work:
                    WorkLogic();
                    break;

                //Keeper is returning to base; check if arrived
                case State.Return:
                    ReturnLogic();
                    break;
                
                //Keeper is idling; check if timer passed for becoming ready
                case State.Idle:
                    IdleLogic();
                    break;
                
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Update timer
            m_timer = Mathf.Max(m_timer - Time.deltaTime, 0.0f);

            // Update Keeper Logic
            KeeperLogic();
        }


        private void SetMovePosition(Vector3 position, bool teleport, float teleportTimer, int appearIndex)
        {
            m_agent.SetDestination(position);

            // m_aiPath.destination = position;
            // m_aiPath.SearchPath();

            //if (teleport)
            //    StartCoroutine(Teleport(teleportTimer, appearIndex));
        }

        IEnumerator Teleport(float delay, int appearIndex)
        {
            //Wait delay seconds and then teleport the keeper
            yield return new WaitForSeconds(delay);

            // Get remaining points in path
            var buffer = new List<Vector3>();
            
            m_aiPath.GetRemainingPath(buffer, out bool stale);


            // If path is still long enough, teleport
            if (buffer.Count >= 20) {
                // Prevent keeper form moving to play disappear animation
                m_aiPath.canMove = false;

                // Enter below code to play disappear animation
                StartCoroutine(ChangeCharacterOpacity(characterMaterial, 1f, 0f));
                StartCoroutine(ChangeCharacterOpacity(m_carryBox.GetComponent<MeshRenderer>().material, 1f, 0f));
                StartCoroutine(ChangeCharacterOpacity(trolleyMaterial1, 1f, 0f));
                StartCoroutine(ChangeCharacterOpacity(trolleyMaterial2, 1f, 0f));
                StartCoroutine(ChangeCharacterOpacity(trolleyMaterial3, 1f, 0f));
                yield return new WaitForSeconds(changeTimeOpacity+.5f);

                Vector3 teleportPos = buffer[buffer.Count - appearIndex];
                m_aiPath.Teleport(teleportPos);

                // Enter below code to play appear animation
                StartCoroutine(ChangeCharacterOpacity(characterMaterial, 0f, 1f));
                StartCoroutine(ChangeCharacterOpacity(m_carryBox.GetComponent<MeshRenderer>().material, 0f, 1f));
                StartCoroutine(ChangeCharacterOpacity(trolleyMaterial1, 0f, 1f));
                StartCoroutine(ChangeCharacterOpacity(trolleyMaterial2, 0f, 1f));
                StartCoroutine(ChangeCharacterOpacity(trolleyMaterial3, 0f, 1f));
                yield return new WaitForSeconds(changeTimeOpacity+.5f);
                //yield return new WaitForSeconds("ENTER APPEAR ANIMATION DURATION TIME");
                m_aiPath.canMove = true;
            }
        }

        // private void HideDissolveBox()
        // {
        //     m_BoxDissolve.SetActive(false);
        // }

        // IEnumerator ChangeValueDissolve()
        // {
        //     float t = 0f;
        //     while (t < changeTimeDissolve)
        //     {
        //         t += Time.deltaTime;
        //         valueDissolve = Mathf.Lerp(0f, 1f, t / changeTimeDissolve);
        //         m_BoxDissolve.GetComponent<MeshRenderer>().material.SetFloat("_Dissolve_Amount", valueDissolve);
        //         yield return null;
        //     }
        // }

        IEnumerator ChangeCharacterOpacity(Material materialToChange, float startOpacity, float endOpacity)
        {
            float timer = 0f;
            while (timer < changeTimeOpacity)
            {
                timer += Time.deltaTime;
                valueOpacity = Mathf.Lerp(startOpacity, endOpacity, timer / changeTimeOpacity);
                materialToChange.SetFloat("_Opacity", valueOpacity);
                yield return null;
            }
            
        }
    }
}
