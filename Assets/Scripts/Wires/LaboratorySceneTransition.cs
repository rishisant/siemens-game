using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * This class handles the scene transition from the laboratory to the wire game
 */
public class LaboratorySceneTransition : MonoBehaviour
{
    public Button wireGameStartButton;
    private bool playerInZone = false;

    /**
     * OnTriggerEnter2D() is a Unity function that runs when another Collider2D
     * enters the zone of this gameObject's Collider2D
     *
     * This handles the zone for starting the wire game. When a player enters
     * this area, a button will pop up which allows them to enter the wire game
     * 
     * @param other The other Collider2D that is currently colliding with this gameObject
     */
    public void OnTriggerEnter2D(Collider2D other)
    {
        playerInZone = true;
        wireGameStartButton.gameObject.SetActive(true);
    }

    /**
     * OnTriggerExit2D() is a Unity function that runs when another Collider2D
     * exits the zone of this gameObject's Collider2D
     *
     * This handles the zone for starting the wire game. When a player leaves
     * this area, the button should become inactive again
     * 
     * @param other The other Collider2D that is currently colliding with this gameObject
     */
    public void OnTriggerExit2D(Collider2D other)
    {
        playerInZone = false;
        wireGameStartButton.gameObject.SetActive(false);
    }

    /**
     * StartWireGame() is a function that is attached to the WireGameButton in
     * the laboratory. When this button is pressed, the player will load into
     * the wire game scene
     */
    public void StartWireGame()
    {
        if (playerInZone)
        {
            SceneManager.LoadScene("WireGame");
        }
        else
        {
            Debug.Log("Player pressed wire game start button while not in zone");
        }
    }
}
