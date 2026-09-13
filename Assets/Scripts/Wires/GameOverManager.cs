using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/** @brief Presents a completed wire run and submits its score once. */
public class GameOverManager : MonoBehaviour
{
    public TMP_Text levelTimeElapsed;
    public TMP_Text gameTimeElapsed;
    private bool submitted;
    public void Setup(TimeSpan levelTime, TimeSpan gameTime)
    {
        if (submitted) return;
        submitted = true;
        PuzzleScore.Submit(7, gameTime);
        if (PlayerData.Instance != null) PlayerData.Instance.wire_puzzle_fullround++;
        FindObjectOfType<PuzzleHUD>().ShowResult("Power restored!", "All 6 rounds connected\n\nActive time  " + PuzzleHUD.FormatTime(gameTime), "Play again", RestartButton, ExitButton);
    }
    public void RestartButton() { SceneManager.LoadScene("WireGame"); }
    public void ExitButton() { PuzzleHUD.ReturnToLab(); }
}
