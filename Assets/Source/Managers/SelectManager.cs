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
    /// Select Manager is a class for selecting objects.
    /// </summary>
    [DefaultExecutionOrder(900)]
    public class SelectManager : Singleton<SelectManager>
    {

        [FormerlySerializedAs("m_currentlySelected")] 
        [SerializeField]
        [Tooltip("The currently selected object. Null if none is selected")]
        protected GameObject m_selected;
        public static GameObject Selected => Instance.m_selected;
        
        private Vector3 m_pointerPos;
        private Camera m_camera;


        [FormerlySerializedAs("m_layerMask")]
        [SerializeField]
        [Tooltip("What layers should the selector ray be able to hit")]
        protected LayerMask m_layers;
        public static LayerMask Layers => Instance.m_layers;


        [Header("Debug")]
        [SerializeField] 
        [Tooltip("Display rays")]
        private bool m_debugRays;


        // Start is called before the first frame update
        protected void Start()
        {
            m_camera = Camera.main;
        }

        public static void Clear()
        {
            Instance.m_selected = null;
        }

        /// <summary>
        /// Casts a ray to see if there's a selectable object in the way
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        protected GameObject CastRay(Ray ray)
        {
            RaycastHit hitInfo;

            #if UNITY_EDITOR
            if(m_debugRays)
            {
                Debug.DrawRay(ray.origin,ray.direction*1000, Color.red, 1.0f);
            }
            #endif
            
            if (Physics.Raycast(ray, out hitInfo, 1000, m_layers)) 
            {
                GameObject hitTarget = hitInfo.transform.gameObject;
                return hitTarget;
            }
            return null;
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

            // Create and cast ray
            Vector3 screenPoint = new Vector3(m_pointerPos.x, m_pointerPos.y, m_camera.nearClipPlane);
            Vector3 worldPoint = m_camera.ScreenToWorldPoint(screenPoint);
            Vector3 rayForward = worldPoint - m_camera.transform.position;
            Ray ray = new Ray(worldPoint, rayForward);
            m_selected = CastRay(ray);
        }
        
        
        // Update is called once per frame
        void Update()
        {
            // Don't allow selection of objects while we are placing objects
            bool allowSelection = true;
            allowSelection &= !PlacementManager.IsActive();
            allowSelection &= !PreviewManager.IsActive();

            bool hasClicked = Input.GetMouseButtonDown(0);

            if( hasClicked && allowSelection )
            {
                OnClick();
            }
            m_pointerPos = Input.mousePosition;
            
        }
    }
}
