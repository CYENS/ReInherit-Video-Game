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


        private bool isSelected => this == m_manager.currentlySelected;


        [SerializeField] private GameObject white;
        [SerializeField] private GameObject green;

        
        private bool IsLayerValid( int layer )
        {
            // Only 32 layers are allowed in Unity
            if (layer < 0) return false;
            if (layer >= 32) return false;

            // Perform a quick bit test
            LayerMask newMask = 1 << layer;
            return (m_layerMask & newMask) != 0;
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

        private void Update()
        {
            white.SetActive( isSelected == false );
            green.SetActive( isSelected == true );
        }
    }
}
