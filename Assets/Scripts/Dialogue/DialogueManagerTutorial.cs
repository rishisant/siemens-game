using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rishi Santhanam
// This is for the sensei at the tutorial portion of the game.
public class DialogueManagerTutorial : MonoBehaviour
{
    // SerializeFields all things
    [SerializeField] private Character_Movement playerMovement;

    // Get the dialogue panel, the textmeshpro text for the current dialogue
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMPro.TextMeshProUGUI dialogueText;

    // Get the character (called Character-Sprite) under the dialogue panel
    [SerializeField] private UnityEngine.UI.Image characterImage;

    // Serialize the dialogue array text
    [SerializeField] private string[] dialogues;
    
    // Serialize the character image it's associated with (in the array)
    [SerializeField] private int[] characterImageAssociations;

    // Let's serialize the images (4 reactions)
    [SerializeField] private Sprite[] characterImages = new Sprite[4];

    // The current index of the dialogue
    public int dialogueIndex = 0;

    // Typing speed
    [SerializeField] private float typingSpeed = 0.05f;

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

        // Set the character image
        characterImage.sprite = characterImages[characterImageAssociations[dialogueIndex]];
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
        // }
        // else
        // {
        //     EndDialogue();
        // }
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
    }

    // Resume dialogue
    public void ResumeDialogue()
    {
        dialoguePanel.SetActive(true);
        DisplayNextSentence();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClearDialogue();
            DisplayNextSentence();
        }
    }
}
