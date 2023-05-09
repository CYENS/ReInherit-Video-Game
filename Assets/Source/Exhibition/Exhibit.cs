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
        internal State m_state = State.Storage;


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
        public GameObject dissolveBox => m_exCase.dissolveBox;


        [SerializeField]
        [Tooltip("Metadata attached to the object")]
        private MetaData m_metaData;


        private Ghostify[] ghostifies;
        private ExhibitVisitorHandler m_visitorHandler;

        public ExhibitVisitorHandler GetVisitorHandler() { return m_visitorHandler; }
        
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

            if( m_metaData == null )
            {
                m_metaData = gameObject.AddComponent<MetaData>();
            }

            if( m_info == null )
            {
                Debug.LogError("Exhibit "+this+" lacks an info object");
                return;
            }
            else 
            {
                m_metaData.icon = m_info.m_thumb;
                m_metaData.label = m_info.name;
                m_metaData.mesh = m_info.m_mesh;

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


        protected void Start() 
        {
            if(TryGetComponent<ExhibitVisitorHandler>(out ExhibitVisitorHandler evh)) {
                m_visitorHandler = evh;
            }
            ghostifies = GetComponentsInChildren<Ghostify>(true);
            SetState(m_state);
        }
        
        public State GetState() => m_state;

        protected void SetGhostState( bool value )
        {
            foreach( var ghost in ghostifies )
            {
                ghost.enabled = value;
            }
        }

        

        public Vector3 ClosestStandPoint(Vector3 point)
        {
            // Get Current Exhibit Case
            var exhibitCase = m_exCase;
            Vector3 closestPoint = exhibitCase.ClosestStandPoint(point);
            return closestPoint;
        }

        public void Install() => SetState( State.Display );

        public void SetState( State state )
        {
            this.m_state = state;
            switch(state)
            {
                case State.Display:
                    gameObject.SetActive(true);
                    m_visitorHandler.GenerateViewPoints();
                    NavMeshManager.Bake();
                    SetGhostState(false);
                break;

                case State.Transit:
                    gameObject.SetActive(true);

                    SetGhostState(true);
                break;

                case State.Storage:
                    gameObject.SetActive(false);

                break;

            }
        }


    }
}
