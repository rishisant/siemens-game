using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Rishi Santhanam - Equipping items for the inventory
 */
public class InvButtonItem : MonoBehaviour
{
    // Serialized Fields
    // in the Inventory.cs script
    public int item_id;

    // The PlayerData is a singleton
    // We will grab the playerData object and
    // equip the item
    private PlayerData playerData => PlayerData.Instance;

    // Basically, we will be calling the Set?Sprite method on the playerObject
    // to change the sprite of the playerObject to the sprite of the item
    // that the button represents

    /**
     * @brief Equip the item that the player clicked on. If the item is already
     * equipped, then it will be unequipped.
     */
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
            playerData.NotifyEquippedItemsChanged();
            return;
        }

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
        playerData.NotifyEquippedItemsChanged();
    }
}
