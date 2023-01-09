using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class Exhibit : MonoBehaviour
    {
        public Transform placementPoint;

        public ExhibitSlot[] slots;

        private void Awake()
        {
            slots = GetComponentsInChildren<ExhibitSlot>();
            
        }


    }
}
