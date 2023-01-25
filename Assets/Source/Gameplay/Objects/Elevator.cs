using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class Elevator : MonoBehaviour
    {
        
        [SerializeField] [Tooltip("Where this elevator leads to")]
        private Elevator other;

        [SerializeField] [Tooltip("The point at which the character exits the elevator")]
        private Transform exit;

        public Vector3 exitPoint => exit.position;
        
        private void Awake()
        {
            // Just to ensure the two elevators are connected
            if (other != null)
                other.other = this;
        }

        
        
    }
}
