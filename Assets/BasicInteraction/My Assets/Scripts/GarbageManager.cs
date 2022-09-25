using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

namespace Cyens.ReInherit
{
    public class GarbageManager : MonoBehaviour
    {
        [SerializeField] private List<Garbage> m_Garbage;
        private int m_lastGarbageCount;
        private GameObject m_floor;
        
        public List<Garbage> GetGarbage => m_Garbage;
        
        // Start is called before the first frame update
        void Awake()
        {
            m_Garbage = new List<Garbage>();
            m_lastGarbageCount = 0;
            m_floor = GameObject.Find("Floor");
        }

        private void SortListWithDistance()
        {
            m_Garbage = m_Garbage.OrderByDescending(g => g.GetDuration).ToList();
        }

        private void GetGarbageObjects()
        {
            m_Garbage = new List<Garbage>();
            foreach (Transform child in transform)
            {
                if(child.gameObject.activeInHierarchy)
                    m_Garbage.Add(child.GetComponent<Garbage>());
            }
            SortListWithDistance();
        }

        private void ReactivateGarbage()
        {
            foreach (Transform child in transform)
                child.gameObject.SetActive(true);
        }

        private void Update()
        {
            GetGarbageObjects();
            if(m_Garbage.Count == 0)
                ReactivateGarbage();
            if (m_Garbage.Count != m_lastGarbageCount) {
                if(m_floor.TryGetComponent(out FloorNavMesh surface))
                    surface.RebakeNavmesh();
                Debug.Log("Rebaking NavMesh.");
            }
            m_lastGarbageCount = m_Garbage.Count;
        }
    }
}
