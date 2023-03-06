using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class colliding : MonoBehaviour
    {
        public bool isColliding = false;


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "Floor" && other.gameObject.tag != "ElevatorEntryArea")
                isColliding = true;
        }
        private void OnTriggerExit(Collider other)
        {
            isColliding = false;
        }
    }
}
