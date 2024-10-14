using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowAllButton : MonoBehaviour
{
    public InventoryUI inventoryUI; // Reference to the InventoryUI script

    void Start()
    {
        // Get the Button component and add the listener
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        // Call ShowAll on the InventoryUI instance
        inventoryUI.ShowAll();
    }
}
