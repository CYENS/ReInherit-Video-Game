using System;
using System.Collections;
using System.Collections.Generic;
using Cyens.ReInherit.Managers;
using UnityEngine;

using UnityEngine.Animations.Rigging;
using Cyens.ReInherit.Exhibition;
using Unity.VisualScripting;
using UnityEngine.ProBuilder.MeshOperations;

namespace Cyens.ReInherit
{
    public class Janitor : BaseAgent
    {
        private JanitorManager m_janitorManager;
        private Renderer[] m_renderers;
        public bool returningToBase = false;
        
        [SerializeField] private GameObject m_RigHands;
        private GameObject m_BoxDissolve;
        private GameObject newBox;
        private float changeTimeDissolve = 1f; // time it takes to change the value
        private float valueDissolve = 0f; // the value to change
        private float changeTimeOpacity = 0.5f;
        private float valueOpacity = 1;
        private Garbage m_currentGarbage;

        // Start is called before the first frame update
        protected new void Start()
        {
            base.Start();
            m_janitorManager = JanitorManager.Instance;
            // Deactivate by default so they're not visible
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Find all child renderers of the model.
        /// </summary>
        private void FindRenderers()
        {
            m_renderers = GetComponentsInChildren<Renderer>();
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
        
        public void AddCleanTask( Garbage garbage )
        {
            m_currentGarbage = garbage;
            if (returningToBase)
                interruptMoveTask = true;
            returningToBase = true;
            // Go to the GARBAGE
            AddMoveTask( garbage.position, 1.0f, 0.1f );
            returningToBase = false;
            AddMessageTask( gameObject, "CleanGarbage" );
            AddWaitTask(1f);
            AddMessageTask( gameObject, "Done" );
        }

        public bool InBase()
        {
            Vector3 basePoint = m_janitorManager.GetBasePosition();
            if (Vector3.Distance(transform.position, basePoint) <= 2f)
                return true;
            return false;
        }

        public void AddReturnTask()
        {
            returningToBase = true;
            Vector3 basePoint = m_janitorManager.GetBasePosition();
            AddMoveTask( basePoint, 0.8f, 0.5f );
            AddMessageTask( gameObject, "DoneBase" );
        }

        /// <summary>
        /// Done with all tasks.
        /// Notify janitor manager and deactivate.
        /// </summary>
        public void Done() 
        {
            m_janitorManager.DoneWorking( this );
        }

        public void CleanGarbage()
        {
            if(m_currentGarbage != null)
                m_currentGarbage.Clean();
        }
        
        /// <summary>
        /// Done with all tasks.
        /// Notify janitor manager and deactivate.
        /// </summary>
        public void DoneBase()
        {
            returningToBase = false;
            m_janitorManager.DoneWorking( this );
            if(InBase())
                gameObject.SetActive(false);
        }
    }
}
