using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/**
 * This class handles the level over screen and behavior. It is called by the
 * WireGenerator and also calls the WireGenerator when we want to start a new
 * level 
 * @see WireGenerator
 */
public class LevelOverManager : MonoBehaviour
{
    public TMP_Text levelTimeElapsed;
    public WireGenerator wireGenerator;

    /**
     * Setup() sets up the screen for the level being over. This sets up a
     * level over screen that gives the amount of time the player took on the
     * level, and a Next and Exit button.
     * 
     * @param levelTimeSpan The System.TimeSpan of the level time
     */
    public void Setup(System.TimeSpan levelTimeSpan)
    {
        levelTimeElapsed.text = "Level Time: " + System.String.Format("{0:00}:{1:00}.{2:00}",
            levelTimeSpan.Minutes, levelTimeSpan.Seconds,
            levelTimeSpan.Milliseconds / 10);

        gameObject.SetActive(true);
    }

    /**
     * NextButton() is a function that is attached to the Next button on the
     * level over screen. When this button is pressed, the level over screen
     * becomes inactive and the next level is loaded
     */
    public void NextButton()
    {
        gameObject.SetActive(false);
        wireGenerator.StartLevel();
    }

    /**
     * ExitButton() is a function that is attached to the Exit button on the game over screen.
     * When this button is pressed, the player will be returned to the Laboratory via the SceneManager
     */
    public void ExitButton()
    {
        SceneManager.LoadScene("Laboratory_L1");
    }
}
