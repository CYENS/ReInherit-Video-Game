using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.Serialization;

namespace Cyens.ReInherit
{
    /// <summary>
    /// Select Manager is a class for selecting objects containing the 'Selectable' class.
    /// </summary>
    [DefaultExecutionOrder(900)]
    public class SelectManager : InputUser
    {
        
        public static SelectManager singleton;


        [FormerlySerializedAs("m_currentlySelected")] 
        [SerializeField]
        [Tooltip("The currently selected object. Null if none is selected")]
        internal Selectable currentlySelected;
        
        
        private Vector3 m_pointerPos;

        private new Camera m_camera;


        [FormerlySerializedAs("m_layerMask")]
        [SerializeField]
        [Tooltip("What layers should the selector ray be able to hit")]
        // It is internal so that Selectable class has access to this
        internal LayerMask layerMask;
        
        // Start is called before the first frame update
        protected void Awake()
        {
            if (singleton != null) {
                Debug.LogError("There's more than one SelectManager scripts (there should only be one)");    
            }

            base.Awake();
            m_camera = Camera.main;
            singleton = this;
        }

        /// <summary>
        /// Perform check to see if an object or character is selected
        /// </summary>
        /// <param name="context"></param>
        protected override void OnSelect(InputAction.CallbackContext context)
        {
            // Debug.Log("Clicked Left Mouse Button");

            // Create ray
            Vector3 screenPoint = new Vector3(m_pointerPos.x, m_pointerPos.y, m_camera.nearClipPlane);
            Vector3 worldPoint = m_camera.ScreenToWorldPoint(screenPoint);
            Vector3 rayForward = worldPoint - m_camera.transform.position;

            Ray ray = new Ray(worldPoint, rayForward);
            
            // Cast ray
            bool hit = false;
            RaycastHit hitInfo;
            Debug.DrawRay(ray.origin,ray.direction*1000, Color.red, 1.0f);
            if (Physics.Raycast(ray,out hitInfo, 1000, layerMask)) {
                
                GameObject hitTarget = hitInfo.transform.gameObject;
                Selectable selectable = hitTarget.GetComponent<Selectable>();
                if (selectable != null) {
                    
                    currentlySelected = selectable;
                    hit = true;
                    Debug.Log("Selected "+hitTarget);
                }
            }

            if (hit == false && (currentlySelected != null)) {
                Debug.Log("Deselected everything");
                currentlySelected = null;
            }
        }

        /// <summary>
        /// Update pointer position.
        /// </summary>
        /// <param name="context"></param>
        protected override void OnMove(InputAction.CallbackContext context)
        {
            m_pointerPos = context.ReadValue<Vector2>();
            //Debug.Log("Mouse moved at " + _pointerPos);
        }
        
        
        
        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
