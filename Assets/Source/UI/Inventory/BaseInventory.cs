using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cyens.ReInherit
{
    /// <summary>
    /// Contains and manages an inventory of exhibits.
    /// </summary>
    public class BaseInventory : MonoBehaviour
    {

        [Header("References")]

        [SerializeField]
        [Tooltip("The collection to match the inventory items with")]
        private Collection m_collection;        


        [SerializeField]
        [Tooltip("The box that contains all of the UI items")]
        private GridLayoutGroup m_container;

        [SerializeField]
        [Tooltip("Prefab of the item box.")]
        private GameObject m_itemPrefab;

        [SerializeField]
        private MetaData[] m_metaData;
        
        private List<InventoryItem> m_items;


        [Header("Variables")]
        [SerializeField]
        [Tooltip("The currently selected inventory item")]
        protected InventoryItem m_selected;


        private void OnEnable() 
        {
            
            m_collection.hasChanged += Refresh;
        }

        private void OnDisable() 
        {
            m_collection.hasChanged -= Refresh;

            if( m_selected != null )
            {
                m_selected = null;
                OnDeselect();
            }

        }

        protected void Clear()
        {
            Debug.Log("Clear!!");
            for(int i=0; i<m_container.transform.childCount; i++)
            {
                Transform child = m_container.transform.GetChild(i);
                Debug.Log("Destroy gameobject "+child.gameObject);
                Destroy(child.gameObject);
            }
        }

        protected void Refresh()
        {
            Debug.Log("Refresh!!");
            Clear();
            m_metaData = m_collection.Get<MetaData>();

            // Populate grid layout group with items
            m_items = new List<InventoryItem>();
            foreach( var metaData in m_metaData )
            {
                GameObject temp = GameObject.Instantiate(m_itemPrefab, m_container.transform);
                temp.name = metaData.label;
                var item = temp.GetComponent<InventoryItem>();
                item.m_metaData = metaData;
                m_items.Add(item);
            }
        }

        // Start is called before the first frame update
        protected void Start()
        {
            Refresh();
        }

        protected virtual void OnDeselect() 
        {

        }
        protected virtual void OnSelect() 
        {

        }

        /// <summary>
        /// Responds to selection by an inventory item
        /// </summary>
        /// <param name="item"></param>
        internal void Select( InventoryItem item )
        {
            // Deselect item if clicked on again
            if( m_selected == item )
            {
                m_selected = null;
                OnDeselect();
            }
            else 
            {
                m_selected = item;
                OnSelect();
            }
        }

        

        protected void Update() 
        {
            
            // Cancel selection
            if( m_selected != null )
            {
                // Right click
                if( Input.GetMouseButtonDown(1) )
                {
                    m_selected = null;
                    OnDeselect();
                }
            }
        }

    }
}
