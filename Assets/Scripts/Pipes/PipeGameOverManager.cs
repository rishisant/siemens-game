using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/** @brief Presents a completed pipe run and submits its score exactly once. */
public class PipeGameOverManager : MonoBehaviour
{
    public TMP_Text timeElapsed;
    private bool submitted;
    public void Setup(TimeSpan time)
    {
        if (submitted) return;
        submitted = true;
        PuzzleScore.Submit(5, time);
        if (PlayerData.Instance != null) PlayerData.Instance.pipe_puzzle_wins++;
        var hud = FindObjectOfType<PuzzleHUD>();
        hud.ShowResult("Circuit complete!", "All 3 boards connected\n\nRun time  " + PuzzleHUD.FormatTime(time), "Play again", RestartButton, ExitButton);
    }
    public void RestartButton() { SceneManager.LoadScene("PipeGame"); }
    public void ExitButton() { PuzzleHUD.ReturnToLab(); }
}
