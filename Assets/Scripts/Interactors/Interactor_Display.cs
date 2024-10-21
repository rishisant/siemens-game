using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class Interactor_Display : MonoBehaviour
{
    // Call the GameObject for Leaderboards
    [SerializeField] private GameObject leaderboards;

    // Call the GameObject for the Base Game UI
    [SerializeField] private GameObject baseGameUI;

    // Call the GameObject for Player
    [SerializeField] private GameObject player;

    // 1-5
    [SerializeField] private TMP_Text top5;
    // 6-10
    [SerializeField] private TMP_Text top10;


    // Defining the Exit Out for Leaderboards
    public void ExitLeaderboards()
    {
        leaderboards.SetActive(false);
        baseGameUI.SetActive(true);
    }

    // Grab the playerData's interactable string
    // EX: If the playerData's interactable string is "leaderboard"
    // then we will show the leaderboards
    private string interactable => player.GetComponent<PlayerData>().interactable;

    public void DisplayPanel()
    {
        if (interactable == "leaderboard")
        {
            leaderboards.SetActive(true);
            baseGameUI.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // DEBUG: Interactable testing
        // Debug.Log(interactable);
    }

    public void GetWackyWiresLeaderboard()
    {
        Debug.Log("getting leaderboard");
        getLeaderboard(7);
    }

    private void getLeaderboard(int gameId)
    {
        string url = "https://g7fh351dz2.execute-api.us-east-1.amazonaws.com/default/Leaderboard";
        string jsonData = System.String.Format(@"{{
            ""game_id"": {0}
        }}", gameId);

        WebRequestUtility.SendWebRequest(this, url, jsonData, setLeaderboardText);
    }

    void setLeaderboardText(string responseText)
    {
        if (responseText != null)
        {

            top5.text = "testing";
            top10.text = "bruh";
            Debug.Log(responseText);
        }
    }
}
