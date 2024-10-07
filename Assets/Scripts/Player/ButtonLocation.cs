using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ButtonLocation : MonoBehaviour
{
    public float interactionDistance;
    public bool clicked = false;
    
    private static List<ButtonLocation> allButtonLocations = new List<ButtonLocation>();

    // This method returns the world position of the button
    public Vector2 GetButtonPosition()
    {
        return transform.position; // Get the position of the button in world space
    }

    private void Awake()
    {
        // Add this instance to the static list
        allButtonLocations.Add(this);
    }

    public void OnButtonClicked()
    {
        // Make the button disappear
        SetClickedForAllButtons();
        gameObject.SetActive(false); // Hides the button

        // Change the clicked variable for all other buttons
        
    }

    // Method to set the clicked variable for all buttons
    public static void SetClickedForAllButtons()
    {
        foreach (var button in allButtonLocations)
        {
            button.clicked = !button.clicked; // Set the clicked variable
        }
    }

}
