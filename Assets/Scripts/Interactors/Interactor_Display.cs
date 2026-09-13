using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

/**
 * @class UserScoreList
 * @brief This class contains a list of `UserScore` objects
 * @details This class is used with the json utility to parse the user scores
 * that are returned from the API endpoint. It is important that these member
 * names stay the same (i.e. don't change to camel case) since they have to be
 * consistent with what's returned from the API
 * @see UserScore
 */
[System.Serializable]
public class UserScoreList
{
    public List<UserScore> users;
}

/**
 * @class UserScore
 * @brief This class is used to store the user's score and name
 * for the leaderboard
 * @details This class is used with the json utility to parse the userscores
 * that are returned from the API endpoint. It is important that these members
 * stay the same (i.e. don't change to camel case since they have to be
 * consistent with what's returned from the API)
 */
[System.Serializable]
public class UserScore
{
    // DON'T CHANGE THIS TO CAMEL CASE
    // since we are parsing JSON, the names need to match up with what is in
    // the JSON for this to serialize properly
    public string user_name;
    public double score;
}


/**
 * @class Interactor_Display
 * @brief This class is used to handle the display of the leaderboards
 */
public class Interactor_Display : MonoBehaviour
{
    // Call the GameObject for Leaderboards
    [SerializeField] private GameObject leaderboards;

    // Call the GameObject for the Base Game UI
    [SerializeField] private GameObject baseGameUI;

    // Call the GameObject for Player
    [SerializeField] private GameObject player;

    // Call the Inventory Panel
    [SerializeField] private GameObject inventoryPanel;

    // Menu Panel
    [SerializeField] private GameObject menuPanel;

    // Modal
    [SerializeField] private GameObject modal;

    [SerializeField] private TMP_Text top5;
    [SerializeField] private TMP_Text top10;

    // Grab the dialogueManager script object
    // the script is DialogueManager_TS on the Town_Square scene
    // and is on DialogueManager object
    [SerializeField] private DialogueManager_TS dialogueManager;

    // Now the dialoguemanager for casino
    [SerializeField] private DialogueManager_Casino dialogueManagerCasino;

    // Now the dialoguemanager for Lab
    [SerializeField] private DialogueManager_Lab dialogueManagerLab;

    // GameManager
    public GameManager gameManager => GameManager.Instance;
    public PlayerData playerData => PlayerData.Instance;

    // Error screen handling
    [SerializeField] private GameObject errorScreen;
    [SerializeField] private TMP_Text errorMessageText;

    // Defining open Menu
    public void OpenMenu()
    {
        menuPanel.SetActive(true);
        baseGameUI.SetActive(false);

        // Stop the player
        player.GetComponent<Character_Movement>().StopPlayer();
    }

    // Defining leave Menu
    public void LeaveMenu()
    {
        menuPanel.SetActive(false);
        baseGameUI.SetActive(true);

        // Unstop the player
        player.GetComponent<Character_Movement>().UnstopPlayer();
    }

    private void uploadPlayerDataOnExit()
    {
        string url = "https://g7fh351dz2.execute-api.us-east-1.amazonaws.com/default/PlayerDataUpload";
        string jsonData = System.String.Format(@"{{
            ""user_id"": {0},
            ""current_coins"": {1},
            ""items_owned"": {2},
            ""items_equipped"": {3},
            ""cards_owned"": {4},
            ""achievements_complete"": {5},
            ""achievements"": {6},
            ""has_finished_cutscene"": {7},
            ""location_x"": {8},
            ""location_y"": {9},
            ""current_scene"": {10},
            ""interactions"": {11}
        }}",
            playerData.userId.ToString(),
            playerData.coins.ToString(),
            "[" + string.Join(", ", playerData.unlocked_items) + "]",
            "[" + string.Join(", ", playerData.equipped_items) + "]",
            "[]",
            "[" + string.Join(", ", playerData.unlocked_achievements) + "]",
            "{}",
            "true",
            player.transform.position.x,
            player.transform.position.y,
            "\"Town_Square\"",
            "{" + string.Join(", ", playerData.npc_interactions.Select(kvp => $"\"{kvp.Key}\": {kvp.Value}")) + "}"
        );
        Debug.Log(jsonData);
        WebRequestUtility.SendWebRequest(this, url, jsonData, onExitRequestComplete, onExitRequestComplete);
    }

    public void onExitRequestComplete(string responseText)
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Defining the Exit Out for the Game
    public void ExitGame()
    {
        // need to upload player data on returning to main menu so that the database stays updated
        uploadPlayerDataOnExit();
    }

    // Defining the Exit Out for Leaderboards
    public void ExitLeaderboards()
    {
        leaderboards.SetActive(false);
        baseGameUI.SetActive(true);

        // Unstop the player
        player.GetComponent<Character_Movement>().UnstopPlayer();
    }

    // Defining the Exit Out for Inventory
    public void ExitInventory()
    {
        inventoryPanel.SetActive(false);
        baseGameUI.SetActive(true);
        modal.SetActive(false);

        // Unstop the player
        player.GetComponent<Character_Movement>().UnstopPlayer();
    }

    // Opening the Inventory
    public void OpenInventory()
    {
        inventoryPanel.SetActive(true);
        baseGameUI.SetActive(false);
        modal.SetActive(true);

        // Stop the player
        player.GetComponent<Character_Movement>().StopPlayer();
    }

    // Grab the playerData's interactable string
    // EX: If the playerData's interactable string is "leaderboard"
    // then we will show the leaderboards
    public string interactable
    {
        get { return PlayerData.Instance.interactable; }
    }

    public void DisplayPanel()
    {
        // Debug.Log("Display Panel" + interactable);
        // If the player walks into the collider for the leaderboard,
        // we will change the player's interactable to "leaderboard"
        // and show the interact button.
        if (interactable == "leaderboard")
        {
            // Debug.Log("Leaderboard Interact");
            leaderboards.SetActive(true);
            baseGameUI.SetActive(false);

            // Stop the player
            player.GetComponent<Character_Movement>().StopPlayer();
        } else if (interactable == "enterlaboratory")
        {
            // Debug.Log("Enter Laboratory"); -33, 10.25
            gameManager.ChangePlayerSpawnPosition(new Vector2(-33f, 10.25f));
            SceneManager.LoadScene("Laboratory_Main");
        } else if (interactable == "exitlaboratory")
        {
            // Debug.Log("Exit Laboratory");
            gameManager.ChangePlayerSpawnPosition(new Vector2(13.97f, -4.36f));
            SceneManager.LoadScene("Town_Square");
        } else if (interactable == "drunkard")
        {
            // Debug.Log("Drunkard Interact");
            dialogueManager.TalkToDrunkGuy();
        }
        else if (interactable == "shopowner")
        {
            // Debug.Log("Shop Owner Interact");
            dialogueManager.TalkToShopOwner();
        }
        // exit caisno and enter asino
        else if (interactable == "exitcasino")
        {
            // Debug.Log("Exit Casino");
            gameManager.ChangePlayerSpawnPosition(new Vector2(-22f, -1.5f));
            SceneManager.LoadScene("Town_Square");
        } else if (interactable == "entercasino")
        {
            // Debug.Log("Enter Casino"); //0.36,-9.37
            gameManager.ChangePlayerSpawnPosition(new Vector2(0.36f, -9.37f));
            SceneManager.LoadScene("Casino_Main");
        } else if (interactable == "casinoowner")
        {
            //none
            // Make sure to call the dialogueManagerCasino.TalkToCasinoOwner() function
            // to talk to the casino owner
            dialogueManagerCasino.CasinoOwnerSpeak();
        } else if (interactable == "pipegame")
        {
            // Debug.Log("Pipe Game Interact");
            // Save the current player vector to gamemanager
            gameManager.ChangePlayerSpawnPosition(player.transform.position);
            SceneManager.LoadScene("PipeGame");
        } else if (interactable == "wiregame")
        {
            // Debug.Log("Wire Game Interact");
            // Save the current player vector to gamemanager
            gameManager.ChangePlayerSpawnPosition(player.transform.position);
            SceneManager.LoadScene("WireGame");
        } else if (interactable == "deckmaster")
        {
            // Debug.Log("Deck Master Interact");
            dialogueManagerLab.DeckMasterSpeak();
        } else if (interactable == "cardgame")
        {
            // Debug.Log("Card Game Interact");
            // Save the current player vector to gamemanager
            gameManager.ChangePlayerSpawnPosition(player.transform.position);
            if (playerData.npc_interactions["deckmaster"] == 0)
            {
                dialogueManagerLab.DeckMasterInterrupt();
            } else
            {
            DeckmasterDuel.Show();
            }
        }
         else {
            // Debug.Log("No Interactable");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Find dialogueManager_TS if current scene is Town_Square
        if (SceneManager.GetActiveScene().name == "Town_Square")
        {
            if (dialogueManager == null)
            {
                dialogueManager = GameObject.Find("DialogueManager").GetComponent<DialogueManager_TS>();
            }
        }
    }

    public void GetPeculiarPipesLeaderboard()
    {
        Debug.Log("getting pipe leaderboard");
        getLeaderboard(5);
    }
    public void GetWackyWiresLeaderboard()
    {
        Debug.Log("getting wire leaderboard");
        getLeaderboard(7);
    }

    private void getLeaderboard(int gameId)
    {
        string url = "https://g7fh351dz2.execute-api.us-east-1.amazonaws.com/default/Leaderboard";
        string jsonData = System.String.Format(@"{{
            ""game_id"": {0}
        }}", gameId);

        WebRequestUtility.SendWebRequest(this, url, jsonData, setLeaderboardText, OnRequestFail);
    }

    void setLeaderboardText(string responseText)
    {
        if (responseText != null)
        {
            string json = System.String.Format(@"{{
                ""users"" : {0}
            }}", responseText);

            UserScoreList scores = JsonUtility.FromJson<UserScoreList>(json);

            int i = 1;
            string top5Text = "";
            string top10Text = "";

            foreach (UserScore user in scores.users)
            {
                string currText = System.String.Format("{0}. {1} : {2:00.00}\n", i, user.user_name, user.score);
                if (i < 6)
                {
                    top5Text += currText;
                }
                else if (i <= 10)
                {
                    top10Text += currText;
                }
                else
                {
                    break;
                }
                i++;
            }

            top5.text = top5Text;
            top10.text = top10Text;
        }
    }

    public void OnRequestFail(string responseText)
    {
        Debug.LogError($"Request failed with error message {responseText}");
        SetupErrorScreen("Error: Unable to get leaderboards, please check your internet connection.");

    }

    public void SetupErrorScreen(string errorMessage)
    {
        errorScreen.SetActive(true);
        errorMessageText.text = errorMessage;
    }

    public void ErrorExitButton()
    {
        errorScreen.SetActive(false);
    }
}
