using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/***
 * This class is used when the left mouse is released when dragging from the inventory.
 * This script is attached to "Canvas-Inventory"
 ***/
public class ItemDropHandler : MonoBehaviour, IDropHandler
{
    private GameObject InventoryMannager;
    private Inventory inventoryScript;
    private Image image;
    private int index;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Drop item");

        // I will use this later when I implement the check if the position of the item to be dropped is not appropriate
        // => to display the icon in the inventory UI
        image = eventData.pointerCurrentRaycast.gameObject.GetComponent<Image>();
        var tempColor = image.color;
        tempColor.a = 1f;
        image.color = tempColor;

        // Get the index of what child the gameobject is in the Canvas-Inventory-ItemsParent
        index = eventData.pointerCurrentRaycast.gameObject.transform.parent.transform.parent.GetSiblingIndex();
        // Disable the "DragCollision" script so disabled game objects don't detect collisions
        inventoryScript.itemsGameObjects[index].GetComponent<DragColision>().enabled = false;
        // Remove the item from the inventory
        Inventory.instance.remove(Inventory.instance.items[index]);
        // Disable the variable "isDragging" from the "ItemDragHandler" script
        transform.GetChild(0).transform.GetChild(index).transform.GetChild(0).transform.GetChild(0).GetComponent<ItemDragHandler>().isDragging = false;        
    }
    
    private void Start()
    {
        // Find from the scene the "InventoryManager"
        InventoryMannager = GameObject.Find("InventoryManager");
        // Get the "Inventory" script from the "InventoryManager"
        inventoryScript = InventoryMannager.GetComponent<Inventory>();
    }

}
