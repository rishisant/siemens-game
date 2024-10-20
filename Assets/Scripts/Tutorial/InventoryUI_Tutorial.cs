using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rishi Santhanam
// Manages the tutorial aspect of the inventory, basically if you press the inventory button
// the inventory panel should open, etc. and once the back button is pressed, it will remove
// the whole inventory interface and change its main boolean (public) to false, so the
// tutorial manager script can move on and finish the tutorial.

public class InventoryUI_Tutorial : MonoBehaviour
{
    // SerializeField all the UI objects
    // such as the inventory panel, inventory button
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject inventoryButton;

    [SerializeField] private GameObject blackoutPanel;

    // Now the close button
    [SerializeField] private GameObject closeButton;

    // The tutorial manager
    [SerializeField] private TutorialManager tutorialManager;

    // Now, create a public bool called inventoryClosed
    // to check if the inventory is closed
    public bool inventoryClosed = false; // This will turn to true and will be accessed later by the tutorial manager

    // Create the functions for the closeButton and inventoryPanel to set to active and inventoryButton
    // to set to inactive
    public void OpenInventory()
    {
        inventoryPanel.SetActive(true);
        inventoryButton.SetActive(false);

        // Blackout the panel
        blackoutPanel.SetActive(true);
    }

    public void CloseInventory()
    {
        // In the case of the tutorial, we're going to change the inventoryClosed to true
        inventoryPanel.SetActive(false);
        inventoryButton.SetActive(true);

        // Blackout the panel removed
        blackoutPanel.SetActive(false);

        inventoryClosed = true; // This is not necessary for the actual game, but for the tutorial
        // And also, deactivate the inventoryButton
        inventoryButton.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set the inventory panel to false
        inventoryPanel.SetActive(false);
    }
    
}
