using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/***
 * This class is used when the left mouse is pressed when dragging from the inventory.
 * This script is attached to "Canvas-Inventory-ItemsParent-InventorySlot-ItemButton-Icon"
 ***/
using static UnityEditor.Progress;

public class ItemDragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    private GameObject InventoryMannager;
    private Inventory inventoryScript;
    private Image image;

    // Speed at which the object will rotate
    [SerializeField] float rotateSpeed = 10.0f;

    // What number of child it is on Canvas-Inventory-ItemsParent
    private int index;

    // This variable is set to true when you click an icon in the inventory ui and set to false from the "ItemDropHandler" script
    public bool isDragging = false;

    private void Start()
    {
        // Find from the scene the "InventoryManager"
        InventoryMannager = GameObject.Find("InventoryManager");

        // Get the "Inventory" script from the "InventoryManager"
        inventoryScript = InventoryMannager.GetComponent<Inventory>();
    }


    public void OnDrag(PointerEventData eventData)
    {
        // Update the icon position
        transform.position = Input.mousePosition;

        // Update the position of the dragging object
        dragObjectUpdatePosition();

        // Check if the mouse scrollwheel has been moved
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        // If the scrollwheel has been moved, rotate the object
        if (scroll != 0.0f)
        {
            // Rotate the dragging object
            inventoryScript.itemsGameObjects[index].transform.Rotate(Vector3.up, scroll * rotateSpeed, Space.World);
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        // Put the icon from inventory ui back to its place (if not dropped on the scene)
        transform.localPosition = Vector3Int.zero;

        //gameObject.GetComponent<Image>().enabled = true;

    }

    // When an icon from the inventory ui is clicked
    public void OnPointerDown(PointerEventData eventData)
    {
        // Enable the "isDragging" variable
        isDragging = true;

        // Hide the icon from the inventory UI
        image = gameObject.GetComponent<Image>();
        var tempColor = image.color;
        tempColor.a = 0f;
        image.color = tempColor;

        // Get the index of what child the gameobject is in the Canvas-Inventory-ItemsParent
        index = eventData.pointerCurrentRaycast.gameObject.transform.parent.transform.parent.GetSiblingIndex();

        // Update the position of the dragging object
        dragObjectUpdatePosition();

        // Enable the dragging gameobject in the scene
        inventoryScript.itemsGameObjects[index].SetActive(true);

        // Enable the "DragColision" script of the dragging fameobject
        inventoryScript.itemsGameObjects[index].GetComponent<DragColision>().enabled = true;

    }


    private void Update()
    {
        if(isDragging)
        {
            // Check if the mouse scrollwheel has been moved
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            // If the scrollwheel has been moved, rotate the object
            if (scroll != 0.0f)
            {
                inventoryScript.itemsGameObjects[index].transform.Rotate(Vector3.up, scroll * rotateSpeed, Space.World);
            }
        }
        
    }

    private void dragObjectUpdatePosition()
    {
        Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(inventoryScript.itemsGameObjects[index].transform.position).z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);

        // Update the position of the dragging object
        inventoryScript.itemsGameObjects[index].transform.position = new Vector3(worldPosition.x, .5f, worldPosition.z);
    }

}
