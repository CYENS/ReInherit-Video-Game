using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Interactable
{
    public Item item;

    public override void Interact()
    {
        // Goes back to Interactable.cs and executes the code
        base.Interact();
        PickUp();
    }
    void PickUp()
    {
        Debug.Log("Picking up " + item.name);
         
        // Add to inventory
        bool wasPickedUp = Inventory.instance.add(item, gameObject);

        if(wasPickedUp)
        {
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
            
    }

}
 