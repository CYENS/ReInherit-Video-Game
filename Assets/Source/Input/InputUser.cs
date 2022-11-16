using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Cyens.ReInherit
{
    public class InputUser : MonoBehaviour
    {
        
        /// <summary>
        /// Allows this script to implement input events
        /// </summary>
        protected ReInheritControls m_controls;
        
        protected void Awake()
        {
            m_controls = new ReInheritControls();
        }
        
        private void OnEnable()
        {
            m_controls.Enable();
            m_controls.GamePlay.Select.performed += OnSelect;
            m_controls.GamePlay.Point.performed += OnMove;
        }

        private void OnDisable()
        {
            m_controls.Disable();
            m_controls.GamePlay.Select.performed -= OnSelect;
            m_controls.GamePlay.Point.performed -= OnMove;
        }


        /// <summary>
        /// Perform check to see if an object or character is selected
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnSelect(InputAction.CallbackContext context)
        {
            
        }


        /// <summary>
        /// Update pointer position.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnMove(InputAction.CallbackContext context)
        {
            
        }



    }
}
