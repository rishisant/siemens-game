using System;
using TMPro;
using UnityEngine;

/** @brief Pauses between rounds so reading results does not affect the score. */
public class LevelOverManager : MonoBehaviour
{
    public TMP_Text levelTimeElapsed;
    public WireGenerator wireGenerator;
    public void Setup(TimeSpan time)
    {
        FindObjectOfType<PuzzleHUD>().ShowResult("Round connected", "Round " + wireGenerator.level + " / 6\n\nTime  " + PuzzleHUD.FormatTime(time), "Next round", NextButton, ExitButton);
    }
    public void NextButton() { wireGenerator.StartLevel(); }
    public void ExitButton() { PuzzleHUD.ReturnToLab(); }
}
