using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TextMeshPro

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory; // Reference to the Inventory
    public GameObject buttonPrefab; // Reference to the button prefab
    public Transform buttonContainer; // Parent for the buttons
    public BodyPart selectedBodyPart;
    public Dictionary<BodyPart, bool> bodyPartVisibility;

    private List<Button> itemButtons = new List<Button>(); // List to store button objects

    void Start()
    {
        bodyPartVisibility = new Dictionary<BodyPart, bool>
        {
            { BodyPart.Head, true },
            { BodyPart.Body, true },
            { BodyPart.Arms, true },
            { BodyPart.Legs, true },
            { BodyPart.Accessories, true }
            // Initialize all parts to visible
        };

        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        // Clear existing buttons
        foreach (Button button in itemButtons)
        {
            Destroy(button.gameObject);
        }
        itemButtons.Clear();

        // Create buttons for each cosmetic item
        foreach (CosmeticItem item in inventory.cosmeticItems)
        {
            GameObject newButtonObject = Instantiate(buttonPrefab, buttonContainer);
            Button newButton = newButtonObject.GetComponent<Button>();
            
            //TextMeshProUGUI buttonText = newButtonObject.GetComponentInChildren<TextMeshProUGUI>();
            //buttonText.text = item.itemName;
            
            Image iconImage = newButtonObject.transform.Find("IconImage").GetComponent<Image>();
            iconImage.sprite = item.itemIcon; // Set the cosmetic item's icon
            
            bool shouldShow = bodyPartVisibility[item.bodyPart];
            newButtonObject.SetActive(shouldShow);

            newButton.onClick.AddListener(() => OnItemClicked(item));

            itemButtons.Add(newButton); // Add to the list
        }
    }
    
    private void OnItemClicked(CosmeticItem item)
    {
        // Handle item click logic here
        Debug.Log("Clicked on: " + item.itemName);
    }

    public void ToggleBodyPartVisibility(BodyPart bodyPart)
    {
        if (bodyPartVisibility.ContainsKey(bodyPart))
        {
            bodyPartVisibility[bodyPart] = !bodyPartVisibility[bodyPart]; // Toggle visibility
            UpdateInventoryUI(); // Refresh the UI to reflect changes
        }
    }

    private void ResetBodyPartVisibility()
    {
        // Create a temporary list of keys to avoid modifying the dictionary while iterating
        List<BodyPart> keys = new List<BodyPart>(bodyPartVisibility.Keys);
        
        foreach (BodyPart part in keys)
        {
            bodyPartVisibility[part] = true; // Set all body parts to visible
        }
    }
    
    // Call this method to show all cosmetics
    public void ShowAll()
    {
        ResetBodyPartVisibility(); // Reset visibility to show all cosmetics
        UpdateInventoryUI(); // Refresh the UI to reflect this
    }

    // New method to handle the inventory button click
    public void OnInventoryButtonClick()
    {
        ShowAll(); // Show all cosmetics when the inventory button is clicked
    }
}