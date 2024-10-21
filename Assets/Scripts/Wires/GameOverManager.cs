using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking; 
using TMPro;

/**
 * This class handles the game over screen and behavior. It is called by the WireGenerator
 * @see WireGenerator
 */
public class GameOverManager : MonoBehaviour
{
    public TMP_Text levelTimeElapsed;
    public TMP_Text gameTimeElapsed;

    private string score;
    /**
     * Setup() sets the game over screen to be active so it will actually show up when called.
     * 
     * @param levelTimeSpan The timespan of the elapsed time spent on a level
     * @param gameTimeSpan The timespan of the elapsed time spent on the entire game
     */
    public void Setup(System.TimeSpan levelTimeSpan, System.TimeSpan gameTimeSpan)
    {
        levelTimeElapsed.text = "Level Time: " + System.String.Format("{0:00}:{1:00}.{2:00}",
            levelTimeSpan.Minutes, levelTimeSpan.Seconds,
            levelTimeSpan.Milliseconds / 10);

        gameTimeElapsed.text = "Game Time: " + System.String.Format("{0:00}:{1:00}.{2:00}",
            gameTimeSpan.Minutes, gameTimeSpan.Seconds,
            gameTimeSpan.Milliseconds / 10);

        score = System.String.Format("{0}.{1}", gameTimeSpan.Minutes * 60 + gameTimeSpan.Seconds, gameTimeSpan.Milliseconds / 10);

        gameObject.SetActive(true);
    }

    /**
     * RestartButton() is a function that is attached to the Restart button on the game over screen.
     * When this button is pressed, the scene is reloaded using the SceneManager
     */
    public void RestartButton()
    {
        // FIXME: replace with current user_id
        uploadTime(12);
        SceneManager.LoadScene("WireGame");
    }

    /**
     * ExitButton() is a function that is attached to the Exit button on the game over screen.
     * When this button is pressed, the player will be returned to the Laboratory via the SceneManager
     */
    public void ExitButton()
    {
        // FIXME: replace with current user_id
        uploadTime(12);
        SceneManager.LoadScene("Laboratory_L1");
    }

    /**
     * uploadTime() is a function that sends a POST request to the backend to upload the time
     */
    private void uploadTime(int userId)
    {
        string url = "https://g7fh351dz2.execute-api.us-east-1.amazonaws.com/default/ScoreUpload";
        string jsonData = System.String.Format(@"{{
            ""user_id"": {0},
            ""game_id"": {1},
            ""score"": {2}
        }}", userId, 7, score);
        Debug.Log("uploading score " + score);
        WebRequestUtility.SendWebRequest(this, url, jsonData, OnRequestComplete);
    }

    void OnRequestComplete(string responseText)
    {
        Debug.Log(responseText);
    }

}
