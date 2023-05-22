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



        protected void Start()
        {
            m_categories = m_categoryLibrary.GetAssets<EventCategory>();
            foreach( var category in m_categories )
            {
                category.Init();
            }


            m_gameEvents = new List<GameEvent>();
            for( int i=0; i<6; i++ )
            {
                var group = GetEventGroup();
                ShuffleGroup(ref group);
                m_gameEvents.AddRange( group );
            }
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


        /*
        [Header("Assets")]

        [SerializeField]
        private AssetLibrary m_assetLibrary;

        #if UNITY_EDITOR
        [Tooltip("Forces a reload of the assets in the editor")]
        [SerializeField] private bool m_reload;
        #endif


        [SerializeField]
        private RoundEndEvent[] m_allEvents;

        
        [SerializeField]
        private List<RoundEndEvent.Types> m_typeQueue;

        [SerializeField]
        private List<RoundEndEvent> m_eventQueue;


        //[SerializeField]
        //private List<>
        


        protected void ShuffleTypesArray( ref RoundEndEvent.Types[] array)
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

        protected void GenerateTypeQueue()
        {
            m_typeQueue = new List<RoundEndEvent.Types>();

            // Repeating the process 6 times is more than enough
            for( int i=0; i<6; i++ )
            {

            }

            // Add the items
            

            // Mix the types up
        }

        protected void Start()
        {
            m_allEvents = m_assetLibrary.GetAssets<RoundEndEvent>();



            m_eventQueue = new List<RoundEndEvent>();
        }

        
        #if UNITY_EDITOR
        private void OnValidate() 
        {
            if( m_reload )
            {
                m_reload = false;
                m_allEvents = m_assetLibrary.GetAssets<RoundEndEvent>();
            }
        }
        #endif

        */
    }
}
