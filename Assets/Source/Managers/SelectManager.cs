using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.InputSystem.Users;
using UnityEngine.Serialization;
using Cyens.ReInherit.Patterns;


namespace Cyens.ReInherit.Managers
{
    /// <summary>
    /// Select Manager is a class for selecting objects containing the 'Selectable' class.
    /// </summary>
    [DefaultExecutionOrder(900)]
    public class SelectManager : Singleton<SelectManager>
    {
        
        public static SelectManager singleton;


        [FormerlySerializedAs("m_currentlySelected")] 
        [SerializeField]
        [Tooltip("The currently selected object. Null if none is selected")]
        internal Selectable currentlySelected;
        

        protected HashSet<GameObject> selectables;



        private Vector3 m_pointerPos;

        private Camera m_camera;


        [FormerlySerializedAs("m_layerMask")]
        [SerializeField]
        [Tooltip("What layers should the selector ray be able to hit")]
        // It is internal so that Selectable class has access to this
        internal LayerMask layerMask;
        
        // Start is called before the first frame update
        protected void Start()
        {
            m_camera = Camera.main;
            selectables = new HashSet<GameObject>();
        }


        /// <summary>
        /// Casts a ray to see if there's a selectable object in the way
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        protected Selectable CastRay(Ray ray)
        {
            Selectable selectable = null;
            RaycastHit hitInfo;
            Debug.DrawRay(ray.origin,ray.direction*1000, Color.red, 1.0f);
            if (Physics.Raycast(ray, out hitInfo, 1000, layerMask)) {

                GameObject hitTarget = hitInfo.transform.gameObject;
                selectable = hitTarget.GetComponent<Selectable>();
                if (selectable == null) return null;
                if (selectable.enabled == false) return null;
            }
            return selectable;
        }
        
        /// <summary>
        /// Perform check to see if an object or character is selected
        /// </summary>
        /// <param name="context"></param>
        protected void OnClick()
        {
            // Ignore UI Elements (Important!)
            if (GameUI.Instance.isPointerOverUIElement)
                return;


            // Debug.Log("Clicked Left Mouse Button");

            // Create and cast ray
            Vector3 screenPoint = new Vector3(m_pointerPos.x, m_pointerPos.y, m_camera.nearClipPlane);
            Vector3 worldPoint = m_camera.ScreenToWorldPoint(screenPoint);
            Vector3 rayForward = worldPoint - m_camera.transform.position;
            Ray ray = new Ray(worldPoint, rayForward);
            Selectable selectable = CastRay(ray);
            
            // Case: when the ray hits a selectable object
            if (selectable != null) {
                
                if( currentlySelected != null )
                    currentlySelected.DeSelect();

                currentlySelected = selectable;
                currentlySelected.Select();
            }
            // Case: when the ray misses. If there's an already selected object, deselect it
            else if( currentlySelected != null ) {
                currentlySelected.DeSelect();
                currentlySelected = null;
            }

        }
        
        
        // Update is called once per frame
        void Update()
        {
        
            if( Input.GetMouseButtonDown(0) )
            {
                OnClick();
            }
            m_pointerPos = Input.mousePosition;
        }
    }
}
