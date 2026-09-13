using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief This handles the dialogue for sensei during the tutorial scene
 *
 * @see DialogueManager
 */
public class DialogueManagerTutorial : DialogueManagerBase
{
    // SerializeFields all things
    [SerializeField] private Character_Movement playerMovement;

    // Get the dialogue panel (the dialogue text lives in DialogueManagerBase)
    [SerializeField] private GameObject dialoguePanel;

    // Get the character (called Character-Sprite) under the dialogue panel
    [SerializeField] private UnityEngine.UI.Image characterImage;

    // Serialize the dialogue array text
    [SerializeField] private string[] dialogues;

    // Serialize the character image it's associated with (in the array)
    [SerializeField] private int[] characterImageAssociations;

    // Let's serialize the images (4 reactions)
    [SerializeField] private Sprite[] characterImages = new Sprite[4];

    // Make the UI buttons serialized
    // Just activate and deactivate the UI
    [SerializeField] private GameObject UI;

    // The current index of the dialogue
    public int dialogueIndex = 0;

    // Is dialoguePaused and don't check for mouse?
    private bool dialoguePaused = false;

    // Start the dialogue
    public void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        UI.SetActive(false);

        // Typing the sentence
        StartCoroutine(TypeSentence(dialogues[dialogueIndex]));

        // Set the character image
        characterImage.sprite = characterImages[characterImageAssociations[dialogueIndex]];
    }

    // Display the next sentence
    public void DisplayNextSentence()
    {
        if (!dialoguePanel.activeSelf || dialoguePaused) return;
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.maxVisibleCharacters = int.MaxValue;
            isTyping = false;
            return;
        }
        // Stop all coroutines
        StopAllCoroutines();

        // Start Coroutine
        ClearDialogue();
        dialogueIndex++;

        // Typing the sentence
        isTyping = true;
        // if index is not out of the bounds
        if (dialogueIndex < dialogues.Length)
        {
            StartCoroutine(TypeSentence(dialogues[dialogueIndex]));
        }
        else
        {
            EndDialogue();
            return;
        }

        // Set the character image
        characterImage.sprite = characterImages[characterImageAssociations[dialogueIndex]];
    }

    // Clear the dialogue
    public void ClearDialogue()
    {
        dialogueText.text = "";
    }

    // All dialogue finished
    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }

    // Pause dialogue and remove panel, and decrement the index (since the player will tap again)
    public void PauseDialogue()
    {
        dialoguePanel.SetActive(false);
        dialoguePaused = true;
        UI.SetActive(true);
    }

    // Resume dialogue
    public void ResumeDialogue()
    {
        dialoguePanel.SetActive(true);
        dialoguePaused = false;
        isTyping = false;
        DisplayNextSentence();
        UI.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set the UI to false
        UI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) && !dialoguePaused && dialoguePanel.activeSelf)
        {
            DisplayNextSentence();
        }
    }
}
