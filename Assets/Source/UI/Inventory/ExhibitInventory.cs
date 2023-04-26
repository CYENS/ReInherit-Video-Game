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
            GameObject temp = metaData.gameObject;
            temp.transform.position = point;
            Exhibit exhibit = temp.GetComponent<Exhibit>();
            if( exhibit != null )
            {
                exhibit.m_state = Exhibit.State.Display;
            }

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
