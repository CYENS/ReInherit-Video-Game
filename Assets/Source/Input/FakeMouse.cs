using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Cyens.ReInherit
{
    public class FakeMouse : InputUser
    {
    
        private Image m_image;
        private float m_timer = 0.0f;

        [SerializeField] private Sprite spriteDefault;
        [SerializeField] private Sprite spriteLeftMouse;
        [SerializeField] private Sprite spriteRightMouse;


        protected new void Awake()
        {
            base.Awake();
            m_image = GetComponent<Image>();
            
        }
        
        protected override void OnSelect(InputAction.CallbackContext context)
        {
            m_image.sprite = spriteLeftMouse;
            m_timer = 0.5f;
        }
        protected override void OnTarget(InputAction.CallbackContext context)
        {
            m_image.sprite = spriteRightMouse;
            m_timer = 0.5f;
        }
        
        private void Update()
        {
            m_timer = Mathf.Max(m_timer - Time.deltaTime, 0.0f);
            if (m_timer < float.Epsilon) m_image.sprite = spriteDefault;
        }
    }
}
