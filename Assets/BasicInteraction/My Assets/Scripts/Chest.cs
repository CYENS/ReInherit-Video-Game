using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private string m_prompt;
    
    public string InteractionPrompt => m_prompt;

    public bool Interact(Interactor interactor)
    {
        Debug.Log("Opening chest!");
        return true;
    }
}
