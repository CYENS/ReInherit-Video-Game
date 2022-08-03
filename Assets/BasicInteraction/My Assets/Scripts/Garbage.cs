using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garbage : MonoBehaviour, IInteractable
{
    [SerializeField] private string m_prompt;
    [SerializeField] private float m_duration = 0;
    [SerializeField] private float m_expiresDuration = 600f;

    public string InteractionPrompt => m_prompt;
    public float GetDuration => m_duration;
    
    public bool Interact(Interactor interactor)
    {
        Debug.Log("Collect Garbage by Player!");
        gameObject.SetActive(false);
        return true;
    }
    
    public bool Interact(InteractorAI interactor)
    {
        Debug.Log("Collect Garbage by AI!");
        gameObject.SetActive(false);
        return true;
    }

    private void FixedUpdate()
    {
        m_duration += Time.fixedDeltaTime;
    }
}
