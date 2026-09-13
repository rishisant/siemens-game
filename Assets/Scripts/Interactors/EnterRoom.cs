using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Scene Manager
using UnityEngine.SceneManagement;

// Rishi Santhanam
// This script is used to load the next scene
/**
 * @class EnterRoom
 * @brief This class is used to load the next scene when entering/exiting a room
 */
public class EnterRoom : MonoBehaviour
{
    // Essentially, this script is used to load the next scene
    // Whether entering or exiting a room
    // PlayerData
    public PlayerData playerData => PlayerData.Instance;

    // GameManager
    public GameManager gameManager => GameManager.Instance;

    [SerializeField] private int scene_checker = 0;

    public string MapDestination { get { return scene_checker==1?"LAB":scene_checker==2?"CASINO":scene_checker==3 || scene_checker==4?"TOWN":""; } }

    // Serialize the interact button
    [SerializeField] private GameObject interactButton;

    // OnTriggerEnter2D
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If scene_checker = 1, laboratory (interactable == enterlaboratory)
        // If scene_checker = 2, casino
        // If scene_checker = 3, town square from lab
        // If scene_checker = 4, town square from casino
        if (scene_checker == 1)
        {
            interactButton.SetActive(true);
            playerData.interactable = "enterlaboratory";
        } // ...
        else if (scene_checker == 2)
        {
            interactButton.SetActive(true);
            playerData.interactable = "entercasino";
        } // ...
        else if (scene_checker == 3)
        {
            interactButton.SetActive(true);
            playerData.interactable = "exitlaboratory";
        } // ...
        else if (scene_checker == 4)
        {
            interactButton.SetActive(true);
            playerData.interactable = "exitcasino";
        } // ...
        // else if (scene_checker ==)
    }

    // OnTriggerExit2D
    private void OnTriggerExit2D(Collider2D other)
    {
        playerData.interactable = "";
        interactButton.SetActive(false);
    }
}
