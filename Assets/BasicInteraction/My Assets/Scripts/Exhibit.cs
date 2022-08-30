using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class Exhibit : MonoBehaviour, IInteractable
    {
        [SerializeField] private string label;
        public string InteractionPrompt => "Look At "+label;
        
        public bool Interact(Interactor interactor)
        {
            Debug.Log("Admiring Exhibit...");
            return true;
        }
    }
}
