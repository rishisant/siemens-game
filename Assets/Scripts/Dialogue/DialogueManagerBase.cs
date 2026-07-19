using System.Collections;
using UnityEngine;

/**
 * @brief Shared base for the scene dialogue managers. Owns the text box,
 * typing speed, and the typewriter coroutine so each scene's manager only
 * has to provide its own conversations.
 *
 * @see DialogueManager_TS, DialogueManager_Lab, DialogueManager_Casino,
 * DialogueManagerTutorial, DialogueManager_Cutscene
 */
public abstract class DialogueManagerBase : MonoBehaviour
{
    // The text box the sentences are typed into
    public TMPro.TextMeshProUGUI dialogueText;

    // The speed at which the text is typed (seconds per character)
    [SerializeField] protected float typingSpeed = 0.025f;

    // Whether the typewriter is currently revealing a sentence. Set to false
    // while a sentence is typing to skip to the full sentence
    protected bool isTyping = true;

    protected IEnumerator TypeSentence (string sentence)
    {
        // Set the full sentence once and reveal it character by character,
        // instead of rebuilding the string and re-laying-out the text box
        // on every letter
        dialogueText.text = sentence;
        WaitForSeconds wait = new WaitForSeconds(typingSpeed);
        try
        {
            dialogueText.maxVisibleCharacters = 0;
            for (int visible = 1; visible <= sentence.Length; visible++)
            {
                if (!isTyping)
                {
                    break;
                }
                dialogueText.maxVisibleCharacters = visible;
                yield return wait;
            }
        }
        finally
        {
            // Also runs when the coroutine is stopped mid-sentence, so the
            // text box never gets stuck partially revealed
            dialogueText.maxVisibleCharacters = int.MaxValue;
        }

        isTyping = false;
    }
}
