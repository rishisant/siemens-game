using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/** @brief Screen-aligned HUD proxies keep original button actions and visibility rules. */
public class DesktopHudLayout : MonoBehaviour
{
    private class Action {public Button source,view;public TMP_Text label;}
    private readonly List<Action> actions=new List<Action>();
    private RectTransform canvas,safe;
    private GameObject currencySource;
    private TMP_Text coins;
    private Character_Movement player;
    private Action interaction;
    private int width,height;
    private void Start()
    {
        player=GetComponent<Character_Movement>();
        var original=FindObjectsOfType<RectTransform>(true);
        canvas=WorldUi.Canvas("Town HUD",55);safe=WorldUi.Rect("Safe area",canvas,Vector2.zero,Vector2.zero,Vector2.zero,Vector2.zero);WorldUi.SafeArea(safe);
        foreach(var rect in original)
        {
            switch(rect.name)
            {
                case "UI_Joystick":if(!MobileControls.Enabled)rect.gameObject.SetActive(false);break;
                case "UI_Button_Menu":Add(rect,"Menu",new Vector2(64,62),Vector2.up,new Vector2(18,-18),true);break;
                case "UI_Button_Inventory":Add(rect,"Bag",new Vector2(72,62),Vector2.right,new Vector2(-18,18),true);break;
                case "UI_Button_Achievements":Add(rect,"Awards",new Vector2(72,62),Vector2.right,new Vector2(-98,18),true);break;
                case "UI_Button_Dance":Add(rect,"Emote",new Vector2(72,62),Vector2.right,new Vector2(-178,18),true);break;
                case "UI_Button_Interact":interaction=Add(rect,"Interact  [E]",new Vector2(260,42),new Vector2(.5f,0),new Vector2(0,22),false);break;
                case "Currency":
                    currencySource=rect.gameObject;HideOriginal(rect.gameObject);
                    var panel=WorldUi.Rect("Coins",safe,Vector2.one,Vector2.one,new Vector2(-18,-18),new Vector2(130,42));
                    
                    var image=rect.GetComponent<Image>();if(image!=null){var icon=WorldUi.Rect("Coin",panel,Vector2.zero,Vector2.zero,new Vector2(12,9),new Vector2(30,30)).gameObject.AddComponent<Image>();icon.sprite=image.sprite;icon.preserveAspect=true;}
                    coins=WorldUi.Text("Balance",panel,"",22,WorldUi.Paper);
                    var originalText=rect.GetComponentInChildren<TMP_Text>(true);if(originalText!=null){coins.font=originalText.font;coins.fontSharedMaterial=originalText.fontSharedMaterial;}
                    coins.enableAutoSizing=true;coins.fontSizeMin=14;coins.fontSizeMax=22;WorldUi.Place(coins.rectTransform,Vector2.zero,Vector2.zero,new Vector2(46,5),new Vector2(78,32));break;
            }
        }
    }
    private static void HideOriginal(GameObject obj)
    {
        var group=obj.GetComponent<CanvasGroup>();if(group==null)group=obj.AddComponent<CanvasGroup>();group.alpha=0;group.blocksRaycasts=false;
    }
    private Action Add(RectTransform source,string name,Vector2 size,Vector2 anchor,Vector2 pos,bool icon)
    {
        var button=source.GetComponent<Button>();if(button==null)return null;
        var view=WorldUi.Button(name,safe,name,size,()=>button.onClick.Invoke());
        WorldUi.Place(view.GetComponent<RectTransform>(),anchor,anchor,pos,size);
        var motion=view.GetComponent<ButtonMotion>();motion.CaptureScale();
        var label=view.GetComponentInChildren<TMP_Text>();
        if(icon)
        {
            view.GetComponent<UiSurface>().enabled=false;
            var original=source.GetComponent<Image>();var art=WorldUi.Rect("Icon",view.transform,new Vector2(.5f,1),new Vector2(.5f,1),new Vector2(0,-1),new Vector2(40,40)).gameObject.AddComponent<Image>();art.sprite=original.sprite;art.preserveAspect=true;art.raycastTarget=true;view.targetGraphic=art;
            WorldUi.Place(label.rectTransform,Vector2.zero,Vector2.zero,new Vector2(2,4),new Vector2(size.x-4,21));label.fontSize=12;label.fontSizeMax=12;label.color=WorldUi.Paper;
        }
        HideOriginal(source.gameObject);var action=new Action{source=button,view=view,label=label};actions.Add(action);return action;
    }
    private void Update()
    {
        if(safe==null)return;
        if(width!=Screen.width||height!=Screen.height){width=Screen.width;height=Screen.height;WorldUi.SafeArea(safe);}
        bool free=player!=null&&player.CanMove;
        foreach(var action in actions)
        {
            if(action.source==null)continue;
            action.view.gameObject.SetActive(free&&action.source.gameObject.activeInHierarchy);
            action.view.interactable=action.source.interactable;
        }
        if(coins!=null){coins.text=PlayerData.Instance.coins.ToString();coins.transform.parent.gameObject.SetActive(currencySource!=null&&currencySource.activeInHierarchy);}
        if(interaction!=null&&interaction.view.gameObject.activeSelf)
        {
            interaction.label.text=InteractionName(PlayerData.Instance.interactable)+"  [E]";
            if(Input.GetKeyDown(KeyCode.E)&&interaction.source.interactable)interaction.source.onClick.Invoke();
        }
    }
    private static string InteractionName(string target)
    {
        switch(target)
        {
            case "deckmaster":return "Talk to Deckmaster";case "shopowner":return "Talk to Ethan";case "drunkard":return "Talk";
            case "pipegame":return "Play Pipes";case "wiregame":return "Play Wires";case "cardgame":return "Play Card Jitsu";
            case "enterlaboratory":return "Enter laboratory";case "entercasino":return "Enter casino";
            case "exitlaboratory":case "exitcasino":return "Return to town";default:return "Interact";
        }
    }
    private void OnDestroy(){if(canvas!=null)Destroy(canvas.gameObject);}
}
