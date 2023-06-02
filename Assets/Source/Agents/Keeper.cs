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
    public class Keeper : BaseAgent
    {

        private KeeperManager m_keeperManager;
        //private List<Renderer> m_renderers;
        private Renderer[] m_renderers;

        private AIPath m_aiPath;
        [SerializeField] private Task currentTask;



 

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

        [Header("Object References")]
        [SerializeField] private GameObject m_carryBox;

        // [System.Serializable]
        // public class Task
        // {
        //     public Exhibit target;
        //     public Goal goal;

        //     public Vector3 position => target.transform.position;
        // }





        // Start is called before the first frame update
        protected new void Start()
        {
            base.Start();

            //m_aiPath = GetComponent<AIPath>();
            //FindRenderers();
            //EnableDisableRenderers(false);
            m_WeightRigHand = m_RigHands.GetComponent<Rig>();


            m_keeperManager = KeeperManager.Instance;

            // Deactivate by default so they're not visible
            gameObject.SetActive(false);
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


        protected override void Update()
        {
            base.Update();

            
            
        }


        public void AddRemoveTask( Exhibit exhibit )
        {
            // Make the box invisible
            m_carryBox.SetActive(false);

            Vector3 standPoint = exhibit.ClosestStandPoint(transform.position);
            Vector3 basePoint = m_keeperManager.GetBasePosition();

            // Go to the crate
            AddMoveTask( standPoint, 1.0f, 0.1f );

            // Look at the exhibit
            AddLookTask( exhibit.transform, 10.0f, 1.0f );

            // Next activate the dissolve box effect
            AddActivateTask( m_carryBox );

            AddMessageTask( exhibit.gameObject , "Remove" );

            AddWaitTask(1.0f);

            AddMoveTask( basePoint, 0.8f, 0.5f );
            AddMessageTask( gameObject, "Done" );
        }
    

        public void AddPlaceTask(Exhibit exhibit)
        {
            // Make the box visible
            m_carryBox.SetActive(true);

            Vector3 standPoint = exhibit.ClosestStandPoint(transform.position);
            Vector3 basePoint = m_keeperManager.GetBasePosition();

            // First carry the crate to where the exhibit will be placed
            AddMoveTask( standPoint, 1.0f, 0.1f );

            // Look at the exhibit
            AddLookTask( exhibit.transform, 10.0f, 1.0f );

            // Next activate the dissolve box effect
            AddActivateTask( exhibit.dissolveBox );

            // Wait for one second, then change the state of the exhibit
            AddDeactivateTask(m_carryBox);
            AddWaitTask(1.0f);
            AddMessageTask( exhibit.gameObject, "Install" );
            AddWaitTask(2.0f);

            // Move back to base and deactivate
            AddMoveTask( basePoint, 0.8f, 0.5f );
            AddMessageTask( gameObject, "Done" );

        }

        /// <summary>
        /// Done with all tasks.
        /// Notify keeper manager and deactivate.
        /// </summary>
        public void Done() 
        {
            m_keeperManager.DoneWorking( this );
            gameObject.SetActive(false);
        }

    }
}
