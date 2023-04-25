using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    public Interactable focus; 
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                removeFocus();

            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                // The object has been clicked
                // You can access the game object that was clicked using hit.collider.gameObject
                if (interactable != null)
                {
                    setFocus(interactable);
                }

            }
        }
    }

    void  setFocus(Interactable newFocus)
    {
        if (newFocus != focus)
        {
            if (focus != null)
                focus.OnDeFocused();
            focus = newFocus;
        }

        newFocus.OnFocused();
    }

    void removeFocus()
    {
        if (focus != null)
            focus.OnDeFocused(); 
        focus = null; 
    }
}
