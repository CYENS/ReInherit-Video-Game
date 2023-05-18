using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Cyens.ReInherit
{
    [CreateAssetMenu(fileName = "PrefabsLibraryObject", menuName = "ScriptableObjects/PrefabsLibrary", order = 1)]
    public class PrefabsLibrary : ScriptableObject
    {
        [SerializeField] private bool m_refreshPrefabs = false;
        [SerializeField] private string[] m_directories;
        [SerializeField] private string[] m_objectFilters;
        [Serializable] public class PrefabsDictionary : SerializableDictionary<string, GameObject> {};
        [SerializeField] private PrefabsDictionary m_prefabs;


        public void AddPrefab(string name, GameObject prefab) { m_prefabs.Add(name, prefab); }
        public GameObject GetPrefab(string name) { return m_prefabs[name]; }
        public bool SearchPrefab(string name) { return m_prefabs.ContainsKey(name); }


        #if UNITY_EDITOR
        private void LoadAllAssets()
        {
            string types = "";
            foreach (string type in m_objectFilters)
                types += type + " ";
            
            string[] guids = AssetDatabase.FindAssets( types.Remove(types.Length - 1, 1), m_directories);

            foreach (var guid in guids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                m_prefabs.Add(go.name, go);
            }
        }
        

        private void Awake()
        {
            m_prefabs = new PrefabsDictionary();
            m_directories = new[] { "Assets/Prefabs" };
            m_objectFilters = new[] { "t:Prefab" };
            LoadAllAssets();
        }
        

        private void OnValidate()
        {
            if (m_refreshPrefabs) {
                m_refreshPrefabs = false;
                m_prefabs.Clear();
                LoadAllAssets();
            }
        }
        #endif
    }
}
