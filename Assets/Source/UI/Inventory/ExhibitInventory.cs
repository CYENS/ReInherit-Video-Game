using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Managers;
using Cyens.ReInherit.Exhibition;

namespace Cyens.ReInherit
{

    public class ExhibitInventory : BaseInventory
    {

        protected override void OnDeselect()
        {
            PlacementManager.Cancel();
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

            PlacementManager.PlaceExhibit( gameObject, mesh);

        }
    }
}
