using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Patterns;

namespace Cyens.ReInherit
{
    /// <summary>
    /// A type of singleton which manages a collection of objects.
    /// - Ensures that collections items are of the correct type
    /// - Adds/Removes/Queries collection items 
    /// - A Utility for external scripts to interact with the collection items
    /// 
    /// Generic Constraints:
    /// - S: Should be the same as the class that will inherit CollectionManager.
    ///     This is to allow the Singleton pattern to work correctly. 
    /// - T: The type of items of the collection
    /// </summary>
    public class CollectionManager<S,T> : Singleton<S> 
        where T : Component
        where S : Component
    {
        
        [Header("Collection")]
        
        [SerializeField]
        [Tooltip("The name of the collection gameObject")]
        protected string m_collectionName;

        [SerializeField]
        [Tooltip("The collection object")]
        protected Collection m_collection;


        [SerializeField]
        [Tooltip("Collection items")]
        protected List<T> m_items;


        protected void Init()
        {
            if( m_collection != null )
            {
                return;
            }

            // Ensures that we have a collection with the right name
            m_collection = Collection.Find(m_collectionName);
            if( m_collection == null )
            {
                m_collection = Collection.Create(m_collectionName);
            } 
        }
        protected void Refresh()
        {
            // Clean out anything else that shouldn't belong in the collection
            m_collection.Clean<T>();

            m_items.Clear();

            // Grabs a list of the items
            var items = m_collection.Get<T>();
            if(m_items == null)
            {
                // Ensure that our list isn't null
                m_items = new List<T>();
            }
            m_items.AddRange( items );
        }

        private void OnEnable() 
        {
            Init();
            m_collection.hasChanged += Refresh; 
        }

        private void OnDisable() 
        {
            m_collection.hasChanged -= Refresh; 
        }

        /// <summary>
        /// Regular setup function.
        /// Please call base.Start() if you wish to replace this function
        /// with inheritance.
        /// </summary>
        protected void Start() 
        {
            Init();
            Refresh();
        }


        protected void Update() 
        {

        }

    }
}
