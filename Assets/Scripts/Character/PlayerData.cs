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

    // NOTE:
    // Will eventually store all obtained items here PUBLICLY
    // because we will need to access this later for inventory
    // and other setup.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
