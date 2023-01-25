using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class WayPoint : MonoBehaviour
    {
        internal GameObject owner;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == owner) {
                Destroy(gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
