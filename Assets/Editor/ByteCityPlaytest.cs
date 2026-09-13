using UnityEditor;
using UnityEditor.SceneManagement;

/** @brief Starts a self-contained local game from the editor menu. */
public static class ByteCityPlaytest
{
    [MenuItem("Byte City/Play locally")]
    public static void PlayLocally()
    {
        if (EditorApplication.isPlaying) return;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        SessionState.SetBool("ByteCity.Playtest", true);
        EditorSceneManager.OpenScene("Assets/Scenes/Initial-Scenes/MainMenu.unity");
        EditorApplication.isPlaying = true;
    }
}
