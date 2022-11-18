using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    /// <summary>
    /// A script that changes the material of the current object, if it is selected.
    /// If unselected is switches back to the original material.
    /// </summary>
    public class HighlightController : MonoBehaviour
    {
        private bool m_selected = false;

        private Renderer m_renderer;

        
        [SerializeField]
        [Tooltip("The material to swap to if selected")]
        private Material highlightMaterial;

        private Material m_originalMaterial;
        
        
        protected void Awake()
        {
            m_renderer = GetComponent<Renderer>();
            m_originalMaterial = m_renderer.sharedMaterial;
        }

        public void Select()
        {
            if (m_selected) return;
            m_selected = true;
            m_renderer.sharedMaterial = highlightMaterial;
        }

        public void DeSelect()
        {
            if (!m_selected) return;
            m_selected = false;
            m_renderer.sharedMaterial = m_originalMaterial;
        }
        
        private void Update()
        {
            
        }
    }
}
