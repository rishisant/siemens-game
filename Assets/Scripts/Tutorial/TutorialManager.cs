using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Essentially, want to have a tutorial manager that allows for Sensei to give the
// player instructions through dialogue. This will be done through a dialogue manager
// that will be called by the tutorial manager. The tutorial manager will be responsible
// for managing the tutorial, panning the camera, and other tutorial-related tasks.

public class TutorialManager : MonoBehaviour
{
    // The dialogue panel
    [SerializeField] private GameObject dialoguePanel;

    // The dialogue manager
    [SerializeField] private DialogueManagerTutorial dialogueManager;

    // Grab the dialogueIndex from the dialogue manager
    private int dialogueIndex => dialogueManager.dialogueIndex;

    // Which flag we're at
    private int flagIndex = 0;

    // The player
    [SerializeField] private Character_Movement player;

    // The camera
    [SerializeField] private CameraFollow camera;

    // Grab the public bool inventoryClosed from InventoryUI_Tutorial
    // This will be used to check if the user finished the inventory tutorial
    [SerializeField] private InventoryUI_Tutorial inventoryUI;
    private bool inventoryClosed => inventoryUI.inventoryClosed;

    // Import the inventory button
    [SerializeField] private GameObject inventoryButton;

    // The flag (from flag object 1 - 3)
    // The flag's child activate object
    [SerializeField] private GameObject[] flags;
    [SerializeField] private GameObject[] flagActivateObjects;

    // The object panel for Finish-Tutorial
    // The modal to black out everything
    [SerializeField] private GameObject finishTutorialPanel;
    [SerializeField] private GameObject blackoutPanel;

    // Private flag to check for waiting on objective
    private bool waitingOnObjective = false;

    // Detect any taps
    private void OnMouseDown()
    {
        dialogueManager.DisplayNextSentence();
    }

    // Start is called before the first frame update
    void Start()
    {
        // First, stop the player and wait for 1.5 seconds
        player.StopPlayer();
        
        // Wait for 3 seconds
        StartCoroutine(WaitForSeconds(3.0f));

        // Start the dialogue
        dialogueManager.StartDialogue();
    }

    // Wait for seconds
    private IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    // Update is called once per frame
    void Update()
    {
        // Print out the dialogue index and waiting on objective
        // Debug.Log("INDEX: " + dialogueIndex + " WAITING: " + waitingOnObjective);

        // Check when hits the 2nd dialogue index
        if (dialogueIndex == 5 && !waitingOnObjective)
        {
            // Move the camera to the first flag
            camera.PanCamera(flags[0].transform);

            // Pause the dialogue
            dialogueManager.PauseDialogue();

            // Assign an objective
            waitingOnObjective = true;

            // Let player move
            player.UnstopPlayer();
        } 
            
        else if (dialogueIndex == 5 && waitingOnObjective)
        {
            // Check if the player is near the flag
            if (Vector2.Distance(player.transform.position, flags[0].transform.position) < 0.5f)
            {
                // Play the Flag activate animation and set active
                flagActivateObjects[0].SetActive(true);

                // Unpause the dialogue and stop the player
                dialogueManager.ResumeDialogue();
                player.StopPlayer();

                waitingOnObjective = false;
            }
        }
        else if (dialogueIndex == 7 && !waitingOnObjective) {

            // Activate flag 2
            flags[1].SetActive(true);

            // Pan camera to flag 2
            camera.PanCamera(flags[1].transform);

            // Pause the dialogue
            dialogueManager.PauseDialogue();

            // Assign an objective
            waitingOnObjective = true;

            // Let player move
            player.UnstopPlayer();
        }
        else if (dialogueIndex == 7 && waitingOnObjective)
        {
            // Check if the player is near the flag
            if (Vector2.Distance(player.transform.position, flags[1].transform.position) < 0.5f)
            {
                // Play the Flag activate animation and set active
                flagActivateObjects[1].SetActive(true);

                // Unpause the dialogue and stop the player
                dialogueManager.ResumeDialogue();
                player.StopPlayer();

                waitingOnObjective = false;
            }
        }
        else if (dialogueIndex == 9 && !waitingOnObjective)
        {
            // Activate flag 3
            flags[2].SetActive(true);

            // Pan camera to flag 3
            camera.PanCamera(flags[2].transform);

            // Pause the dialogue
            dialogueManager.PauseDialogue();

            // Assign an objective
            waitingOnObjective = true;

            // Let player move
            player.UnstopPlayer();
        }
        else if (dialogueIndex == 9 && waitingOnObjective)
        {
            // Check if the player is near the flag
            if (Vector2.Distance(player.transform.position, flags[2].transform.position) < 0.5f)
            {
                // Play the Flag activate animation and set active
                flagActivateObjects[2].SetActive(true);

                // Unpause the dialogue and stop the player
                dialogueManager.ResumeDialogue();
                player.StopPlayer();

                waitingOnObjective = false;
            }
        }
        // Now for the inventory tutorial part... Basically just wait on objective.
        else if (dialogueIndex == 13 && !waitingOnObjective)
        {
            // Pause the dialogue
            dialogueManager.PauseDialogue();

            // Assign an objective
            waitingOnObjective = true;

            // Let player move
            player.UnstopPlayer();

            // inventory button set to active
            inventoryButton.SetActive(true);
        }
        else if (dialogueIndex == 13 && waitingOnObjective)
        {
            // Check if the player has closed the inventory
            if (inventoryClosed)
            {
                // Unpause the dialogue and stop the player
                dialogueManager.ResumeDialogue();
                player.StopPlayer();

                waitingOnObjective = false;
            }
        }

        else if (waitingOnObjective) {
            return;
        }

        // if dialogueIndex is maxed out
        else if (dialogueIndex == 17)
        {
            // End the tutorial
            EndTutorial();
        }
    }

    // End of tutorial, unstop the player and stop the dialogue
    public void EndTutorial()
    {
        dialoguePanel.SetActive(false);
        finishTutorialPanel.SetActive(true);
        blackoutPanel.SetActive(true);
    }
    
}
