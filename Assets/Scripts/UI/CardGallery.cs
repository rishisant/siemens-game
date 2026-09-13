using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/** @brief Original card-book artwork with contained, responsive content and independent navigation. */
public class CardGallery : MonoBehaviour
{
    private List<int> ids;
    private int index;
    private string title;
    private Action closed;
    private RectTransform safe,panel,frame,banner,art;
    private TMP_Text heading,nameText,rarity,stats,description,counter;
    private Image cardImage;
    private Button previous,next,done,close;
    private int width,height;
    public static void Show(string title,IEnumerable<int> cards,Action closed=null)
    {
        var canvas=WorldUi.Canvas("Card gallery",95);canvas.gameObject.AddComponent<WorldUiFocus>();
        var gallery=canvas.gameObject.AddComponent<CardGallery>();gallery.title=title;
        gallery.ids=cards.Where(id=>PlayerData.Instance.cards.ContainsKey(id)).Distinct().ToList();gallery.closed=closed;
    }
    private void Start()
    {
        var dim=WorldUi.Rect("Dim",transform,Vector2.zero,Vector2.zero,Vector2.zero,Vector2.zero);
        dim.anchorMax=Vector2.one;dim.offsetMin=dim.offsetMax=Vector2.zero;
        dim.gameObject.AddComponent<Image>().color=new Color(.08f,.07f,.13f,.65f);
        safe=WorldUi.Rect("Safe area",transform,Vector2.zero,Vector2.zero,Vector2.zero,Vector2.zero);WorldUi.SafeArea(safe);
        panel=WorldUi.Rect("Card details",safe,new Vector2(.5f,.5f),new Vector2(.5f,.5f),Vector2.zero,new Vector2(800,570));
        frame=WorldUi.Rect("Original card book",panel,Vector2.zero,Vector2.zero,Vector2.zero,Vector2.zero);
        frame.gameObject.AddComponent<UiSurface>().Use(ByteCityTheme.Current.cardFrame);
        banner=WorldUi.Rect("Original title frame",panel,new Vector2(.5f,1),new Vector2(.5f,1),Vector2.zero,new Vector2(380,46));
        banner.gameObject.AddComponent<UiSurface>().Use(ByteCityTheme.Current.dialoguePanel,200,16);
        heading=WorldUi.Text("Title",banner,title,18,WorldUi.Ink);heading.alignment=TextAlignmentOptions.Center;
        heading.rectTransform.anchorMin=Vector2.zero;heading.rectTransform.anchorMax=Vector2.one;heading.rectTransform.offsetMin=new Vector2(18,8);heading.rectTransform.offsetMax=new Vector2(-18,-8);
        heading.enableAutoSizing=true;heading.fontSizeMin=12;heading.fontSizeMax=18;
        close=WorldUi.Button("Close cards",panel,"",new Vector2(40,34),Close);
        close.GetComponent<UiSurface>().Use(ByteCityTheme.Current.closeButton);
        art=WorldUi.Rect("Card art",panel,Vector2.up,Vector2.up,Vector2.zero,Vector2.zero);
        cardImage=art.gameObject.AddComponent<Image>();cardImage.preserveAspect=true;cardImage.raycastTarget=false;
        nameText=WorldUi.Text("Card name",panel,"",20,WorldUi.Paper);
        rarity=WorldUi.Text("Rarity",panel,"",16,WorldUi.Accent);
        stats=WorldUi.Text("Stats",panel,"",18,WorldUi.Paper);
        description=WorldUi.Text("Description",panel,"",14,WorldUi.Muted);description.alignment=TextAlignmentOptions.TopLeft;
        previous=WorldUi.Button("Previous card",panel,"<",new Vector2(42,34),()=>Change(-1));
        next=WorldUi.Button("Next card",panel,">",new Vector2(42,34),()=>Change(1));
        counter=WorldUi.Text("Card count",panel,"",14,WorldUi.Paper);counter.alignment=TextAlignmentOptions.Center;
        done=WorldUi.Button("Done",panel,"Done",new Vector2(124,34),Close);
        panel.gameObject.AddComponent<PanelMotion>();Layout();Render();
    }
    private void Layout()
    {
        width=Screen.width;height=Screen.height;WorldUi.SafeArea(safe);Canvas.ForceUpdateCanvases();
        float w=Mathf.Min(800,safe.rect.width-32),h=Mathf.Min(570,safe.rect.height-32);
        panel.sizeDelta=new Vector2(w,h);frame.sizeDelta=new Vector2(w,h-54);
        banner.sizeDelta=new Vector2(Mathf.Min(380,w*.65f),46);
        WorldUi.Place(close.GetComponent<RectTransform>(),Vector2.one,Vector2.one,new Vector2(-w*.08f,-60),new Vector2(40,34));
        WorldUi.Place(art,Vector2.up,Vector2.up,new Vector2(w*.12f,-110),new Vector2(w*.31f,h-208));
        float left=w*.55f,contentWidth=w*.31f;
        WorldUi.Place(nameText.rectTransform,Vector2.up,Vector2.up,new Vector2(left,-112),new Vector2(contentWidth,65));
        nameText.enableAutoSizing=true;nameText.fontSizeMin=14;nameText.fontSizeMax=20;
        WorldUi.Place(rarity.rectTransform,Vector2.up,Vector2.up,new Vector2(left,-190),new Vector2(contentWidth,26));
        WorldUi.Place(stats.rectTransform,Vector2.up,Vector2.up,new Vector2(left,-232),new Vector2(contentWidth,62));
        WorldUi.Place(description.rectTransform,Vector2.up,Vector2.up,new Vector2(left,-322),new Vector2(contentWidth,Mathf.Max(64,h-412)));
        WorldUi.Place(previous.GetComponent<RectTransform>(),Vector2.zero,Vector2.zero,new Vector2(w*.15f,44),new Vector2(42,34));
        WorldUi.Place(counter.rectTransform,Vector2.zero,new Vector2(.5f,0),new Vector2(w*.29f,44),new Vector2(80,34));
        WorldUi.Place(next.GetComponent<RectTransform>(),Vector2.zero,Vector2.zero,new Vector2(w*.38f,44),new Vector2(42,34));
        WorldUi.Place(done.GetComponent<RectTransform>(),Vector2.zero,new Vector2(.5f,0),new Vector2(w*.71f,72),new Vector2(124,34));
    }
    private void Change(int amount){index=Mathf.Clamp(index+amount,0,Mathf.Max(0,ids.Count-1));Render();}
    private void Render()
    {
        previous.interactable=index>0;next.interactable=index<ids.Count-1;counter.text=ids.Count==0?"0 cards":(index+1)+" / "+ids.Count;
        cardImage.enabled=ids.Count>0;
        if(ids.Count==0){nameText.text="No new cards";rarity.text="";stats.text="";description.text="Duplicates returned coins. Your collection is ready for a match.";return;}
        var card=PlayerData.Instance.cards[ids[index]];cardImage.sprite=card.image;nameText.text=card.name;
        rarity.text=card.rarity==1?"COMMON":card.rarity==2?"RARE":card.rarity==3?"ULTRA-RARE":"LEGENDARY";
        rarity.color=card.rarity>=3?new Color(.92f,.79f,1):WorldUi.Accent;
        stats.text=card.element+"\nPower  "+card.power;
        string beats=card.element==PlayerData.Card.Element.Heat?"Pressure":card.element==PlayerData.Card.Element.Pressure?"Electrical":"Heat";
        description.text="Beats "+beats+".\n\nSame element? Higher power wins.";
    }
    private void Update()
    {
        if(safe==null)return;
        if(width!=Screen.width||height!=Screen.height)Layout();
        if(Input.GetKeyDown(KeyCode.LeftArrow))Change(-1);
        if(Input.GetKeyDown(KeyCode.RightArrow))Change(1);
        if(Input.GetKeyDown(KeyCode.Escape))Close();
    }
    private void Close(){var callback=closed;closed=null;Destroy(gameObject);if(callback!=null)callback();}
}
