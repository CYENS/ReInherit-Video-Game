using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Patterns;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cyens.ReInherit.Managers
{
    /// <summary>
    /// Responsible for managing the events that are shown to the players after the current round ends.
    /// </summary>
    public class GameEventsManager : Singleton<GameEventsManager>
    {

        [Header("Assets")]

        [Tooltip("An asset library containing the categories")]
        [SerializeField] private AssetLibrary m_categoryLibrary;

        #if UNITY_EDITOR
        [Tooltip("Forces a reload of the assets in the editor")]
        [SerializeField] private bool m_reload;
        #endif

        [SerializeField] private EventCategory[] m_categories;


        [SerializeField] private List<GameEvent> m_gameEvents;

        [Header("Parameters")]
        [SerializeField] private int m_index = 0;


        [Header("Reference")]

        [SerializeField] private string m_uiGroupLabel = "GaveEventUI";
        [SerializeField] private GameObject m_uiGroup;

        private GameManager m_gameManager;


        protected void OnRoundEnd(int round)
        {
            int index = round-1;
            Debug.Log("Round End: "+index);
            if( index < 0 || index >= m_gameEvents.Count || m_uiGroup == null )
            {
                return;
            }

            GameEvent next = m_gameEvents[index];
            
            // (TODO) Evaluate conditions


            GameObject prefab = (GameObject)next.whenPass;
            if( prefab != null )
            {
                GameObject ui = Instantiate(prefab, m_uiGroup.transform );
            }
            
        }

        private void OnEnable() 
        {
            if( m_gameManager == null )
            {
                m_gameManager = GameManager.Instance;
            }
            Debug.Log("When Round Ends");
            m_gameManager.whenRoundEnds += OnRoundEnd;
        }

        private void OnDisable() 
        {
            if( m_gameManager == null )
            {
                m_gameManager = GameManager.Instance;
            }
            m_gameManager.whenRoundEnds -= OnRoundEnd;
        }

        protected void Start()
        {
            m_categories = m_categoryLibrary.GetAssets<EventCategory>();
            foreach( var category in m_categories )
            {
                category.Init();
            }

            // Create a shuffled list of game events
            m_gameEvents = new List<GameEvent>();
            for( int i=0; i<6; i++ )
            {
                var group = GetEventGroup();
                ShuffleGroup(ref group);
                m_gameEvents.AddRange( group );
            }

            // Get Reference to the UI component
            m_uiGroup = Reference.Find(m_uiGroupLabel);

            // Get reference to game mananger
            m_gameManager = GameManager.Instance;
        }

        #if UNITY_EDITOR
        private void OnValidate() 
        {
            if( m_reload )
            {
                m_reload = false;
                m_categories = m_categoryLibrary.GetAssets<EventCategory>();
            }
        }
        #endif


        protected GameEvent[] GetEventGroup()
        {   
            List<GameEvent> group = new List<GameEvent>();

            for( int i=0; i<m_categories.Length; i++ )
            {
                var category = m_categories[i];

                GameEvent gameEvent = category.Next();
                if( gameEvent != null )
                {
                    group.Add(gameEvent);
                }
            }

            return group.ToArray();
        }



        protected void ShuffleGroup( ref GameEvent[] array)
        {
            // Using Knuth's shuffling algo
            for( int i=0; i<array.Length; i++ )
            {
                var temp = array[i];
                int r = Random.Range(i, array.Length);
                array[i] = array[r];
                array[r] = temp;
            }
        }
    }
}
