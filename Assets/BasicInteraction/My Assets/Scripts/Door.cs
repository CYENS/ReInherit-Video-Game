using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private string m_prompt;
    
    public string InteractionPrompt => m_prompt;
    
    public bool Interact(Interactor interactor)
    {
        var inventory = interactor.GetComponent<Inventory>();

        if (inventory == null) return false;

        if (inventory.GetHasKey())
        {
            Debug.Log("Opening door!");
            return true;
        }

        Debug.Log("No key found! Press Q to Get Key.");
        return false;

    }
}
