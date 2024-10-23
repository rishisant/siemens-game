using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

// Rishi Santhanam
// ItemIDs.cs

// Defines all ItemIDs for the game
// This includes all items, dances, and unlockables


public class ItemIDs : MonoBehaviour
{
    // Each item will have an ID, a name, and a type
    // Types: "hat", "chest", "leggings", "shoes", "dance", "unlockable"

    // Will be defined in an item database/dictionary that will have an integer id linked to
    // a object struct that contains name, type, and other information
    
    // Create the object struct
    public struct Item
    {
        public string name; // The name of the item
        public string type; // "hat", "chest", "leggings", "shoes", "dance", "unlockable"
        public int id; // The main kxwey
    }

    // Create the item database
    // Hats start at 100, Chests start at 200, Leggings start at 300, Shoes start at 400, Dances start at 500, Unlockables start at 1000
    public Dictionary<int, Item> item_database = new Dictionary<int, Item>()
    {
        // Hats
        {100, new Item{name = "Orange Hard Hat", type = "hat", id = 100}},
        {101, new Item{name = "White Hard Hat", type = "hat", id = 101}},
        {102, new Item{name = "Yellow Hard Hat", type = "hat", id = 102}},
        {103, new Item{name = "Orange Party Hat", type = "hat", id = 103}},
        {104, new Item{name = "Purple Party Hat", type = "hat", id = 104}},
        {105, new Item{name = "Blue Party Hat", type = "hat", id = 105}},
        {106, new Item{name = "Cone", type = "hat", id = 106}},
        {107, new Item{name = "Medieval Helmet", type = "hat", id = 107}},
        {108, new Item{name = "A&M Football Helmet", type = "hat", id = 108}},
        // Chest
        {200, new Item{name = "White Hoodie", type = "chest", id = 200}},
        {201, new Item{name = "Grey Hoodie", type = "chest", id = 201}},
        {202, new Item{name = "Black Hoodie", type = "chest", id = 202}},
        {203, new Item{name = "Blue Hoodie", type = "chest", id = 203}},
        {204, new Item{name = "Green Hoodie", type = "chest", id = 204}},
        {205, new Item{name = "Black Safety Vest", type = "chest", id = 205}},
        {206, new Item{name = "Grey Safety Vest", type = "chest", id = 206}},
        {207, new Item{name = "Orange Safety Vest", type = "chest", id = 207}},
        {208, new Item{name = "Medieval Armor", type = "chest", id = 208}},
        // Leggings
        {300, new Item{name = "Black Utility Pants", type = "leggings", id = 300}},
        {301, new Item{name = "Blue Utility Pants", type = "leggings", id = 301}},
        {302, new Item{name = "Grey Utility Pants", type = "leggings", id = 302}},
        {303, new Item{name = "White Nano-Fiber Leggings", type = "leggings", id = 303}},
        {304, new Item{name = "Grey Nano-Fiber Leggings", type = "leggings", id = 304}},
        {305, new Item{name = "Black Nano-Fiber Leggings", type = "leggings", id = 305}},
        {306, new Item{name = "Medieval Leggings", type = "leggings", id = 306}},
        // Footwear
        {400, new Item{name = "Brown Utility Boots", type = "shoes", id = 400}},
        {401, new Item{name = "Grey Utility Boots", type = "shoes", id = 401}},
        {402, new Item{name = "Black Utility Boots", type = "shoes", id = 402}},
        {403, new Item{name = "BW Jordan 1", type = "shoes", id = 403}},
        {404, new Item{name = "Retro Jordan 1", type = "shoes", id = 404}},
        {405, new Item{name = "UNC Jordan 1", type = "shoes", id = 405}},
        {406, new Item{name = "Medieval Boots", type = "shoes", id = 406}},
        // Dances
        {500, new Item{name = "Head-Ripper", type = "dance", id = 500}},
        {501, new Item{name = "Robot Dance", type = "dance", id = 501}},
        {502, new Item{name = "Zen Flip", type = "dance", id = 502}},
        // Unlockables
        {1000, new Item{name = "lore-object", type = "unlockable", id = 1000}}, // rishi to naveed
        {1001, new Item{name = "lore-object", type = "unlockable", id = 1001}}, // rishi to ethan, rohan
        {1002, new Item{name = "lore-object", type = "unlockable", id = 1002}}, // rishi ...
        {1003, new Item{name = "lore-object", type = "unlockable", id = 1003}},
        {1004, new Item{name = "lore-object", type = "unlockable", id = 1004}},
        {1005, new Item{name = "lore-object", type = "unlockable", id = 1005}},
        {1006, new Item{name = "lore-object", type = "unlockable", id = 1006}},
        {1007, new Item{name = "lore-object", type = "unlockable", id = 1007}},
        {1008, new Item{name = "lore-object", type = "unlockable", id = 1008}},
        {1009, new Item{name = "lore-object", type = "unlockable", id = 1009}},
        {1010, new Item{name = "lore-object", type = "unlockable", id = 1010}},
        {1011, new Item{name = "lore-object", type = "unlockable", id = 1011}},
        {1012, new Item{name = "lore-object", type = "unlockable", id = 1012}},
        {1013, new Item{name = "lore-object", type = "unlockable", id = 1013}},
        {1014, new Item{name = "lore-object", type = "unlockable", id = 1014}},
        {1015, new Item{name = "lore-object", type = "unlockable", id = 1015}},
        {1016, new Item{name = "lore-object", type = "unlockable", id = 1016}},
        {1017, new Item{name = "lore-object", type = "unlockable", id = 1017}},
        {1018, new Item{name = "lore-object", type = "unlockable", id = 1018}},
        {1019, new Item{name = "lore-object", type = "unlockable", id = 1019}},
        {1020, new Item{name = "lore-object", type = "unlockable", id = 1020}},
        {1021, new Item{name = "lore-object", type = "unlockable", id = 1021}},
        {1022, new Item{name = "lore-object", type = "unlockable", id = 1022}},
        {1023, new Item{name = "lore-object", type = "unlockable", id = 1023}},
        {1024, new Item{name = "lore-object", type = "unlockable", id = 1024}},
        {1025, new Item{name = "lore-object", type = "unlockable", id = 1025}},
        {1026, new Item{name = "lore-object", type = "unlockable", id = 1026}},
        {1027, new Item{name = "lore-object", type = "unlockable", id = 1027}},
        {1028, new Item{name = "lore-object", type = "unlockable", id = 1028}},
        {1029, new Item{name = "lore-object", type = "unlockable", id = 1029}},
        {1030, new Item{name = "lore-object", type = "unlockable", id = 1030}},
        {1031, new Item{name = "lore-object", type = "unlockable", id = 1031}},
        {1032, new Item{name = "lore-object", type = "unlockable", id = 1032}},
        {1033, new Item{name = "lore-object", type = "unlockable", id = 1033}},
        {1034, new Item{name = "lore-object", type = "unlockable", id = 1034}},
        {1035, new Item{name = "lore-object", type = "unlockable", id = 1035}},
        {1036, new Item{name = "lore-object", type = "unlockable", id = 1036}},
        {1037, new Item{name = "lore-object", type = "unlockable", id = 1037}},
        {1038, new Item{name = "lore-object", type = "unlockable", id = 1038}},
        {1039, new Item{name = "lore-object", type = "unlockable", id = 1039}},
        {1040, new Item{name = "lore-object", type = "unlockable", id = 1040}}
    };

    // Type of item that is selected in inventory (either "all", "hat" ...)
    public string selectedType = "all";
    public string newselectedType = "all";

    // Set the selected type of item
    public void setHat()
    {
        selectedType = "hat";
    }

    public void setChest()
    {
        selectedType = "chest";
    }

    public void setLeggings()
    {
        selectedType = "leggings";
    }

    public void setShoes()
    {
        selectedType = "shoes";
    }

    public void setUnlockable() // includes dance
    {
        selectedType = "unlockable";
    }


    // Put all the item IDs and the sprites in separate Serializables
    // This will allow us to easily assign the sprites to the item IDs
    // This will be used in the inventory system
    [Serializable]
    public struct ItemSprite
    {
        public int id;
        public Sprite sprite;
    }

    // Create the item sprites
    [SerializeField] private ItemSprite[] itemSprites;

    // The UI buttons in the inventory
    [SerializeField] private GameObject[] inventoryButtons;

    // Grab the player's owned items and equippeditems
    [SerializeField] private PlayerData playerData;

    // Function to get the sprite of an item
    private Sprite GetItemSprite(int itemID)
    {
        // Loop through all the item sprites
        for (int i = 0; i < itemSprites.Length; i++)
        {
            // If the item ID matches the item sprite ID
            if (itemID == itemSprites[i].id)
            {
                // Return the sprite
                return itemSprites[i].sprite;
            }
        }

        // If the item ID is not found, return null
        return null;
    }

    // Function to fill in the inventory buttons with the correct sprites
    // Only fill between 100-200 for hats
    // ... etc etc
    private void FillInventoryButtons()
    {
        List<int> ownedItems = playerData.unlocked_items;

        if (selectedType == "all")
        {
            for (int i = 0; i < inventoryButtons.Length; i++)
            {
                if (i < ownedItems.Count)
                {
                    int itemID = ownedItems[i];
                    Sprite itemSprite = GetItemSprite(itemID);
                    inventoryButtons[i].SetActive(true);
                    inventoryButtons[i].GetComponent<Image>().sprite = itemSprite;
                }
                else
                {
                    inventoryButtons[i].SetActive(false);
                }
            }
        } else if (selectedType == "hat") {

            List<int> displayItems = new List<int>();

            for (int i = 0; i < ownedItems.Count; i++)
            {
                if (ownedItems[i] >= 100 && ownedItems[i] < 200)
                {
                    displayItems.Add(ownedItems[i]);
                }
            }

            for (int i = 0; i < inventoryButtons.Length; i++)
            {
                if (i < displayItems.Count)
                {
                    int itemID = displayItems[i];
                    Sprite itemSprite = GetItemSprite(itemID);
                    inventoryButtons[i].SetActive(true);
                    inventoryButtons[i].GetComponent<Image>().sprite = itemSprite;
                }
                else
                {
                    inventoryButtons[i].SetActive(false);
                }
            }

        } else if (selectedType == "chest") {
                
                List<int> displayItems = new List<int>();
    
                for (int i = 0; i < ownedItems.Count; i++)
                {
                    if (ownedItems[i] >= 200 && ownedItems[i] < 300)
                    {
                        displayItems.Add(ownedItems[i]);
                    }
                }
    
                for (int i = 0; i < inventoryButtons.Length; i++)
                {
                    if (i < displayItems.Count)
                    {
                        int itemID = displayItems[i];
                        Sprite itemSprite = GetItemSprite(itemID);
                        inventoryButtons[i].SetActive(true);
                        inventoryButtons[i].GetComponent<Image>().sprite = itemSprite;
                    }
                    else
                    {
                        inventoryButtons[i].SetActive(false);
                    }
                }
    
            } else if (selectedType == "leggings") {
    
                List<int> displayItems = new List<int>();
    
                for (int i = 0; i < ownedItems.Count; i++)
                {
                    if (ownedItems[i] >= 300 && ownedItems[i] < 400)
                    {
                        displayItems.Add(ownedItems[i]);
                    }
                }
    
                for (int i = 0; i < inventoryButtons.Length; i++)
                {
                    if (i < displayItems.Count)
                    {
                        int itemID = displayItems[i];
                        Sprite itemSprite = GetItemSprite(itemID);
                        inventoryButtons[i].SetActive(true);
                        inventoryButtons[i].GetComponent<Image>().sprite = itemSprite;
                    }
                    else
                    {
                        inventoryButtons[i].SetActive(false);
                    }
                }
    
            } else if (selectedType == "shoes") {
    
                List<int> displayItems = new List<int>();
    
                for (int i = 0; i < ownedItems.Count; i++)
                {
                    if (ownedItems[i] >= 400 && ownedItems[i] < 500)
                    {
                        displayItems.Add(ownedItems[i]);
                    }
                }
    
                for (int i = 0; i < inventoryButtons.Length; i++)
                {
                    if (i < displayItems.Count)
                    {
                        int itemID = displayItems[i];
                        Sprite itemSprite = GetItemSprite(itemID);
                        inventoryButtons[i].SetActive(true);
                        inventoryButtons[i].GetComponent<Image>().sprite = itemSprite;
                    }
                    else
                    {
                        inventoryButtons[i].SetActive(false);
                    }
                }
        }
        else if (selectedType == "unlockable") {
    
            List<int> displayItems = new List<int>();
    
            for (int i = 0; i < ownedItems.Count; i++)
            {
                if (ownedItems[i] >= 1000)
                {
                    displayItems.Add(ownedItems[i]);
                }
            }
    
            for (int i = 0; i < inventoryButtons.Length; i++)
            {
                if (i < displayItems.Count)
                {
                    int itemID = displayItems[i];
                    Sprite itemSprite = GetItemSprite(itemID);
                    inventoryButtons[i].SetActive(true);
                    inventoryButtons[i].GetComponent<Image>().sprite = itemSprite;
                }
                else
                {
                    inventoryButtons[i].SetActive(false);
                }
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        // selectedType = all
        selectedType = "all";

        // Fill in the inventoryButtons images with the
        // correct sprites
        FillInventoryButtons();

        // Debug log all item equipped
        Debug.Log("Equipped Items: " + playerData.equipped_items[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedType != newselectedType)
        {
            FillInventoryButtons();
            newselectedType = selectedType;
        }

        // DEBUG LOG
        // Debug.Log("Selected Type: " + selectedType);
    }
}
