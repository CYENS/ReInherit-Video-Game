using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Managers;
using Cyens.ReInherit.Exhibition;

namespace Cyens.ReInherit
{
    public class Dialog : MonoBehaviour
    {

        private StageManager m_stageManager;
        private ExhibitManager m_exhibitManager;


        private bool m_active;

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

        public void AddNextArtifact()
        {
            m_exhibitManager.AddNext();
            m_exhibitManager.Advance();
        }

        public void RemoveCredibility(float rating )
        {
            GameManager.Rating += rating;
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
