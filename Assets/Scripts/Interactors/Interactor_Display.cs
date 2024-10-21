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

    // Defining the Exit Out for Leaderboards
    public void ExitLeaderboards()
    {
        leaderboards.SetActive(false);
        baseGameUI.SetActive(true);
    }

    // Grab the playerData's interactable string
    // EX: If the playerData's interactable string is "leaderboard"
    // then we will show the leaderboards
    private string interactable => player.GetComponent<PlayerData>().interactable;

    public void DisplayPanel()
    {
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
