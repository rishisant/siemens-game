using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BodyPartButton : MonoBehaviour
{
    public InventoryUI inventoryUI; // Reference to the InventoryUI script
    public BodyPart bodyPart; // The body part associated with this button

    void Start()
    {
        // Get the Button component and add the listener
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        // Call SetSelectedBodyPart on the InventoryUI instance
        inventoryUI.ToggleBodyPartVisibility(bodyPart);
    }
}
