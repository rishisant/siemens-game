using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/** @brief A screen-aligned conversation card, preserving the scene's dialogue references and objectives. */
[DefaultExecutionOrder(600)]
public class DialogueSurface : MonoBehaviour
{
    private static readonly List<DialogueSurface> all=new List<DialogueSurface>();
    public static bool AnyVisible { get { foreach(var item in all)if(item!=null && item.gameObject.activeInHierarchy)return true;return false; } }
    private DialogueManagerBase manager;
    private RectTransform safe, panel;
    private TMP_Text speaker, hint, actionLabel;
    private Image portrait;
    private Button advance;
    private int width,height;
    private string previousText;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register() { all.Clear();SceneManager.sceneLoaded-=OnScene;SceneManager.sceneLoaded+=OnScene; }
    private static void OnScene(Scene scene,LoadSceneMode mode)
    {
        if(scene.name=="Starting-Cutscene" || scene.name=="MainMenu")return;
        foreach(var manager in Object.FindObjectsOfType<DialogueManagerBase>(true))
        {
            if(manager.dialogueText==null || manager.gameObject.scene!=scene)continue;
            Transform root=manager.dialogueText.transform;
            while(root!=null && root.name!="Dialogue-Panel")root=root.parent;
            if(root==null || root.GetComponent<DialogueSurface>()!=null)continue;
            var component=root.gameObject.AddComponent<DialogueSurface>();component.Configure(manager);
        }
    }
    private void Configure(DialogueManagerBase source)
    {
        manager=source;panel=(RectTransform)transform;all.Add(this);
        var canvas=WorldUi.Canvas("Conversation UI",65);
        safe=WorldUi.Rect("Safe area",canvas,Vector2.zero,Vector2.zero,Vector2.zero,Vector2.zero);WorldUi.SafeArea(safe);
        foreach(var label in GetComponentsInChildren<TMP_Text>(true))
        {
            if(label.name=="Char-Name")speaker=label;
            else if(label.name=="Text-TTC")hint=label;
        }
        foreach(var image in GetComponentsInChildren<Image>(true))if(image.name=="Character-Sprite")portrait=image;
        manager.dialogueText.transform.SetParent(panel,false);
        if(speaker!=null)speaker.transform.SetParent(panel,false);
        if(portrait!=null)portrait.transform.SetParent(panel,false);
        if(hint!=null)hint.gameObject.SetActive(false);
        foreach(Transform child in panel)
            if(child!=manager.dialogueText.transform && (speaker==null||child!=speaker.transform) && (portrait==null||child!=portrait.transform))child.gameObject.SetActive(false);
        foreach(var graphic in panel.GetComponents<Graphic>())graphic.enabled=false;
        panel.SetParent(safe,false);
        var background=WorldUi.Rect("Conversation background",panel,Vector2.zero,Vector2.zero,Vector2.zero,Vector2.zero);
        background.anchorMax=Vector2.one;background.offsetMin=background.offsetMax=Vector2.zero;background.SetAsFirstSibling();
        var surface=background.gameObject.AddComponent<UiSurface>();surface.Use(ByteCityTheme.Current.dialoguePanel,200,10);
        if(speaker==null)speaker=WorldUi.Text("Speaker",panel,"Sensei",18,WorldUi.Accent);
        speaker.font=ByteCityTheme.Font;speaker.fontSharedMaterial=speaker.font.material;speaker.fontSize=18;speaker.color=WorldUi.Ink;speaker.alignment=TextAlignmentOptions.MidlineLeft;speaker.enableAutoSizing=false;
        var body=manager.dialogueText;body.font=WorldUi.BodyFont;body.fontSharedMaterial=body.font.material;body.fontSize=22;body.enableAutoSizing=false;body.color=WorldUi.Ink;
        body.alignment=TextAlignmentOptions.TopLeft;body.richText=false;body.lineSpacing=5;body.overflowMode=TextOverflowModes.Overflow;
        if(portrait!=null){portrait.preserveAspect=true;portrait.raycastTarget=false;}
        advance=WorldUi.Button("Continue conversation",panel,"Continue",new Vector2(124,34),()=>{if(!(manager is DialogueManagerTutorial))manager.RequestAdvance();});
        actionLabel=advance.GetComponentInChildren<TMP_Text>();
        hint=WorldUi.Text("Input hint",panel,"SPACE / CLICK",11,new Color(.40f,.36f,.48f));
        gameObject.AddComponent<PanelMotion>();Layout();
    }
    private void Layout()
    {
        if(safe==null)return;
        width=Screen.width;height=Screen.height;WorldUi.SafeArea(safe);Canvas.ForceUpdateCanvases();
        float w=Mathf.Min(920,Mathf.Max(280,safe.rect.width-32));
        bool showPortrait=w>=570;float left=showPortrait?166:24;float textWidth=w-left-24;
        var body=manager.dialogueText;body.fontSize=w<570?15:18;
        float preferred=body.GetPreferredValues(body.text,textWidth,1000).y;
        float h=Mathf.Max(198,preferred+102);
        WorldUi.Place(panel,new Vector2(.5f,0),new Vector2(.5f,0),new Vector2(0,20),new Vector2(w,h));
        WorldUi.Place(speaker.rectTransform,Vector2.up,Vector2.up,new Vector2(left,-22),new Vector2(textWidth,26));
        WorldUi.Place(body.rectTransform,Vector2.up,Vector2.up,new Vector2(left,-54),new Vector2(textWidth,h-106));
        if(portrait!=null){portrait.gameObject.SetActive(showPortrait);WorldUi.Place(portrait.rectTransform,Vector2.up,Vector2.up,new Vector2(24,-24),new Vector2(118,h-48));}
        WorldUi.Place(advance.GetComponent<RectTransform>(),Vector2.right,Vector2.right,new Vector2(-26,20),new Vector2(124,34));
        WorldUi.Place(hint.rectTransform,Vector2.zero,Vector2.zero,new Vector2(left,22),new Vector2(textWidth-140,30));
        previousText=body.text;
    }
    private void LateUpdate()
    {
        if(manager==null || advance==null || actionLabel==null)return;
        if(width!=Screen.width||height!=Screen.height||previousText!=manager.dialogueText.text)Layout();
        actionLabel.text=manager.IsRevealing?"Reveal text":"Continue  >";
    }
    private void OnDestroy() { all.Remove(this); }
}
