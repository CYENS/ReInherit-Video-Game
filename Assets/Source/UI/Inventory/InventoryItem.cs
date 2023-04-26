using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cyens.ReInherit
{
    public class InventoryItem : MonoBehaviour
    {

        
        [Tooltip("Metadata")]
        internal MetaData m_metaData;

        [Tooltip("The parent inventory object")]
        internal BaseInventory m_inventory;

        [Header("References")]
        [SerializeField]
        [Tooltip("Reference to the icon image component")]
        private Image m_iconImage;


        // Start is called before the first frame update
        void Start()
        {
            m_inventory = GetComponentInParent<BaseInventory>();

            name = m_metaData.label;
            m_iconImage.sprite = m_metaData.icon;
        }

        public void Select() 
        {
            // Notify parent that the icon was selected
            m_inventory.Select( this );
        }

    }
}