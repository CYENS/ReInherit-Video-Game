using Unity.VisualScripting;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    bool isFocus = false;
    bool hasInteracted = false;

    public virtual void Interact()
    {
        //This method is meant to be overwritten
        Debug.Log("Interact");
    }

    private void Update()
    {
        if(isFocus && !hasInteracted)
        {
            Interact();
            hasInteracted = true; 
        }
    }

    public void OnFocused()
    {
        isFocus = true;
        hasInteracted = false; 
    }

    public void OnDeFocused()
    {
        isFocus = false ;
        hasInteracted = false;  
    }

}
