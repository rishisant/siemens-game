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

    private List<Button> itemButtons = new List<Button>(); // List to store button objects

    void Start()
    {
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
            
            newButton.onClick.AddListener(() => OnItemClicked(item));

            itemButtons.Add(newButton); // Add to the list
        }
    }

    private void OnItemClicked(CosmeticItem item)
    {
        // Handle item click logic here
        Debug.Log("Clicked on: " + item.itemName);
    }
}