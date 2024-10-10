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
        string elapsedTime = System.String.Format("{0:00}:{1:00}.{2:00}",
            ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        timeElapsed.text = elapsedTime;
        gameObject.SetActive(true);
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("WireGame");
    }

    public void ExitButton()
    {
        // FIXME: add a coordinate here so that we know where to spawn so they
        // can spawn back at their computer?
        SceneManager.LoadScene("Laboratory_L1");
    }
}