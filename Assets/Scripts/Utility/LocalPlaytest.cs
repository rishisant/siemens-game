using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/** @brief Guest practice session, with in-memory progress and no backend requests. */
public class LocalPlaytest : MonoBehaviour
{
    public static bool IsActive { get; private set; }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        IsActive = false;
#if UNITY_EDITOR
        if (!UnityEditor.SessionState.GetBool("ByteCity.Playtest", false)) return;
        UnityEditor.SessionState.SetBool("ByteCity.Playtest", false);
        IsActive = true;
        DontDestroyOnLoad(new GameObject("Local playtest").AddComponent<LocalPlaytest>().gameObject);
#endif
    }
    public static void StartGuest()
    {
        if (IsActive)
        {
            EnterWorld();
            return;
        }
        IsActive = true;
        DontDestroyOnLoad(new GameObject("Guest session").AddComponent<LocalPlaytest>().gameObject);
    }
    private IEnumerator Start()
    {
        yield return null;
        var player = PlayerData.Instance;
        if (player == null) { Debug.LogError("Start playtesting through Byte City > Play locally."); yield break; }
        player.username = PlayerPrefs.GetString("ByteCity.DisplayName", "Explorer"); player.userId = 0; player.coins = 1000;
        player.unlocked_items = new List<int> {108};
        player.unlocked_cards = new List<int>();
        player.unlocked_cards.AddRange(new[] {0,4,9,1,11,6});
        player.npc_interactions["sensei"] = PlayerPrefs.GetInt("ByteCity.TutorialComplete", 0) == 1 ? 2 : 0;
        if (GameManager.Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
        EnterWorld();
    }
    private static void EnterWorld()
    {
        if (TownMultiplayer.Instance != null) TownMultiplayer.Instance.ConnectToTown();
        Go(PlayerData.Instance.npc_interactions["sensei"] == 2 ? "Town_Square" : "Starting-Cutscene");
    }
    public static void ReplayTutorial()
    {
        PlayerData.Instance.npc_interactions["sensei"] = 0;
        Go("Tutorial");
    }
    private static void Go(string scene)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.playerSpawnPosition = scene == "Tutorial" ? new Vector2(-14.89f,0.23f) : scene == "Town_Square" ? new Vector2(-30.1f,20.49f) : new Vector2(-33f,10.25f);
        if (PlayerData.Instance != null) PlayerData.Instance.interactable = "none";
        SceneManager.LoadScene(scene);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) Go("Town_Square");
        if (Input.GetKeyDown(KeyCode.F2)) Go("PipeGame");
        if (Input.GetKeyDown(KeyCode.F3)) Go("WireGame");
        if (Input.GetKeyDown(KeyCode.F4)) Go("Laboratory_Main");
    }
    private void OnGUI()
    {
#if UNITY_EDITOR
        GUI.Box(new Rect(Screen.width / 2f - 219, 2, 438, 26), "PLAYTEST  |  F1 Town  F2 Pipes  F3 Wires  F4 Lab");
#endif
    }
}
