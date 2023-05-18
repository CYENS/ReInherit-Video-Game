using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Architect;

namespace Cyens.ReInherit.Managers
{
    /// <summary>
    /// Contains a collection of stage areas (blocks that are you allowed to build inside)
    /// These stage areas can be switched between.
    /// </summary>
    public class StageManager : CollectionManager<StageManager, Stage>
    {

        [Header("References")]
        
        [SerializeField]
        private RoomGraph m_roomGraph;
        public RoomGraph roomGraph => m_roomGraph;


        [Header("Parameters")]

        [SerializeField]
        private int m_select = 0;

        protected Stage[] m_stages;


        public void Select(int index) 
        {
            m_select = index;

            foreach(var stage in m_stages)
            {
                stage.gameObject.SetActive(false);
            }
            m_stages[index].gameObject.SetActive(true);
        }

  
        #if UNITY_EDITOR
        private void OnValidate() 
        {
            if( Application.isPlaying == false )
            {
                return;
            }    
            if( gameObject.scene.name == null )
            {
                return;
            }

            Select(m_select);
        }
        #endif



        protected new void Start() 
        {
            base.Start();
            m_stages = m_collection.Get<Stage>();
            foreach(var stage in m_stages)
            {
                stage.m_roomGraph = m_roomGraph;
            }

            Select(m_select);
        }
        
    }
}
