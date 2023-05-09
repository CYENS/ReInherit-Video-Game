using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Cyens.ReInherit
{
    /// <summary>
    /// Groups together a collection of gameobjects based on a common attribute.
    /// It can be used to group together exhibits, keepers, visitors, etc.
    /// That way we don't need to place those objects directly under their managers.
    /// 
    /// Why not use Tags? 
    /// - We would need to create a 'Collection' tag, which won't be very obvious what its purpose is
    /// - We need Utility functions to create/get these groups of objects anyway.
    /// </summary>
    public class Collection : MonoBehaviour
    {
        
        /// <summary>
        /// A helpful utility method that finds a collection with the given name.
        /// 
        /// This method should be used sparingly! It uses Linq queries, and FindObjectsOfType.
        /// So it shouldn't be used in any critical functions.
        /// </summary>
        /// <param name="name">The gameobject name of that Collection script</param>
        /// <returns>The Collection script if found. Null if otherwise</returns>
        public static Collection Find( string name )
        {
            Collection[] collections = FindObjectsOfType<Collection>();
            Collection candidate = Array.Find(collections, collection => collection.name.Equals(name) );
            return candidate;
        }

        /// <summary>
        /// A helpful utility that creates a new collection with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Collection Create(string name) 
        {
            GameObject temp = new GameObject(name);
            var collection = temp.AddComponent<Collection>();
            return collection;
        }

        /// <summary>
        /// Returns all scripts of the specified type that are contained inside the collection.
        /// (Are children of this game object)
        /// </summary>
        /// <typeparam name="T">The type of script we are interested in</typeparam>
        /// <returns>An array of the requested child scripts</returns>
        public T[] Get<T>() where T : Component 
        {
            var items = gameObject.GetComponentsInChildren<T>(true);
            return items;
        }
        

        /// <summary>
        /// Removes any children gameobjects that do not contain the given script.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Clean<T>() where T : Component 
        {
            List<GameObject> marked = new List<GameObject>();
            foreach( Transform child in transform )
            {
                var item = child.GetComponent<T>();
                if( item == null )
                {
                    // Mark for deletion
                    marked.Add(child.gameObject);
                }
            }
            
            // Mark deleted
            foreach( var target in marked )
            {
                Destroy(target);
            }
        }


    }
}
