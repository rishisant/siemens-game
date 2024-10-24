using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor_Display : MonoBehaviour
{
    // Call the GameObject for Leaderboards
    [SerializeField] private GameObject leaderboards;

    // Call the GameObject for the Base Game UI
    [SerializeField] private GameObject baseGameUI;

    // Call the GameObject for Player
    [SerializeField] private GameObject player;

    // Call the Inventory Panel
    [SerializeField] private GameObject inventoryPanel;

    // Defining the Exit Out for Leaderboards
    public void ExitLeaderboards()
    {
        leaderboards.SetActive(false);
        baseGameUI.SetActive(true);

        // Unstop the player
        player.GetComponent<Character_Movement>().UnstopPlayer();
    }

    // Defining the Exit Out for Inventory
    public void ExitInventory()
    {
        inventoryPanel.SetActive(false);
        baseGameUI.SetActive(true);

        // Unstop the player
        player.GetComponent<Character_Movement>().UnstopPlayer();
    }

    // Opening the Inventory
    public void OpenInventory()
    {
        inventoryPanel.SetActive(true);
        baseGameUI.SetActive(false);

        // Stop the player
        player.GetComponent<Character_Movement>().StopPlayer();
    }

    // Grab the playerData's interactable string
    // EX: If the playerData's interactable string is "leaderboard"
    // then we will show the leaderboards
    private string interactable => player.GetComponent<PlayerData>().interactable;

    public void DisplayPanel()
    {
        // Stop the player
        player.GetComponent<Character_Movement>().StopPlayer();

        if (interactable == "leaderboard")
        {
            leaderboards.SetActive(true);
            baseGameUI.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // DEBUG: Interactable testing
        // Debug.Log(interactable);
    }
}
