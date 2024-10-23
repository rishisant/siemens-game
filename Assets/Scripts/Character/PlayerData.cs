using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rishi Santhanam
// Player data storage. All player data is stored here.
public class PlayerData : MonoBehaviour
{
    // Storing information like the player's username, coins
    [SerializeField] public string username;
    [SerializeField] public int coins;
    
    // Storing public information like the current interactable
    public string interactable; // This will be changed to things such as "leaderboard"

    // Item Ids
    [SerializeField] private ItemIDs item_ids;
    // Grab the item_database
    public Dictionary<int, ItemIDs.Item> item_database => item_ids.item_database;

    // NOTE:
    // Will eventually store all obtained items here PUBLICLY
    // because we will need to access this later for inventory
    // and other setup.

    // Add player data such as interactions with certain NPC's
    // Default Npc's: 'deckmaster', 'casino_owner', 'shopkeeper', 'drunk_robot'
    // Storing the interaction_times with each NPC (starting at 0)
    [SerializeField] public Dictionary<string, int> npc_interactions = new Dictionary<string, int>()
    {
        {"deckmaster", 0},
        {"casino_owner", 0},
        {"shopkeeper", 0},
        {"drunk_robot", 0}
    };

    // Storing the current items the player has unlocked (list of item id's)
    [SerializeField] public List<int> unlocked_items = new List<int>();

    // Storing the current items the player is equipping right now
    // List of item id's
    [SerializeField] public List<int> equipped_items = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        // Fill in the inventoryButtons images with the 
    }

    // Update is called once per frame
    void Update()
    {
        // If the X key is pressed down
        // Change the items to 102, 203, 304, 405
        if (Input.GetKeyDown(KeyCode.X))
        {
            equipped_items = new List<int> {102, 203, 304, 405};
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            equipped_items = new List<int> {100, 200, 300, 400};
        }
    }
}
