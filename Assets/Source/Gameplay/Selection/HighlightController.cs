using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    /// <summary>
    /// A script that outlines the current object, if it is selected.
    /// If unselected is switches back with the outliner removed.
    /// </summary>
    public class HighlightController : MonoBehaviour
    {
        private bool m_selected = false;
        private Outline outlineScript;
        [SerializeField] private bool m_isGarbage;
        [SerializeField] private Garbage m_garbage;

        public void Select()
        {
            if (m_selected) return;
            m_selected = true;
            if(m_isGarbage)
                m_garbage.GarbageSelected();
            outlineScript.enabled = true;
        }

        public void DeSelect()
        {
            if (!m_selected) return;
            
            // If the selectable object is a garbage, and already selected,
            // deselect is not allowed.
            if(m_isGarbage)
                return;
                
            m_selected = false;
            outlineScript.enabled = false;
        }

        private void Awake()
        {
            outlineScript = GetComponent<Outline>();
        }

    }
}
