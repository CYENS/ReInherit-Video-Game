using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Cyens.ReInherit
{
    /// <summary>
    /// Moves around a sprite to represent the mouse position during gameplay.
    /// It also shows when the mouse buttons were pressed.
    /// Useful for demonstrations;
    /// </summary>
    public class FakeCursor : InputUser
    {
   
        private Image m_image;
        private RectTransform m_rectTransform;
        
        protected new void Awake()
        {
            base.Awake();
            m_image = GetComponent<Image>();
            m_rectTransform = GetComponent<RectTransform>();
        }
        

        /// <summary>
        /// Update fake cursor position.
        /// </summary>
        /// <param name="context"></param>
        protected override void OnMove(InputAction.CallbackContext context)
        {
            Vector2 mousePos = context.ReadValue<Vector2>();
            m_rectTransform.position = new Vector3(mousePos.x, mousePos.y,0);
        }
        
    }
}
