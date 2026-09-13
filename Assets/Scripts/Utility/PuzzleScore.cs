using System;
using System.Globalization;
using UnityEngine;

/** @brief Uploads a finished run independently of the scene displaying its results. */
public class PuzzleScore : MonoBehaviour
{
    public static string SerializeSeconds(TimeSpan time) { return time.TotalSeconds.ToString("F2", CultureInfo.InvariantCulture); }
    public static void Submit(int gameId, TimeSpan elapsed)
    {
        var player = PlayerData.Instance;
        if (player == null || player.userId <= 0 || LocalPlaytest.IsActive) return;
        var runner = new GameObject("Score upload").AddComponent<PuzzleScore>();
        DontDestroyOnLoad(runner.gameObject);
        string json = "{\"user_id\":" + player.userId + ",\"game_id\":" + gameId + ",\"score\":" + SerializeSeconds(elapsed) + "}";
        WebRequestUtility.SendWebRequest(runner, "https://g7fh351dz2.execute-api.us-east-1.amazonaws.com/default/ScoreUpload", json,
            _ => Destroy(runner.gameObject),
            error => { Debug.LogWarning("Score could not be saved: " + error); Destroy(runner.gameObject); });
    }
}
