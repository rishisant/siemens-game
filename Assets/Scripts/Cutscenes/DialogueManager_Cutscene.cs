using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For the cutscene (there is no player on this scene)
// Just tapping the screen to go to the next dialogue
public class DialogueManager_Cutscene : MonoBehaviour
{
    // Get the dialogue panel, the textmeshpro text for the current dialogue
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMPro.TextMeshProUGUI dialogueText;

    // Get the character (called Character-Sprite) under the dialogue panel
    [SerializeField] private UnityEngine.UI.Image characterImage;

    // Serialize the dialogue array text
    [SerializeField] private string[] dialogues;
    
    // Serialize the character image it's associated with (in the array)
    [SerializeField] private int[] characterImageAssociations;

    // Let's serialize the images (2 reactions)
    [SerializeField] private Sprite[] characterImages = new Sprite[2];

    // The current index of the dialogue
    public int dialogueIndex = 0;

    // Typing speed
    [SerializeField] private float typingSpeed = 0.05f;

    // Serialize the Image UI for the cutscene
    [SerializeField] private UnityEngine.UI.Image cutsceneImage;

    // Serialize the list of images as well as the dialogue correspondence to the image
    [SerializeField] private Sprite[] cutsceneImages;
    [SerializeField] private int[] cutsceneImageAssociations;

    // Is the character typing?
    private bool isTyping = true;

    // Typing the sentence
    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            if (!isTyping)
            {
                dialogueText.text = sentence;
                break;
            }
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        isTyping = false;
    }

    // Start the dialogue
    public void StartDialogue()
    {
        dialoguePanel.SetActive(true);

        // Typing the sentence
        StartCoroutine(TypeSentence(dialogues[dialogueIndex]));
    }

    // Display the next sentence
    public void DisplayNextSentence()
    {
        // DEBUG: Just use if needed, the tutorial is autoended.
        // if (dialogueIndex < dialogues.Length - 1)
        // {
            // Stop all coroutines
            StopAllCoroutines();
            
            // Start Coroutine
            ClearDialogue();
            dialogueIndex++;

            // Typing the sentence
            isTyping = true;
            StartCoroutine(TypeSentence(dialogues[dialogueIndex]));

            // Set the character image
            characterImage.sprite = characterImages[characterImageAssociations[dialogueIndex]];

            // Set the cutscene image
            cutsceneImage.sprite = cutsceneImages[cutsceneImageAssociations[dialogueIndex]];
    }

    // Clear the dialogue
    public void ClearDialogue()
    {
        dialogueText.text = "";
    }

    // End the dialogue
    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // If the player taps the screen, display the next sentence
        if (Input.GetMouseButtonDown(0) && dialogueIndex != dialogues.Length)
        {
            if (dialogueIndex == dialogues.Length - 1)
            {
                EndDialogue();
                dialogueIndex++; // To allow for CutseneManager to activate the finishIntroPanel
            }
            else
            {
                DisplayNextSentence();
            }
        }
    }

    // Pause the dialogue
    public void PauseDialogue()
    {
        dialoguePanel.SetActive(false);
    }

    // Resume the dialogue
    public void ResumeDialogue()
    {
        dialoguePanel.SetActive(true);
        DisplayNextSentence();
    }
}
