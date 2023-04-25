using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Managers;

namespace Cyens.ReInherit
{

    /// <summary>
    /// Objects that react to being selected.
    /// </summary>
    [DefaultExecutionOrder(1000)]
    public class Selectable : MonoBehaviour
    {
        private LayerMask m_layerMask;

        [SerializeField]
        [Tooltip("Whether this object is selected or not")]
        private bool m_selected;
        
        private bool m_prevSelected;


        public bool isSelected => m_selected;


        private HighlightController[] m_highlightControllers;


        [Header("References")]
        [SerializeField]
        private GameObject uiContent;
        


        private void OnDisable() => DeSelect();
        
        private void OnDestroy() => DeSelect();
        

    
        void Awake()
        {
            m_layerMask = SelectManager.Layers;
            
            // Automatically pick a layer that will be visible to the 
            // selection system
            PickValidLayer();

            // Deselect by default on start
            DeSelect();
        }

        /// <summary>
        /// Forces the object to be de-selected
        /// </summary>
        private void DeSelect() 
        {
            if(SelectManager.Selected == gameObject)
            {
                SelectManager.Clear();
            }

            m_prevSelected = false;
            m_selected = false;

            Refresh();
        }

        /// <summary>
        /// Checks to see if the object's layer matches
        /// the internal layer mask (obtained by SelectManager)
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        private bool IsLayerValid( int layer )
        {
            // Only 32 layers are allowed in Unity
            if (layer < 0) return false;
            if (layer >= 32) return false;

            // Perform a quick bit test
            LayerMask newMask = 1 << layer;
            return (m_layerMask & newMask) != 0;
        }


        /// <summary>
        /// Ensures that the object is on a layer that the 
        /// SelectManager will be able to detect
        /// </summary>
        private void PickValidLayer()
        {
            // Current layer is already valid
            if (IsLayerValid(gameObject.layer)) return;

            // Find the first valid layer
            for (int layer = 0; layer < 32; layer++) {
                if (IsLayerValid(layer)) {
                    gameObject.layer = layer;
                    return;
                }
            }
            
            // No valid layer!
        }

        /// <summary>
        /// Refreshes all the UI elements and effects based on the selection state
        /// of the object.
        /// </summary>
        private void Refresh() 
        {
            // Refresh highlight controllers
            m_highlightControllers = GetComponentsInChildren<HighlightController>(true);
            foreach (var highlightController in m_highlightControllers)
            {
                if (highlightController == null) continue;
                if( m_selected )
                {
                    highlightController.Select();
                }
                else 
                {
                    highlightController.DeSelect();
                }
            }

            // Refresh visibility of UI element
            if (uiContent != null)
            {
                uiContent.SetActive(m_selected);
            }
        }


        private void Update() 
        {
            m_selected = (SelectManager.Selected == gameObject);

            // Detect changes
            if( m_prevSelected != m_selected )
            {
                m_prevSelected = m_selected;
                Refresh();
            }
        }
        
    }
}
