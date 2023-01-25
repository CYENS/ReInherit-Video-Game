using UnityEngine;

/***
 * This script is a Unity ScriptableObject that represents an item in a game
 * The ScriptableObject class allows the item to be saved as an asset in the project and used across multiple scenes in the game.
 ***/

// The CreateAssetMenu attribute allows the script to be used to create a new item asset in the Unity editor.
// When the user selects "Create/Inventory/Item" from the Assets menu, a new item asset is created and added to the project.
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")] 
public class Item : ScriptableObject
{
    // The name field represents the name of the item.
    new public string name = "New Item";

    // The icon field represents the sprite that should be used to display the item in the game.
    public Sprite icon = null;

    // The isDefaultItem field specifies whether the item is a default item that should not be removed from the inventory.
    public bool isDefaultItem = false;
}
