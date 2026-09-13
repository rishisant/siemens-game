using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/** @brief Modal UI blocks movement and the world toolbar without changing tutorial movement state. */
public class WorldUiFocus : MonoBehaviour
{
    private static readonly List<WorldUiFocus> panels=new List<WorldUiFocus>();
    private static readonly HashSet<string> names=new HashSet<string>{"Shop_Panel","Inventory_Base","Menu_Panel","Achievements-Panel","Leaderboards_Panel","Deckmaster-Choices"};
    public static bool Blocked { get { if(DialogueSurface.AnyVisible)return true;foreach(var panel in panels)if(panel!=null&&panel.isActiveAndEnabled)return true;return false; } }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register(){panels.Clear();SceneManager.sceneLoaded-=OnScene;SceneManager.sceneLoaded+=OnScene;}
    private static void OnScene(Scene scene,LoadSceneMode mode)
    {
        foreach(var root in scene.GetRootGameObjects())foreach(var rect in root.GetComponentsInChildren<RectTransform>(true))if(names.Contains(rect.name)&&rect.GetComponent<WorldUiFocus>()==null)rect.gameObject.AddComponent<WorldUiFocus>();
    }
    private void OnEnable(){if(!panels.Contains(this))panels.Add(this);}
    private void OnDestroy(){panels.Remove(this);}
}
