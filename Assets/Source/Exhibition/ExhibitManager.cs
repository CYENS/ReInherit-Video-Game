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

        [Header("Assets")]
        
        [SerializeField] protected AssetLibrary m_assetLibrary;
        [SerializeField] protected GameObject[] m_prefabs;
        [SerializeField] protected int m_index;


        protected new void Start() 
        {
            base.Start();
            m_exhibits = m_collection.Get<Exhibit>();

            // Create 
            m_prefabs = m_assetLibrary.GetAssets<GameObject>();
            // TODO: Shuffle array
        }




        public void Advance() => m_index++;
        public GameObject Next()
        {
            if( m_index < 0 || m_index >= m_prefabs.Length )
            {
                return null;
            }

            var current = m_prefabs[m_index];
            return current;
        }

        public ExhibitInfo NextInfo()
        {
            var go = Next();
            if( go == null )
            {
                return null;
            }

            Exhibit exhibit = go.GetComponent<Exhibit>();
            if( exhibit == null )
            {
                return null;
            }

            ExhibitInfo info = exhibit.Info;
            if( info == null )
            {
                return null;
            }

            return info;
        } 

        public void AddNext()
        {
            GameObject prefab = Next();
            if( prefab == null )
            {
                return;
            }

            GameObject go = Instantiate(prefab);
            m_collection.Add(go);

            // Update list
            m_exhibits = m_collection.Get<Exhibit>();
        }


        public Exhibit[] GetExhibitsByState(Exhibit.State state)
        {
            List<Exhibit> outList = new List<Exhibit>();
            
            foreach (var e in m_exhibits) {
                if(e.m_state == state)
                    outList.Add(e);
            }
            return outList.ToArray();
        }
        
        public Exhibit GetExhibit(int index)
        {
            if( m_exhibits == null || m_exhibits.Length == 0 )
            {
                return null;
            }

            index = Mathf.Clamp(index,0,m_exhibits.Length-1);
            return m_exhibits[index];
        }

        public static void Place( int index, Vector3 point )
        {
            Exhibit exhibit = Instance.GetExhibit(index);
            exhibit.transform.position = point;
            exhibit.SetState( Exhibit.State.Transit );
            KeeperManager.Instance.AddPlaceTask( exhibit );
        }
        
        public static Exhibit.State GetState( int index )
        {
            Exhibit exhibit = Instance.GetExhibit(index);
            return exhibit.GetState();
        }

    }
}
