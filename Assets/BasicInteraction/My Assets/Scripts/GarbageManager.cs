using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Cyens.ReInherit
{
    public class GarbageManager : MonoBehaviour
    {
        [SerializeField] private List<Garbage> m_Garbage;

        public List<Garbage> GetGarbage => m_Garbage;
        
        // Start is called before the first frame update
        void Awake()
        {
            m_Garbage = new List<Garbage>();
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
                if(child.gameObject.activeInHierarchy) m_Garbage.Add(child.GetComponent<Garbage>());
            }
            SortListWithDistance();
        }

        private void Update()
        {
            GetGarbageObjects();
        }
    }
}
