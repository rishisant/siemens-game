using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/**
 * @brief This class handles the dialogue. It involves methods for opening
 * dialogue boxes, handling the "typing" of text, the skip buttons, and
 * cleaning up dialogue objects when the dialogue is finished
 */
public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Character_Movement playerMovement;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image characterImage;

    public Animator animator;

    private Queue<string> sentences = new Queue<string>();

    private List<GameObject> interactiveButtons = new List<GameObject>();

    private Queue<CharacterImage> characterImages = new Queue<CharacterImage>();


    void Start()
    {
        // Find all buttons with the ButtonLocation script
        ButtonLocation[] buttonLocations = FindObjectsOfType<ButtonLocation>();
        foreach (var button in buttonLocations)
        {
            interactiveButtons.Add(button.gameObject);
            button.gameObject.SetActive(false); // Hide buttons initially
        }

        if (playerMovement == null)
        {
            playerMovement = FindObjectOfType<Character_Movement>(); // Optional: Find the Player_Movement component in the scene
        }
    }

    public void StartDialogue (Dialogue dialogue) {
        animator.SetBool("IsOpen", true);

        nameText.text = dialogue.name;
        sentences.Clear();
        characterImages.Clear();

        foreach (string sentence in dialogue.sentences){
            sentences.Enqueue(sentence);
        }

        foreach (CharacterImage characterImage in dialogue.characterImages) {
            for(int i = 0; i <= characterImage.associatedValue; i++)
                characterImages.Enqueue(characterImage);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence () {
        if (sentences.Count == 0){
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));

        if (characterImages.Count > 0) {
            CharacterImage characterImageData = characterImages.Dequeue(); // Get character image data
            characterImage.sprite = characterImageData.sprite; // Set the new character image
            // You can access characterImageData.associatedValue if needed
        }

    }

    IEnumerator TypeSentence (string sentence) {
        // Reveal the pre-set sentence one character per frame instead of
        // rebuilding the string on every letter
        dialogueText.text = sentence;
        try {
            dialogueText.maxVisibleCharacters = 0;
            for (int visible = 1; visible <= sentence.Length; visible++) {
                dialogueText.maxVisibleCharacters = visible;
                yield return null;
            }
        } finally {
            dialogueText.maxVisibleCharacters = int.MaxValue;
        }
    }

    void EndDialogue(){
        animator.SetBool("IsOpen", false);
        ButtonLocation.SetClickedForAllButtons();
        //ShowInteractiveButtons();
        playerMovement.ToggleMovement();
    }

    // New method to activate the buttons
    public void ShowInteractiveButtons() {
        foreach (var button in interactiveButtons) {
            ButtonLocation buttonLocation = button.GetComponent<ButtonLocation>();
            Vector2 buttonPosition = buttonLocation.GetButtonPosition();
            //Debug.Log($"Checking button '{button.name}' at position: {buttonPosition}");

            buttonLocation.clicked = false; // Show all interactive buttons
        }
    }

}
