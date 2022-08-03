using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractorAI : MonoBehaviour
{
    [SerializeField] private Transform m_interactionPoint;
    [SerializeField] private float m_interactionPointRadius = 0.5f;
    [SerializeField] private LayerMask m_interactableMask;
    private readonly Collider[] m_colliders = new Collider[3];
    [SerializeField] private int m_numFound;
    [SerializeField] private string m_dectableTag = "Garbage";

    private IInteractable m_interactable;
    private void Update()
    {
        m_numFound = Physics.OverlapSphereNonAlloc(m_interactionPoint.position, m_interactionPointRadius, m_colliders, m_interactableMask);

        //If found some objects to interact with
        if (m_numFound > 0)
        {
            //Interact with one of them
            m_interactable = m_colliders[0].GetComponent<IInteractable>();

            if (m_interactable != null)
            {
                //if "e" key pressed this frame, interact with object
                if (m_colliders[0].CompareTag(m_dectableTag))
                    m_interactable.Interact(this);
            }
        }
        else
        {
            if (m_interactable != null)
                m_interactable = null;
        }
    }

    //Draw interaction point collider on player
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(m_interactionPoint.position, m_interactionPointRadius);
    }
}
