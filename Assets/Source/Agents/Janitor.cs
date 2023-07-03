using System;
using System.Collections;
using System.Collections.Generic;
using Cyens.ReInherit.Managers;
using UnityEngine;

using UnityEngine.Animations.Rigging;
using Cyens.ReInherit.Exhibition;

namespace Cyens.ReInherit
{
    public class Janitor : BaseAgent
    {

        private JanitorManager m_janitorManager;
        private Renderer[] m_renderers;
        [SerializeField] private Task currentTask;
        
        [SerializeField] private GameObject m_RigHands;
        private GameObject m_BoxDissolve;
        private GameObject newBox;
        private float changeTimeDissolve = 1f; // time it takes to change the value
        private float valueDissolve = 0f; // the value to change
        private float changeTimeOpacity = 0.5f;
        private float valueOpacity = 1;

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
        
        public void AddCleanTask( Vector3 garbagePoint )
        {
            Vector3 standPoint = Vector3.zero; // TO-DO: Get garbage position
            Vector3 basePoint = m_janitorManager.GetBasePosition();

            // Go to the GARBAGE
            AddMoveTask( standPoint, 1.0f, 0.1f );

            // Look at the exhibit
            //AddLookTask( exhibit.transform, 10.0f, 1.0f );

            //AddMessageTask( exhibit.gameObject , "Remove" );

            AddWaitTask(1.0f);

            AddMoveTask( basePoint, 0.8f, 0.5f );
            AddMessageTask( gameObject, "Done" );
        }

        /// <summary>
        /// Done with all tasks.
        /// Notify keeper manager and deactivate.
        /// </summary>
        public void Done() 
        {
            m_janitorManager.DoneWorking( this );
            gameObject.SetActive(false);
        }
    }
}
