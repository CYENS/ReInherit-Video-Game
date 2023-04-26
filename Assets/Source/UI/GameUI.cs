using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Managers;
using UnityEngine.EventSystems;
using Cyens.ReInherit.Patterns;

namespace Cyens.ReInherit
{
    /// <summary>
    /// This script is responsible for enabling and disabling UI elements,
    /// based on the game's state
    /// </summary>
    public class GameUI : Singleton<GameUI>
    {

        [Header("References")]

        [Tooltip("Reference to the canvas/object containing the UI elements of the closed museum")]
        [SerializeField]
        private GameObject m_whenClosed;

        [Tooltip("Reference to the canvas/object containing the UI elements of the closed museum")]
        [SerializeField]
        private GameObject m_whenOpen;
        

        [SerializeField]
        [Tooltip("Reference to the current event system (will be set on Awake)")]
        private EventSystem eventSys;


        [Header("Variables")]
        [SerializeField]
        [Tooltip("Whether the mouse is on top of a (raycast blocking) UI element")]
        private bool m_pointerOverUIElement;
        public bool isPointerOverUIElement => m_pointerOverUIElement; 
        


        void Start() 
        {
            if( eventSys == null )
            {
                eventSys = EventSystem.current;
            }

            
        }

        void Update()
        {
            bool museumOpen = GameManager.IsMuseumOpen;
            m_whenClosed.SetActive( !museumOpen );
            m_whenOpen.SetActive( museumOpen );

            // Check if mouse is over a raycast blocking canvas element
            m_pointerOverUIElement = eventSys.IsPointerOverGameObject();
        }
    }
}
