using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cyens.ReInherit
{
    /// <summary>
    /// A structure used to store a list of all artifact infos,
    /// so that they can be associated with an id and referenced in-game.
    /// 
    /// It has an editor-only feature (AssetDatabase) which allows it to
    /// automatically search and find all the artifact infos.
    /// </summary>
    [CreateAssetMenu(fileName = "ArtifactInfoLibrary", menuName = "Artifact/Library", order = 1)]
    [DefaultExecutionOrder(-100000)]
    public class ArtifactInfoLibrary : ScriptableObject
    {
        // A way for objects to reference this object and use it as a utility
        public static ArtifactInfoLibrary singleton;


        [SerializeField]
        private List<ArtifactInfo> m_listing;

        private Dictionary<string,ArtifactInfo> m_lookup;

        // Reverse lookup. Find the artifact instance id from the artifact
        private Dictionary<ArtifactInfo,string> m_revlookup;

        
        private void SetSingleton()
        {
            if(singleton==null) singleton = this;
            else
            {
                if(singleton != this)
                    Debug.LogError("! There are more than one ArtifactInfoLibrary objects! There should only be one!");
            }
        }

        private void BuildLookups()
        {
            m_lookup = new Dictionary<string, ArtifactInfo>();
            m_revlookup = new Dictionary<ArtifactInfo, string>();
            foreach( var info in m_listing )
            {
                m_lookup.Add( info.myid, info );
                m_revlookup.Add( info, info.myid );
            }
        }


        // Run this code on startup
        private void Awake() 
        {
            Debug.Log("ArtifactInfoLibrary is waking up");
            SetSingleton();
            BuildLookups();
        }

        /// <summary>
        /// Finds an ArtifactInfo asset (scriptableObject)
        /// via its unique id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ArtifactInfo Find( string id )
        {
            if( singleton == null )
            {
                Debug.LogError("!ArtifactInfoLibrary is not ready yet!");
                return null;
            }

            ArtifactInfo info;
            var lookup = singleton.m_lookup;
            if( lookup == null )
            {
                Debug.LogError("! ArtifactInfoLibrary Lookup Table is not ready !");
                return null;  
            }

            if( !lookup.TryGetValue(id, out info) )
            {
                Debug.LogError("! No ArtifactInfo exists with given id '"+id+"' !");
                return null;
            }

            return info;
        }


        #if UNITY_EDITOR
        private void OnValidate() 
        {
            SetSingleton();
            
            // Look for assets of type InfoLibrary
            m_listing = new List<ArtifactInfo>();
            var guids = AssetDatabase.FindAssets("t:ArtifactInfo");
            foreach( var guid in guids )
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                ArtifactInfo info = AssetDatabase.LoadAssetAtPath<ArtifactInfo>(path);
                m_listing.Add(info);
            }


                
            // string[] guids = AssetDatabase.FindAssets( types.Remove(types.Length - 1, 1), m_directories);

            // foreach (var guid in guids) {
            //     var path = AssetDatabase.GUIDToAssetPath(guid);
            //     GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            //     m_prefabs.Add(go.name, go);
            // }
        }
        #endif





    }
}
