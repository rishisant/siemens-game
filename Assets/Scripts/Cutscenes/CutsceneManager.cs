using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    // Grab the dialogueManager
    public DialogueManager_Cutscene dialogueManager;

    // Grab the dialogueIndex from the dialogueManager
    private int dialogueIndex => dialogueManager.dialogueIndex;

    // Grab the Finish-Intro panel and activate it
    [SerializeField] private GameObject finishIntroPanel;

    // Grab ModalBack and active it
    [SerializeField] private GameObject modalBack;
    
    // Start
    void Start()
    {
        // Start the cutscene
        dialogueManager.StartDialogue();
    }

    void Update()
    {
        if (dialogueManager.dialogueIndex == 24)
        {
            // Activate the finishIntroPanel
            finishIntroPanel.SetActive(true);
            // Activate the modalBack
            modalBack.SetActive(true);
        }
    }

    // Take to Tutorial Scene or Laboratory-L1
    public void toTutorialScene()
    {
        // Load the tutorial scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Tutorial");
    }

    // Take to Laboratory-L1
    public void toLaboratoryL1()
    {
        // Load the laboratory-l1 scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Laboratory_L1");
    }

    
}
