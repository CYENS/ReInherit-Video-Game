using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace Cyens.ReInherit
{
    /// <summary>
    /// A data object that finds and groups together a collection of assets during development.
    /// During gameplay it is a simple asset group that can be used and referenced by other scripts.
    /// </summary>
    [CreateAssetMenu(fileName = "AssetLibrary", menuName = "Data/AssetLibrary", order = 1)]
    public class AssetLibrary : ScriptableObject
    {


        [Tooltip("Forces a refresh")]
        [SerializeField] private bool m_refresh = false;


        [Header("Parameters")]

        [Tooltip("A simple type filter. Enter the script name, or the asset type here.")]
        [SerializeField] private string m_filter = "t:Prefab";



        [Serializable] public class PrefabsDictionary : SerializableDictionary<string, UnityEngine.Object> {};

        [Header("Results")]
        
        [Tooltip("The list of collected assets")]
        [SerializeField] private List<UnityEngine.Object> m_assets;

        //[Tooltip("Runtime dictionary for quick name based lookup")]
        //private Dictionary<string, UnityEngine.Object> m_lookup;


        //public void AddPrefab(string name, UnityEngine.Object prefab) { m_prefabs.Add(name, prefab); }
        //public UnityEngine.Object GetPrefab(string name) { return m_prefabs[name]; }
        //public bool SearchPrefab(string name) { return m_prefabs.ContainsKey(name); }

        public T[] GetAssets<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();

            foreach( var asset in m_assets )
            {
                if( asset == null )
                {
                    continue;
                }
                if( asset is T )
                {
                    assets.Add(asset as T);
                }
            }
            return assets.ToArray();
        }

        public T[] GetComponents<T>() where T : Component 
        {
            List<T> components = new List<T>();
            foreach( var asset in m_assets )
            {
                if(asset == null)
                {
                    continue;
                }

                // Check if prefab
                if( asset is GameObject )
                {
                    GameObject go = asset as GameObject;

                    // Check if that prefab has the desired component
                    T component = go.GetComponent<T>();
                    if( component != null )
                    {
                        components.Add(component);
                    }
                }
            }

            return components.ToArray();
        }


        #if UNITY_EDITOR

        private string GetDirectory()
        {
            string assetPath = AssetDatabase.GetAssetPath( this.GetInstanceID() );
            string directoryPath = Path.GetDirectoryName(assetPath);
            Debug.Log("My Asset PathL: "+directoryPath);
            return directoryPath;
        }

        private void LoadAllAssets()
        {
            // Get directory of the library asset to use it as a base
            string rootPath = GetDirectory();
            string[] directories = new string[1]{rootPath};

            // Find ids of all assets that meet the filter conditions
            string[] guids = AssetDatabase.FindAssets( m_filter, directories);

            // Add assets to list
            foreach (var guid in guids) 
            {
                // TODO: Do NOT include self!
                var path = AssetDatabase.GUIDToAssetPath(guid);
                Debug.Log("path: "+path);
                UnityEngine.Object go = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                m_assets.Add(go);
            }
        }
        

        private void Awake()
        {
            Clear();
            LoadAllAssets();
        }
        
        /// <summary>
        /// Clear the prefab dictionary if already initialized.
        /// Otherwise initialize.
        /// </summary>
        private void Clear()
        {
            if( m_assets == null )
            {
                m_assets = new List<UnityEngine.Object>();
                return;
            }

            m_assets.Clear();
        }

        /// <summary>
        /// Reloads all assets when the 'refresh' boolean flag is clicked.
        /// It also automatically reloads assets when the scripts compile.
        /// </summary>
        private void OnValidate()
        {
            if (m_refresh) 
            {
                m_refresh = false;
                Clear();
                LoadAllAssets();
            }
        }
        #endif
    }
}
