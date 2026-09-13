using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

// Rishi Santhanam
// ItemIDs.cs

// Defines all ItemIDs for the game
// This includes all items, dances, and unlockables
//

/**
 * @brief ItemIds holds the encoding for the various items. This includes all
 * of the cosmetic items. It also holds the LoreObject which will be the
 * various emails from the original project team that have to do with the Lore.
 * It also holds the achievement descriptions which trigger when the player
 * does various things.
 */
public class ItemIDs : MonoBehaviour
{
    /**
     * @brief struct for defining items
     */
    public struct Item
    {
        public string name; // The name of the item
        public string type; // "hat", "chest", "leggings", "shoes", "dance", "unlockable"
        public int id; // The main kxwey
        public int cost; // The cost of the item
    }

    /**
     * @brief struct for defining lore objects
     * @see Achievement
     */
    public struct LoreObject
    {
        public string sender; // Whoever sends the e-mail
        public string subject; // The subject of the e-mail
        public string body; // The body of the e-mail
        public string date; // The date string of the e-mail (Nov. 29, 2044) etc
        public string[] receivers; // The receivers of the e-mail

        public LoreObject(string sender, string subject, string body, string date, string[] receivers)
        {
            this.sender = sender;
            this.subject = subject;
            this.body = body;
            this.date = date;
            this.receivers = receivers;
        }
    }

    /**
     * @brief struct for defining achievements
     * @see LoreObject
     */
    public struct Achievement
    {
        public string title; // The title of the achievement
        public string description; // The description of the achievement
        public int lore_linker; // The lore object that is linked to the achievement (0 -> 15)
    }

    // Public Dictionary int -> Achievement (for all achievements)
    public Dictionary<int, Achievement> achievement_database = new Dictionary<int, Achievement>()
    {
        {0, new Achievement{title = "Tutorial", description = "Complete the tutorial.", lore_linker = 0}},
        {1, new Achievement{title = "Wire Wiggler", description = "Beat all levels of the wire game.", lore_linker = 2}},
        {2, new Achievement{title = "Wire Wizard", description = "Beat all levels of the wire game five times.", lore_linker = 5}},
        {3, new Achievement{title = "Wire Warrior", description = "Beat all levels of the wire game ten times.", lore_linker = 3}},
        {4, new Achievement{title = "Piping Pupil", description = "Beat all levels of the pipe game once.", lore_linker = 6}},
        {5, new Achievement{title = "Piping Prodigy", description = "Beat all levels of the pipe game five times.", lore_linker = 7}},
        {6, new Achievement{title = "Piping Pro", description = "Beat all levels of the pipe game ten times.", lore_linker = 8}},
        {7, new Achievement{title = "Fluxed Up", description = "Take the neuroflux meter to 100.", lore_linker = 15}},
        {8, new Achievement{title = "Shopaholic", description = "Buy five items from the shop.", lore_linker = 12}},
        {9, new Achievement{title = "Retail Therapy", description = "Buy ten items from the shop.", lore_linker = 13}},
        {10, new Achievement{title = "Richie Rich", description = "Have 1000 coins in your inventory.", lore_linker = 11}},
        {11, new Achievement{title = "Lucky Strike", description = "Win 100 coins from the casino.", lore_linker = 1}},
        {12, new Achievement{title = "Where's my Money?", description = "Lose 100 coins from the casino.", lore_linker = 16}},
        {13, new Achievement{title = "Casino King", description = "Win 500 coins from the casino.", lore_linker = 9}},
        {14, new Achievement{title = "Casino God", description = "Win 1000 coins from the casino.", lore_linker = 10}},
        {15, new Achievement{title = "Factory Fury", description = "Win a match of the card game.", lore_linker = 4}},
        {16, new Achievement{title = "Card Shark", description = "Win five matches of the card game.", lore_linker = 14}},
        {17, new Achievement{title = "Card Collector", description = "Collect all cards from the deckmaster.", lore_linker = 18}},
        {18, new Achievement{title = "Weirdo", description = "Find the drunkard and speak with him.", lore_linker = 17}}
    };

    // Public Dictionary int -> LoreObject (for all lore objects)
    public Dictionary<int, LoreObject> lore_database = new Dictionary<int, LoreObject>()
    {
        {0, new LoreObject("Dr. Santhanam", "Long Time No See...",
        "Hey team,\n\nLong time no see. Well, I'm not sure if we'll see again. You know – when we started this project, we were all excited. To be able to remove the shackles of mortality from mankind... But now, I've lost my way.\n\nOn another note, I've transferred parts of my consciousness into the robot \"Sensei\". Take care of Keiko for me.\n\nI suppose this is goodbye,\nYour buddy",
        "Dec. 17, 2054", new string[]{"Ethan", "Rohan", "Naveed"})},
        {1, new LoreObject("Dr. Santhanam", "Come to the Lab! (URGENT)", "Rohan, can you come to the lab around 5 PM? I've made good progress on the protocol we discussed back in October. I think you'll be pleased with the results.\n\nBest,\nDr. Santhanam", "Nov. 13, 2053", new string[]{"Rohan"})},
        {2, new LoreObject("Dr. Santhanam", "Vacation Time", "Ethan. Me and my wife Keiko are going back to visit her family in Niigata. I was wondering if you'd like to come along.\n\nThat stipend from Project Elysium should be more than enough to cover the trip.\n\nKindest regards,\nRishi", "Mar. 28, 2051", new string[]{"Ethan"})},
        {3, new LoreObject("Dr. Santhanam", "Souvenirs?", "Hey, team.\n\nKeiko and I bought a lot of souvenirs for you all. I was wondering if you wanted to hop on a call and talk about which ones you'd like to take. Rohan, I got you that sword you wanted. Man, life is good...\n\nDrinking party on Sunday?\nRishi", "Apr. 05, 2051", new string[]{"Ethan", "Rohan", "Naveed"})},
        {4, new LoreObject("Ethan", "What are we doing?", "Hey, Rishi.\n\nI've been thinking about the project a lot. The possible implications, it's making me uneasy. When I started this, I wanted to make a difference in the world. I fear that we might make a difference, but it might not be a good one.\n\nLet me know your thoughts,\nEthan", "Jul. 24, 2051", new string[]{"Rishi"})},
        {5, new LoreObject("Ethan", "Re: Vacation Time", "Hey, Rishi.\n\nThanks for the invite. Can we stop by Osaka castle? I got a recommendation from a friend, and I want to try on some nice samurai clothes lol.\n\nBest,\nEthan", "Mar. 29, 2051", new string[]{"Rishi"})},
        {6, new LoreObject("Naveed", "I'm irritated", "Hey, guys\n\nIs anyone else getting frustrated? I've been working my butt off with no progress. Can I make a robot girlfriend? Is that even possible?\n\nIrritated regards,\nNaveed", "Aug. 24, 2051", new string[]{"Rishi", "Ethan", "Rohan"})},
        {7, new LoreObject("Naveed", "Robot Girlfriend?", "Yo, Ethan\n\nYou're okay with the robot girlfriend idea, right? I'm thinking of calling her Mrs. Haq. Get it? Cause my last name is... Oh what's the point in even explaining. Just let me know. Rishi isn't green lighting it.\n\nRegards,\nNaveed", "Aug. 25, 2051", new string[]{"Ethan"})},
        {8, new LoreObject("Ethan", "Re: Robot Girlfriend?", "Naveed,\n\nI'm not sure about the robot girlfriend idea. I think we should focus on the project. We're making good progress, and I think we can make a difference in the world.\n\nBest,\nEthan", "Sep. 1, 2051", new string[]{"Naveed"})},
        {9, new LoreObject("Ethan", "Project Update", "Hey, team.\n\nI'm at the lab with Rohan right now.\n\nWe just successfully transferred consciousness into a robot. The issue is, the consciousness isn't able to do anything yet. But at least it's there. This is a huge milestone, and I think we can do this thing.\n\nDrinks on me at the casino,\nEthan", "Dec. 24, 2051", new string[]{"Rishi", "Rohan", "Naveed"})},
        {10, new LoreObject("Ethan", "Issue: Is this normal?", "Hey, Rishi.\n\nWas working late at the lab last night. I swear I shut all the robots off, and while on my way out, I saw Mrs. Haq behind one of the desks. I swear I turned her off, and didn't move her at all. She started writing random things on the paper. We'll talk about the contents later.\n\nBest,\nEthan", "Feb. 14, 2052", new string[]{"Rishi"})},
        {11, new LoreObject("Dr. Santhanam", "Re: Issue: Is this normal?", "Hey, Ethan\n\nI'm not sure what's going on. It might an issue with the MOSFET diode that deals with the battery. I'll take a look at it tomorrow.\n\nBest,\nRishi", "Feb. 15, 2052", new string[]{"Ethan"})},
        {12, new LoreObject("Ethan", "Where's Rishi?", "Hey, team.\n\nRishi hasn't been in the lab for a few days. He isn't returning any of my e-mails, and I can't seem to get a hold of him. Did you guys see what he sent earlier? I'm worried.\n\nLet me know,\nEthan", "Dec. 23, 2054", new string[]{"Rohan", "Naveed"})},
        {13, new LoreObject("Naveed", "Re: Where's Rishi", "Well, that's good news. At least I can make infinite wives now.\n\nBut seriously, what's going on? I haven't seen him either. I'll check the casino.\n\nRegards,\nNaveed", "Dec. 23, 2054", new string[]{"Ethan", "Rohan"})},
        {14, new LoreObject("Rohan", "Re: Where's Rishi", "Hey team,\n\nRishi and I got on a call that day. I'm not sure how else to say this... I think Midra got a hold of him. The call ended abruptly, and I heard a few screams. I'm scared. If he's gone, what's going to happen to the rest of us?\n\nCall me ASAP,\nRohan", "Dec. 23, 2054", new string[]{"Ethan", "Naveed"})},
        {15, new LoreObject("Naveed", "Football game?", "Texans just won the championship. Anyone down to go to the casino later tonight and buy some drinks? First round's on me.\n\nBest,\nNaveed", "Feb. 5, 2051", new string[]{"Rishi", "Ethan", "Rohan"})},
        {16, new LoreObject("Rohan", "Re: Football game?", "Hey Naveed,\n\nI'm interested, but I have some work at the lab. Rishi told me he's hitting some sort of 'big break' and he needs my assistance.\n\nMy apologies,\nRohan", "Feb. 5, 2051", new string[]{"Naveed"})},
        {17, new LoreObject("Dr. Santhanam", "Project Success", "Hello Midra,\n\nI'm pleased to announce that Project Elysium is a success. We have a demo ready for you to present, and Rohan my assistant will be present at the demo. We're preparing to transfer the first five employees by this Friday.\n\nThanks for being there,\nRishi", "Apr. 29, 2052", new string[]{"Midra"})},
        {18, new LoreObject("Rishi", "To my love", "Keiko, \n\nI'm sorry. I'm sorry for everything. I'm sorry it had to come to this.\n\nTo be honest, Keiko, I've been feeling like I've been losing myself. My mind is in a bunch of pieces. Take the kids back to Japan. But no matter what happens, I love you. And I'll always love you. I'm so glad I met you that one fateful day. Even if this is the end, just know, I'll always be by your side.", "Dec. 17, 2054", new string[]{"Keiko"})},
    };

    // Create the item database
    // Hats start at 100, Chests start at 200, Leggings start at 300, Shoes start at 400, Dances start at 500, Unlockables start at 1000
    public Dictionary<int, Item> item_database = new Dictionary<int, Item>()
    {
        // Hats
        {100, new Item{name = "Orange Hard Hat", type = "hat", id = 100, cost = 125}},
        {101, new Item{name = "White Hard Hat", type = "hat", id = 101, cost = 125}},
        {102, new Item{name = "Yellow Hard Hat", type = "hat", id = 102, cost = 125}},
        {103, new Item{name = "Orange Party Hat", type = "hat", id = 103, cost = 250}},
        {104, new Item{name = "Purple Party Hat", type = "hat", id = 104, cost = 250}},
        {105, new Item{name = "Blue Party Hat", type = "hat", id = 105, cost = 250}},
        {106, new Item{name = "Cone", type = "hat", id = 106, cost = 500}},
        {107, new Item{name = "Medieval Helmet", type = "hat", id = 107, cost = 1000}},
        {108, new Item{name = "A&M Football Helmet", type = "hat", id = 108, cost = 500}},
        // Chest
        {200, new Item{name = "White Hoodie", type = "chest", id = 200, cost = 150}},
        {201, new Item{name = "Grey Hoodie", type = "chest", id = 201, cost = 150}},
        {202, new Item{name = "Black Hoodie", type = "chest", id = 202, cost = 150}},
        {203, new Item{name = "Blue Hoodie", type = "chest", id = 203, cost = 150}},
        {204, new Item{name = "Green Hoodie", type = "chest", id = 204, cost = 150}},
        {205, new Item{name = "Black Safety Vest", type = "chest", id = 205, cost = 450}},
        {206, new Item{name = "Grey Safety Vest", type = "chest", id = 206, cost = 450}},
        {207, new Item{name = "Orange Safety Vest", type = "chest", id = 207, cost = 450}},
        {208, new Item{name = "Medieval Armor", type = "chest", id = 208, cost = 1250}},
        // Leggings
        {300, new Item{name = "Black Utility Pants", type = "leggings", id = 300, cost = 300}},
        {301, new Item{name = "Blue Utility Pants", type = "leggings", id = 301, cost = 300}},
        {302, new Item{name = "Grey Utility Pants", type = "leggings", id = 302, cost = 300}},
        {303, new Item{name = "White Nano-Fiber Leggings", type = "leggings", id = 303, cost = 500}},
        {304, new Item{name = "Grey Nano-Fiber Leggings", type = "leggings", id = 304, cost = 500}},
        {305, new Item{name = "Black Nano-Fiber Leggings", type = "leggings", id = 305, cost = 500}},
        {306, new Item{name = "Medieval Leggings", type = "leggings", id = 306, cost = 1000}},
        // Footwear
        {400, new Item{name = "Brown Utility Boots", type = "shoes", id = 400, cost = 100}},
        {401, new Item{name = "Grey Utility Boots", type = "shoes", id = 401, cost = 100}},
        {402, new Item{name = "Black Utility Boots", type = "shoes", id = 402, cost = 100}},
        {403, new Item{name = "BW Jordan 1", type = "shoes", id = 403, cost = 500}},
        {404, new Item{name = "Retro Jordan 1", type = "shoes", id = 404, cost = 500}},
        {405, new Item{name = "UNC Jordan 1", type = "shoes", id = 405, cost = 500}},
        {406, new Item{name = "Medieval Boots", type = "shoes", id = 406, cost = 750}},
        // Dances
        {500, new Item{name = "Head-Ripper", type = "dance", id = 500, cost = 500}},
        {501, new Item{name = "Robot Dance", type = "dance", id = 501, cost = 500}},
        {502, new Item{name = "Zen Flip", type = "dance", id = 502, cost = 1200}},
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
    /**
     * @brief sprite and sprite id linker struct
     */
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

    // Grab the player's owned items and equippeditems from the playerData (is a singleton)
    private PlayerData playerData => PlayerData.Instance;

    // Function to get the sprite of an item
    public Sprite GetItemSprite(int itemID)
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
    public void FillInventoryButtons()
    {
        if (inventoryButtons == null) return;
        List<int> ownedItems = playerData.unlocked_items;

        if (selectedType == "all")
        {
            for (int i = 0; i < inventoryButtons.Length; i++)
            {
                if (inventoryButtons[i] == null) continue;
                if (i < ownedItems.Count)
                {
                    int itemID = ownedItems[i];
                    Sprite itemSprite = GetItemSprite(itemID);
                    inventoryButtons[i].SetActive(true);
                    inventoryButtons[i].GetComponent<Image>().sprite = itemSprite;
                    inventoryButtons[i].GetComponent<InvButtonItem>().item_id = itemID;

                    // Capture the current value of i in a new local variable
                    int buttonIndex = i;

                    // Dynamically change the button's onClick event
                    inventoryButtons[buttonIndex].GetComponent<Button>().onClick.RemoveAllListeners();

                    // Add the new listener
                    inventoryButtons[buttonIndex].GetComponent<Button>().onClick.AddListener(() =>
                        inventoryButtons[buttonIndex].GetComponent<InvButtonItem>().EquipItem(itemID)
                    );
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
                if (inventoryButtons[i] == null) continue;
                if (i < displayItems.Count)
                {
                    int itemID = displayItems[i];
                    Sprite itemSprite = GetItemSprite(itemID);
                    inventoryButtons[i].SetActive(true);
                    inventoryButtons[i].GetComponent<Image>().sprite = itemSprite;
                    inventoryButtons[i].GetComponent<InvButtonItem>().item_id = itemID;

                    // Capture the current value of i in a new local variable
                    int buttonIndex = i;

                    // Dynamically change the button's onClick event
                    inventoryButtons[buttonIndex].GetComponent<Button>().onClick.RemoveAllListeners();

                    // Add the new listener
                    inventoryButtons[buttonIndex].GetComponent<Button>().onClick.AddListener(() =>
                        inventoryButtons[buttonIndex].GetComponent<InvButtonItem>().EquipItem(itemID)
                    );
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
                        inventoryButtons[i].GetComponent<InvButtonItem>().item_id = itemID;

                        // Capture the current value of i in a new local variable
                        int buttonIndex = i;

                        // Dynamically change the button's onClick event
                        inventoryButtons[buttonIndex].GetComponent<Button>().onClick.RemoveAllListeners();

                        // Add the new listener
                        inventoryButtons[buttonIndex].GetComponent<Button>().onClick.AddListener(() =>
                            inventoryButtons[buttonIndex].GetComponent<InvButtonItem>().EquipItem(itemID)
                        );
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
                        inventoryButtons[i].GetComponent<InvButtonItem>().item_id = itemID;

                        // Capture the current value of i in a new local variable
                        int buttonIndex = i;

                        // Dynamically change the button's onClick event
                        inventoryButtons[buttonIndex].GetComponent<Button>().onClick.RemoveAllListeners();

                        // Add the new listener
                        inventoryButtons[buttonIndex].GetComponent<Button>().onClick.AddListener(() =>
                            inventoryButtons[buttonIndex].GetComponent<InvButtonItem>().EquipItem(itemID)
                        );
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
                        inventoryButtons[i].GetComponent<InvButtonItem>().item_id = itemID;

                        // Capture the current value of i in a new local variable
                        int buttonIndex = i;

                        // Dynamically change the button's onClick event
                        inventoryButtons[buttonIndex].GetComponent<Button>().onClick.RemoveAllListeners();

                        // Add the new listener
                        inventoryButtons[buttonIndex].GetComponent<Button>().onClick.AddListener(() =>
                            inventoryButtons[buttonIndex].GetComponent<InvButtonItem>().EquipItem(itemID)
                        );
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
                if (ownedItems[i] >= 500)
                {
                    displayItems.Add(ownedItems[i]);
                }
            }

            for (int i = 0; i < inventoryButtons.Length; i++)
            {
                if (inventoryButtons[i] == null) continue;
                if (i < displayItems.Count)
                {
                    int itemID = displayItems[i];
                    Sprite itemSprite = GetItemSprite(itemID);
                    inventoryButtons[i].SetActive(true);
                    inventoryButtons[i].GetComponent<Image>().sprite = itemSprite;
                    inventoryButtons[i].GetComponent<InvButtonItem>().item_id = itemID;

                    // Equipping the item
                    // Capture the current value of i in a new local variable
                    int buttonIndex = i;

                    // Dynamically change the button's onClick event
                    inventoryButtons[buttonIndex].GetComponent<Button>().onClick.RemoveAllListeners();

                    // Add the new listener
                    inventoryButtons[buttonIndex].GetComponent<Button>().onClick.AddListener(() =>
                        inventoryButtons[buttonIndex].GetComponent<InvButtonItem>().EquipItem(itemID)
                    );


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

        if (!isSorted(playerData.unlocked_items))
        {
            playerData.unlocked_items.Sort();
        }

        // Fill in the inventoryButtons images with the
        // correct sprites
        FillInventoryButtons();

    }

    // Check sorted list
    private bool isSorted(List<int> list)
    {
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i] < list[i - 1])
            {
                return false;
            }
        }
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        // Make unlocked Items organized if not sorted
        if (!isSorted(playerData.unlocked_items))
        {
            playerData.unlocked_items.Sort();
        }

        if (selectedType != newselectedType)
        {
            FillInventoryButtons();
            newselectedType = selectedType;
        }
    }
}
