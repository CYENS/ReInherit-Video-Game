using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/***
 * This class is used to manage the inventory system.
 * This script is attached to "InventoryManager"
 ***/
public class Inventory : MonoBehaviour
{
    #region Singleton
    // Singleton Pattern
    // A singleton in Unity is a globally accessible class that exists in the scene, but only once.
    public static Inventory instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found!");
            return;
        }

        instance = this;
    }
    #endregion

    // The onItemChangedCallback delegate is used to notify other scripts when the inventory has changed.
    // should be called when the inventory is modified.
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    [SerializeField] private int space = 20;

    // List of the items in the inventory
    public List<Item> items = new List<Item>();

    // List of the gameobjects in the inventory
    public List<GameObject> itemsGameObjects = new List<GameObject>();

    // The add method is used to add an item and its corresponding game object to the inventory.
    // It first checks if there is enough space in the inventory and returns false if there is not.
    // Otherwise, it adds the item and game object to the items and itemsGameObjects lists and
    // invokes the onItemChangedCallback delegate if it is not null.
    public bool add(Item item, GameObject itemGameObject)
    {
        if(!item.isDefaultItem)
        {
            if (items.Count >= space)
            {
                Debug.Log("Not enough room.");
                return false;
            }

            items.Add(item);
            itemsGameObjects.Add(itemGameObject);

            if(onItemChangedCallback != null)
                onItemChangedCallback.Invoke(); 
        }
        return true;
    }

    // The remove method is used to remove an item from the inventory. It removes the item from
    // the items list and removes the corresponding game object from the itemsGameObjects list
    // at the same index. It also invokes the onItemChangedCallback delegate if it is not null.
    public void remove(Item item)
    {
        int index = items.IndexOf(item);
        items.Remove(item);
        itemsGameObjects.RemoveAt(index);

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }
}
