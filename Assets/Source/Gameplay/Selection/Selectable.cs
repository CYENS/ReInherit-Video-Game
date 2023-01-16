using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    [DefaultExecutionOrder(1000)]
    public class Selectable : MonoBehaviour
    {
        private SelectManager m_manager;

        private LayerMask m_layerMask;


        private bool m_selected;
        
        
        public bool isSelected => m_selected;


        private HighlightController[] m_highlightControllers;

        
        private bool IsLayerValid( int layer )
        {
            // Only 32 layers are allowed in Unity
            if (layer < 0) return false;
            if (layer >= 32) return false;

            // Perform a quick bit test
            LayerMask newMask = 1 << layer;
            return (m_layerMask & newMask) != 0;
        }


        private void OnDisable() => DeSelect();
        
        private void OnDestroy() => DeSelect();
        

        public void Select()
        {
            m_highlightControllers = GetComponentsInChildren<HighlightController>(true);
            m_selected = true;
            foreach (var highlightController in m_highlightControllers)
            {
                if (highlightController == null) continue;
                highlightController.Select();
            }
        }

        public void DeSelect()
        {
            m_selected = false;
            if (m_highlightControllers == null) return;
            foreach (var highlightController in m_highlightControllers)
            {
                if (highlightController == null) continue;
                highlightController.DeSelect();
            }
        }
        
        
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

        void Awake()
        {
            m_manager = SelectManager.singleton;
            m_layerMask = m_manager.layerMask;
            
            // Automatically pick a layer that will be visible to the 
            // selection system
            PickValidLayer();

            
        }
        
    }
}
