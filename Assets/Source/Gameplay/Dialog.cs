using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Managers;
using Cyens.ReInherit.Exhibition;

namespace Cyens.ReInherit
{
    /// <summary>
    /// A class that helps create dialog sequences / events in-game.
    /// It contains various helper functions that affect game statistics
    /// (funds, rating, etc) as well as means to advance the dialog
    /// </summary>
    public class Dialog : MonoBehaviour
    {

        private StageManager m_stageManager;
        private ExhibitManager m_exhibitManager;

        private bool m_active;


        [SerializeField]
        [Tooltip("The main dialog presented")]
        private GameObject m_first;

        [SerializeField]
        [Tooltip("The dialog to present when accepting")]
        private GameObject m_accept;

        [SerializeField]
        [Tooltip("The dialog to present when declining")]
        private GameObject m_decline;


        // Start is called before the first frame update
        void Start()
        {
            m_stageManager = StageManager.Instance;
            m_exhibitManager = ExhibitManager.Instance;
            m_active = true;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void Remove()
        {
            if( m_active == false )
            {
                return;
            }
            m_active = false;

            Destroy(gameObject,1.0f);
            //TODO: Give enough time to play some animation e.x fadeout
        }

        public void Close() 
        {
            Destroy(gameObject);
        }

        public void Accept() 
        {
            m_first.SetActive(false);
            m_accept.SetActive(true);
        }

        public void Decline()
        {
            m_first.SetActive(false);
            m_decline.SetActive(true);
        }

        public void AddNextArtifact()
        {
            m_exhibitManager.AddNext();
            m_exhibitManager.Advance();
        }

        public void RemoveCredibility(float rating )
        {
            GameManager.Rating += rating;
        }

        public void Award( int amount ) 
        {
            GameManager.Funds += amount;
        }

        public void ExpandMuseum()
        {
            if( m_active )
            {
                m_stageManager.IncreaseSize();
            }
        } 

    }
}
