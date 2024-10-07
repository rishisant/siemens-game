using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public TMP_Text timeElapsed;

    public void Setup(System.TimeSpan ts)
    {
        Debug.Log(ts);
        string elapsedTime = System.String.Format("{0:00}:{1:00}.{2:00}",
            ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        timeElapsed.text = elapsedTime;
        gameObject.SetActive(true);
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("WireRandomGenScene");
    }

    public void ExitButton()
    {
        Debug.Log("exit button pressed");
        SceneManager.LoadScene("MainMenuScene");
    }
}
