using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Cyens.ReInherit
{
    public class UnitController : InputUser
    {
        private BaseUnit m_unit;

        private Selectable m_selectable;

        private Camera m_camera;

        private Vector3 m_pointerPos;
        
        [SerializeField]
        private GameObject wayPointPrefab;

    
        
        [SerializeField] 
        [Tooltip("Which layers block rays")]
        private LayerMask layerMask;


        [SerializeField] 
        [Tooltip("Layers for which we create a waypoint when interacting with them")]
        private LayerMask navigateMask;
        
        
        protected struct HitInfo
        {
            public Vector3 point;
            public GameObject target;
        }
        
        protected new void Awake()
        {
            base.Awake();
            m_unit = GetComponent<BaseUnit>();
            m_selectable = GetComponent<Selectable>();
            m_camera = Camera.main;
        }
        
        
        
        
        protected HitInfo CastRay(Ray ray)
        {
      
            HitInfo hitInfo;
            hitInfo.point = Vector3.zero;
            hitInfo.target = null;
            
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, 1000, layerMask)) {

                hitInfo.target = raycastHit.transform.gameObject;
                hitInfo.point = raycastHit.point;
                
            }
            
            return hitInfo;
        }
        
        
        /// <summary>
        /// Raycast to check where to move, or what action to perform (and on which object/unit)
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override void OnTarget(InputAction.CallbackContext context)
        {
            if (m_selectable == null)
                return;
            
            // Do not set waypoints if the current unit isn't selected
            if (m_selectable.isSelected == false)
                return;
            
            // Create and cast ray
            Vector3 screenPoint = new Vector3(m_pointerPos.x, m_pointerPos.y, m_camera.nearClipPlane);
            Vector3 worldPoint = m_camera.ScreenToWorldPoint(screenPoint);
            Vector3 rayForward = worldPoint - m_camera.transform.position;
            Ray ray = new Ray(worldPoint, rayForward);

            HitInfo hitInfo = CastRay(ray);

            if (hitInfo.target != null) {

                GameObject target = hitInfo.target;
                bool createWayPoint = (target.layer & navigateMask) != 0;
                
                if (createWayPoint) {
                    GameObject wp = Instantiate(wayPointPrefab, hitInfo.point, Quaternion.identity);
                    m_unit.SetTarget(wp);
                }
                else {
                    m_unit.SetTarget(target);
                }
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
        
    }
}
