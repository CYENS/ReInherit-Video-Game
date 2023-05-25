using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Managers;
using Cyens.ReInherit.Exhibition;

namespace Cyens.ReInherit
{

    public class ExhibitInventory : BaseInventory
    {

        [Header("References")]
        protected GameManager m_gameManager;


        protected override void OnDeselect()
        {
            PlacementManager.Cancel();
        }
        
        protected new void Start() 
        {
            base.Start();
            m_gameManager = GameManager.Instance;

        }

        public void Place(Vector3 point) 
        {
            Debug.Log("Placing object");
            
            MetaData metaData = m_selected.m_metaData;
            int exhibitIndex = metaData.siblingIndex;
            ExhibitManager.Place(exhibitIndex, point );

            m_selected = null;
            OnDeselect();
        }

        protected override void OnSelect( )
        {
            MetaData metaData = m_selected.m_metaData;
            Mesh mesh = metaData.mesh;

            // Ensure that you exhibit is available to place
            int exhibitIndex = metaData.siblingIndex;
            var state = ExhibitManager.GetState(exhibitIndex);

            if( state != Exhibit.State.Storage )
            {
                m_selected = null;
                return;
            }


            PlacementManager.PlaceExhibit( gameObject, mesh);

        }
    }
}
