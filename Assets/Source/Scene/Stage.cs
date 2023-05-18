using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Architect;

namespace Cyens.ReInherit
{
    public class Stage : MonoBehaviour
    {

        [SerializeField]
        private int m_width = 6;
        public int Width => m_width;


        [SerializeField]
        private int m_height = 4;
        public int Height => m_height;

        [Header("References")]
        [SerializeField]
        public RoomGraph m_roomGraph;
        
        private void OnEnable() 
        {
            if( m_roomGraph == null )
            {
                return;
            }
            
            m_roomGraph.MaxWidth = m_width;
            m_roomGraph.MaxHeight = m_height;

            Debug.Log("Setting room graph size: "+m_roomGraph.MaxWidth+", "+m_roomGraph.MaxHeight);
        }


    }
}
