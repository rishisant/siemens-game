using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rishi Santhanam
// Equipping items from the inventory
public class InvButtonItem : MonoBehaviour
{
    // Serialized Fields
    // in the Inventory.cs script
    public int item_id;

    // Also, what is the playerData
    [SerializeField] public PlayerData playerData;

    // Basically, we will be calling the Set?Sprite method on the playerObject
    // to change the sprite of the playerObject to the sprite of the item
    // that the button represents

    public void EquipItem(int item_id)
    {
        // If item is already equipped, unequip it
        if (playerData.equipped_items.Contains(item_id))
        {
            playerData.equipped_items.Remove(item_id);
            // Then add 199 or 299 or 399 or 499
            // to the equipped_items list
            // This is because the player can only have one item

            playerData.equipped_items.Add(item_id / 100 * 100 + 99);
            return;
        }
        // Debug.Log("Equipping item: " + item_id);
        // Equip the item and remove any items within the same 100's range
        // For example if a previous item from the equipped_items list has 200
        // And we're equipping 203, we should remove 200 from the equipped_items list
        
        // Remove any items within the same 100's range
        List<int> toRemove = new List<int>();

        foreach (int equipped_item in playerData.equipped_items)
        {
            if (equipped_item / 100 == item_id / 100)
            {
                toRemove.Add(equipped_item);
            }
        }

        // Remove the items
        foreach (int remove in toRemove)
        {
            playerData.equipped_items.Remove(remove);
        }

        // Equip the item
        playerData.equipped_items.Add(item_id);
        playerData.equipped_items.Sort();

        // Print all the equipped items
        // Debug.Log("EQUIPPED::::");
        // for (int i = 0; i < playerData.equipped_items.Count; i++)
        // {
        //     Debug.Log("Equipped item: " + playerData.equipped_items[i]);
        // }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
