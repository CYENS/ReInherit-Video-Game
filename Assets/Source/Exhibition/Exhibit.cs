using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Managers;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cyens.ReInherit.Exhibition
{
    public class Exhibit : MonoBehaviour
    {
        /// <summary>
        /// Various states of being for the exhibit.
        /// Storage: The artifact is in the museum storage for safe keeping
        /// Transit: The exhibit is currently being set up, and
        /// </summary>
        public enum State {Storage=0, Transit=1, Display=2, Restoration=3}





        [Header("Parameters")]

        [SerializeField]
        [Tooltip("The information associated with this exhibit")]
        private ExhibitInfo m_info;


        [Header("Gameplay")]

        [SerializeField]
        [Tooltip("In what state is the exhibit currently in")]
        private State m_state = State.Storage;


        [SerializeField]
        [Tooltip("The selected exhibit case")]
        [Range(0,5)]
        private int m_upgrade = 0;


        [Header("References")]
        [SerializeField]
        [Tooltip("References to exhibit cases")]
        private ExhibitCase[] m_exCases;

        [SerializeField]
        [Tooltip("The selected exhibit case")]
        private ExhibitCase m_exCase;


        #if UNITY_EDITOR

        /// <summary>
        /// Editor only utilities that help with the fixing of prefabs.
        /// </summary>
        [Header("Prefab")]
        [SerializeField]
        [Tooltip("")]
        private bool m_fixModel = false;

        //[SerializeField]
        private bool m_isPrefab;
        //[SerializeField]
        private PrefabInstanceStatus m_instancePrefabStatus;
        //[SerializeField]
        private bool m_isAsset;
        //[SerializeField]
        private UnityEngine.GameObject m_source;
        //[SerializeField]
        private string m_path;

        private void OnValidate() 
        {
            if( Application.isPlaying )
            {
                // Safeguard. This is meant to validate the exhibit during editing
                return;
            }

            m_exCases = GetComponentsInChildren<ExhibitCase>(true);
            
            // Ensure that the upgrade index does not exceed number of exhibit cases
            m_upgrade = Mathf.Clamp(m_upgrade, 0, m_exCases.Length-1 );

            // Throw a warning if there are no glass cases embedded    
            if( m_exCases.Length == 0 )
            {
                Debug.LogWarning("Exhibit "+this+" does not contain any exhibit cases!");
            }
            else 
            {
                // Select one exhibit case
                m_exCase = m_exCases[m_upgrade];
            }

            // Deactivate all exhibit cases. Keep only the selected one active
            for( int i=0; i<m_exCases.Length; i++ )
            {
                m_exCases[i].enabled = (i==m_upgrade);
            }


            if( m_info == null )
            {
                Debug.LogError("Exhibit "+this+" lacks an info object");
                return;
            }

            m_isPrefab = PrefabUtility.IsPartOfAnyPrefab(this);
            m_instancePrefabStatus = PrefabUtility.GetPrefabInstanceStatus(gameObject);
            m_isAsset = m_instancePrefabStatus == PrefabInstanceStatus.NotAPrefab;
            m_source = PrefabUtility.GetCorrespondingObjectFromOriginalSource(gameObject);
            m_path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);

            if( m_fixModel == false )
            {
                return;
            }
            m_fixModel = false;

            
            if( m_isPrefab == false )
            {
                // Safeguard. Only fix prefab instances.
                return;
            }


            if( m_isAsset ) 
            {
                // Safeguard. Don't mess with prefab assets. They are hard to edit programmatically
                return;
            }

            


            // Ensure that prefabs all have the correct artifact placed inside
            // Don't do this while the game is playing though
            if( m_info != null && m_info.m_artifact != null )
            {
                for( int i=0; i<m_exCases.Length; i++ )
                {  
                    m_exCases[i].AddArtifact(m_info.m_artifact, true);
                }
            }

            StartCoroutine(SavePrefab());
        }

        /// <summary>
        /// A coroutine meant to apply changes to the prefab.
        /// This is necessary because we aren't allowed to save the prefab during OnValidate.
        /// </summary>
        /// <returns></returns>
        IEnumerator SavePrefab( )
        {
            yield return new WaitForSeconds(0.01f);
            PrefabUtility.ApplyPrefabInstance(gameObject,InteractionMode.UserAction);
            yield return new WaitForSeconds(0.1f);
        }
        
        #endif


    }
}