using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rishi Santhanam
// Main menu manager script will be responsible for the functions that the
// buttons on the main menu are assigned to. It will cause certain modals to
// appear and disappear, and will also load the game scene when the play button
// is clicked.
public class MainMenuManager : MonoBehaviour
{
    // Serialize all panels in the game needed
    // in GameObject form (for simple act/deact)
    [SerializeField] private GameObject loginPanel; // Base login panel
    [SerializeField] private GameObject registerPanel; // Base register panel
    [SerializeField] private GameObject loginregAbstractPanel; // The panel responsible for abstracting the login and register panels

    // Serialize the modal blackOut
    [SerializeField] private GameObject blackOutModal;

    // Serialize the settings and credits panels
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    // For beta testing, without logging in you will go to starting cutscene
    public void PlayGame()
    {
        // Load the game scene starting cutscene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Starting-Cutscene");
    }

    // Close button functions for login,register,abstract,settings,credits
    public void CloseLoginPanel()
    {
        loginPanel.SetActive(false);
        blackOutModal.SetActive(false);
    }

    public void CloseRegisterPanel()
    {
        registerPanel.SetActive(false);
        blackOutModal.SetActive(false);
    }

    public void CloseLoginRegAbstractPanel()
    {
        loginregAbstractPanel.SetActive(false);
        blackOutModal.SetActive(false);
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
        blackOutModal.SetActive(false);
    }

    public void CloseCreditsPanel()
    {
        creditsPanel.SetActive(false);
        blackOutModal.SetActive(false);
    }

    // Open button functions for login,register,abstract,settings,credits
    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        blackOutModal.SetActive(true);

        // Make the abstract panel disappear
        loginregAbstractPanel.SetActive(false);
    }

    public void OpenRegisterPanel()
    {
        registerPanel.SetActive(true);
        blackOutModal.SetActive(true);

        // Make the abstract panel disappear
        loginregAbstractPanel.SetActive(false);
    }

    public void OpenLoginRegAbstractPanel()
    {
        loginregAbstractPanel.SetActive(true);
        blackOutModal.SetActive(true);
    }

    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
        blackOutModal.SetActive(true);
    }

    public void OpenCreditsPanel()
    {
        creditsPanel.SetActive(true);
        blackOutModal.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
