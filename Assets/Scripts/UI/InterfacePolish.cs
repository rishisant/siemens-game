using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/** @brief Applies consistent feedback to the existing pixel UI in each scene. */
public static class InterfacePolish
{
    private static readonly HashSet<string> Panels=new HashSet<string>{"Menu_Panel","Inventory_Base","Shop_Panel","Achievements-Panel","Leaderboards_Panel","Deckmaster-Choices"};
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register() { SceneManager.sceneLoaded-=OnScene;SceneManager.sceneLoaded+=OnScene; }
    private static void OnScene(Scene scene,LoadSceneMode mode)
    {
        foreach(var root in scene.GetRootGameObjects())
        {
            foreach(var button in root.GetComponentsInChildren<Button>(true))
            {
                if(button.GetComponent<ButtonMotion>()==null)button.gameObject.AddComponent<ButtonMotion>();
                var colors=button.colors;colors.fadeDuration=.10f;
                colors.highlightedColor=new Color(1f,.97f,1f);colors.pressedColor=new Color(.77f,.78f,.9f);
                button.colors=colors;
            }
            foreach(var rect in root.GetComponentsInChildren<RectTransform>(true))
                if(Panels.Contains(rect.name)&&rect.GetComponent<PanelMotion>()==null)rect.gameObject.AddComponent<PanelMotion>();
        }
    }
}
