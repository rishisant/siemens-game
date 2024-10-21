using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTutorial : MonoBehaviour
{
    // Take to the Laboratory Scene
    public void TakeToLaboratory()
    {
        // Load the laboratory scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Laboratory_L1");
    }
}
