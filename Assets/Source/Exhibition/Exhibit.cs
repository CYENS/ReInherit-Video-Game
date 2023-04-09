using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Managers;

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
        private void OnValidate() 
        {
            m_exCases = GetComponentsInChildren<ExhibitCase>(true);

            // Ensure that the upgrade index does not exceed number of exhibit cases
            m_upgrade = Mathf.Clamp(m_upgrade, 0, m_exCases.Length );

            // Ensure that all glass cases have an artifact inside of them

            // Throw a warning if there are no glass cases embedded    
            if( m_exCases.Length == 0 )
            {
                Debug.LogWarning("Exhibit "+this+" does not contain any exhibit cases!");
            }
            else 
            {
                m_exCase = m_exCases[m_upgrade];
            }
            
            // Deactivate all exhibit cases. Keep only the selected one active
            for( int i=0; i<m_exCases.Length; i++ )
            {
                m_exCases[i].enabled = (i==m_upgrade);
            }

        }
        #endif


    }
}
