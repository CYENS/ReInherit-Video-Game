using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Patterns;
using Cyens.ReInherit.Managers;

namespace Cyens.ReInherit.Exhibition
{
    /// <summary>
    /// Makes it easier for external scripts to interact with Exhibit game objects.
    /// - It is able to change the state of exhibits.
    /// - It is able to add/remove exhibits.
    /// - It manages exhibit related events, such as '0 condition' event
    /// </summary>
    public class ExhibitManager : CollectionManager<ExhibitManager,Exhibit>
    {
        
        protected Exhibit[] m_exhibits;


        protected new void Start() 
        {
            base.Start();
            m_exhibits = m_collection.Get<Exhibit>();
        }
        

        public static void Place( int index, Vector3 point )
        {
            Exhibit exhibit = Instance.m_exhibits[index];
            exhibit.transform.position = point;
            exhibit.SetState( Exhibit.State.Transit );
            KeeperManager.Instance.AddPlaceTask( exhibit );
        }
        
    }
}
