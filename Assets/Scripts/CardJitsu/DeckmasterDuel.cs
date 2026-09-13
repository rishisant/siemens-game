using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/** A staged solo duel using the original table, cards and soundtrack. */
public class DeckmasterDuel : MonoBehaviour
{
    private DeckmasterMatch match;
    private bool revealed;
    private TMP_Text score, result, detail, deckLabel;
    private RectTransform table, handRoot, arena, enemy;
    private Button next, previousPage, followingPage;
    private readonly List<AudioSource> pausedMusic = new List<AudioSource>();
    private AudioSource music;
    private ArcadeFeedback feedback;
    private int page;
    private const int PageSize = 7;
    public bool Resolving { get; private set; }
    public static void Show()
    {
        if (FindObjectOfType<DeckmasterDuel>() != null) return;
        WorldUi.Canvas("Deckmaster match",100).gameObject.AddComponent<DeckmasterDuel>();
    }
    private void Start()
    {
        var player=FindObjectOfType<Character_Movement>();
        if(player!=null) player.StopMoving();
        gameObject.AddComponent<WorldUiFocus>();
        var scaler=GetComponent<CanvasScaler>();scaler.screenMatchMode=CanvasScaler.ScreenMatchMode.Expand;
        var bg=WorldUi.Rect("Backdrop",transform,Vector2.zero,Vector2.zero,Vector2.zero,Vector2.zero);
        bg.anchorMax=Vector2.one;bg.offsetMin=bg.offsetMax=Vector2.zero;
        bg.gameObject.AddComponent<Image>().color=new Color(.055f,.06f,.09f,1);
        table=WorldUi.Rect("Original card table",transform,Vector2.one*.5f,Vector2.one*.5f,Vector2.zero,new Vector2(1240,700));
        table.gameObject.AddComponent<Image>().sprite=ByteCityTheme.Current.duelTable;
        Label(table,"CARD JITSU",new Vector2(0,289),new Vector2(640,36),25);
        var leave=WorldUi.Button("Leave",table,"Leave",new Vector2(105,36),()=>Destroy(gameObject));Place(leave.transform,new Vector2(505,288));
        score=Label(table,"",new Vector2(0,246),new Vector2(1000,35),19);
        Label(table,"HEAT > PRESSURE > ELECTRICAL > HEAT",new Vector2(0,211),new Vector2(1000,25),13);
        arena=WorldUi.Rect("Clash arena",table,Vector2.one*.5f,Vector2.one*.5f,new Vector2(0,56),new Vector2(1050,270));
        Label(arena,"YOU",new Vector2(-210,113),new Vector2(260,25),15);
        Label(arena,"DECKMASTER",new Vector2(210,113),new Vector2(260,25),15);
        Label(arena,"VS",Vector2.zero,new Vector2(90,50),28);
        result=Label(table,"",new Vector2(0,-108),new Vector2(1000,34),20);
        detail=Label(table,"",new Vector2(0,-139),new Vector2(1070,27),12);
        handRoot=WorldUi.Rect("Your collection",table,Vector2.one*.5f,Vector2.one*.5f,new Vector2(0,-230),new Vector2(1030,140));
        deckLabel=Label(table,"",new Vector2(0,-323),new Vector2(850,26),12);
        previousPage=WorldUi.Button("Previous cards",table,"<",new Vector2(40,40),()=>{page--;RenderHand();});Place(previousPage.transform,new Vector2(-553,-225));
        followingPage=WorldUi.Button("More cards",table,">",new Vector2(40,40),()=>{page++;RenderHand();});Place(followingPage.transform,new Vector2(553,-225));
        next=WorldUi.Button("Next round",table,"Next round",new Vector2(190,37),()=>{if(match.Complete)Begin();else{match.Deal();Render();}});Place(next.transform,new Vector2(452,-322));
        feedback=gameObject.AddComponent<ArcadeFeedback>();
        foreach(var source in FindObjectsOfType<AudioSource>())
            if(source.isPlaying && source.loop && source.clip!=null){pausedMusic.Add(source);source.Pause();}
        music=gameObject.AddComponent<AudioSource>();music.clip=ByteCityTheme.Current.cardMusic;music.loop=true;music.volume=.65f;music.Play();
        Begin();
    }
    private static void Place(Transform item,Vector2 position)
    { var r=(RectTransform)item;WorldUi.Place(r,Vector2.one*.5f,Vector2.one*.5f,position,r.sizeDelta); }
    private TMP_Text Label(Transform parent,string value,Vector2 pos,Vector2 size,float font)
    {var label=WorldUi.Text(value,parent,value,font,WorldUi.Paper);WorldUi.Place(label.rectTransform,Vector2.one*.5f,Vector2.one*.5f,pos,size);label.alignment=TextAlignmentOptions.Center;return label;}
    private void Begin()
    {
        var data=PlayerData.Instance;
        var owned=data.unlocked_cards.Where(id=>data.cards.ContainsKey(id)).Distinct().Select(id=>data.cards[id]).ToList();
        if(owned.Count==0){foreach(int id in new[]{0,4,9})data.unlocked_cards.Add(id);owned=new List<PlayerData.Card>{data.cards[0],data.cards[4],data.cards[9]};}
        match=new DeckmasterMatch(owned,new System.Random());page=0;Render();
    }
    private static void Clear(Transform root)
    {foreach(Transform child in root){child.gameObject.SetActive(false);Destroy(child.gameObject);}}
    private void UpdateScore()
    {score.text=PlayerData.Instance.username+"  "+match.PlayerWins+" : "+match.OpponentWins+"  DECKMASTER    /    "+(match.Complete?"FINAL":"ROUND "+(match.Rounds+1));}
    private void Render()
    {
        revealed=false;Resolving=false;result.color=WorldUi.Paper;next.gameObject.SetActive(false);
        // Preserve the arena labels; remove only the previous played cards and effects.
        foreach(Transform child in arena)if(child.name=="Duel card" || child.name=="Pixel spark")Destroy(child.gameObject);
        enemy=CardView(arena,null,new Vector2(210,0),new Vector2(154,185),null);
        UpdateScore();result.text=match.PlayerWins==2 || match.OpponentWins==2?"MATCH POINT. MAKE IT COUNT.":"Your move. Choose any owned card.";
        detail.text="First to 3 wins  /  Same element: higher power wins  /  Victory: +80 coins";
        RenderHand();
    }
    private void RenderHand()
    {
        Clear(handRoot);page=Mathf.Clamp(page,0,(match.Hand.Count-1)/PageSize);
        int count=Mathf.Min(PageSize,match.Hand.Count-page*PageSize);
        for(int i=0;i<count;i++)
        {
            int index=page*PageSize+i;
            var card=CardView(handRoot,match.Hand[index],new Vector2((i-(count-1)*.5f)*140,0),new Vector2(100,120),()=>Play(index));
            card.GetComponent<Button>().interactable=!revealed;
        }
        previousPage.gameObject.SetActive(page>0);followingPage.gameObject.SetActive((page+1)*PageSize<match.Hand.Count);
        deckLabel.text="YOUR DECK  /  "+match.Hand.Count+" CARDS"+(match.Hand.Count>PageSize?"  /  PAGE "+(page+1):"");
    }
    private RectTransform CardView(Transform root,PlayerData.Card? data,Vector2 pos,Vector2 size,UnityEngine.Events.UnityAction action)
    {
        var rect=WorldUi.Rect("Duel card",root,Vector2.one*.5f,Vector2.one*.5f,pos,size);
        var image=rect.gameObject.AddComponent<Image>();image.sprite=!data.HasValue?ByteCityTheme.Current.cardBack:data.Value.image;image.preserveAspect=true;image.raycastTarget=action!=null;
        if(action!=null){var button=rect.gameObject.AddComponent<Button>();button.targetGraphic=image;button.onClick.AddListener(action);button.navigation=new Navigation{mode=Navigation.Mode.None};rect.gameObject.AddComponent<ButtonMotion>();}
        if(data.HasValue){var name=Label(rect,data.Value.name,new Vector2(0,-size.y*.5f-11),new Vector2(size.x+32,23),10);name.enableAutoSizing=true;name.fontSizeMin=7;name.fontSizeMax=10;}
        return rect;
    }
    public void Play(int index)
    {
        if(revealed || match==null || match.Complete || index<0 || index>=match.Hand.Count)return;
        revealed=true;Resolving=true;
        foreach(var button in handRoot.GetComponentsInChildren<Button>())button.interactable=false;
        StartCoroutine(Resolve(index));
    }
    private IEnumerator Resolve(int index)
    {
        var chosen=match.Hand[index];var other=match.Opponent;
        Vector2 from=arena.InverseTransformPoint(handRoot.GetChild(index%PageSize).position);
        var played=CardView(arena,chosen,from,new Vector2(154,185),null);
        result.text="Card committed.";detail.text="Deckmaster is choosing...";feedback.Tone(440,.09f);
        yield return Move(played,new Vector2(-210,0),.3f);
        // Opponent was committed independently; the presentation deliberately waits a full second.
        yield return new WaitForSecondsRealtime(1f);
        result.text="DECKMASTER PLAYS";feedback.Tone(260,.12f);
        yield return Move(enemy,new Vector2(145,0),.22f);
        for(float t=0;t<1;t+=Time.unscaledDeltaTime/.16f){enemy.localScale=new Vector3(1-t,1,1);yield return null;}
        enemy.GetComponent<Image>().sprite=other.image;
        for(float t=0;t<1;t+=Time.unscaledDeltaTime/.16f){enemy.localScale=new Vector3(t,1,1);yield return null;}
        enemy.localScale=Vector3.one;
        Label(enemy,other.name,new Vector2(0,-104),new Vector2(190,23),10);
        yield return Move(played,new Vector2(-145,0),.18f);
        yield return new WaitForSecondsRealtime(.2f);
        int outcome=match.Play(index);
        var tint=outcome>0?ElementColor(chosen.element):outcome<0?ElementColor(other.element):WorldUi.Accent;
        feedback.Burst(arena,Vector2.zero,tint,24);feedback.Tone(outcome>0?740:outcome<0?145:360,.22f);
        for(float t=0;t<.28f;t+=Time.unscaledDeltaTime){table.anchoredPosition=new Vector2(Mathf.Sin(t*95)*5,Mathf.Cos(t*83)*3)*(1-t/.28f);yield return null;}
        table.anchoredPosition=Vector2.zero;
        var winner=outcome>=0?played:enemy;winner.localScale=Vector3.one*1.1f;
        result.color=tint;result.text=outcome>0?"ROUND WON!":outcome<0?"DECKMASTER TAKES THE ROUND":"CLASH! NO POINTS.";
        detail.text=chosen.element+" "+chosen.power+"  vs  "+other.element+" "+other.power;
        score.text=PlayerData.Instance.username+"  "+match.PlayerWins+" : "+match.OpponentWins+"  DECKMASTER";
        if(match.Complete)
        {
            if(match.Won){int reward=match.ClaimReward(PlayerData.Instance);result.text="DECKMASTER DEFEATED!  +80 COINS";detail.text=reward>=0?"New card: "+PlayerData.Instance.cards[reward].name:"The lab has a new card champion.";feedback.Burst(arena,Vector2.zero,tint,45);}
            else result.text=match.PlayerWins==match.OpponentWins?"MATCH DRAWN. RUN IT BACK?":"DECKMASTER WINS. READY FOR A REMATCH?";
        }
        Resolving=false;next.gameObject.SetActive(true);next.GetComponentInChildren<TMP_Text>().text=match.Complete?"Rematch":"Next round";
    }
    private IEnumerator Move(RectTransform card,Vector2 target,float duration)
    {
        Vector2 start=card.anchoredPosition;
        for(float t=0;t<1;t+=Time.unscaledDeltaTime/duration){float eased=1-Mathf.Pow(1-t,3);card.anchoredPosition=Vector2.Lerp(start,target,eased)+Vector2.up*Mathf.Sin(t*Mathf.PI)*25;card.localRotation=Quaternion.Euler(0,0,Mathf.Sin(t*Mathf.PI)*8);yield return null;}
        card.anchoredPosition=target;card.localRotation=Quaternion.identity;
    }
    public static Color ElementColor(PlayerData.Card.Element element)
    {return element==PlayerData.Card.Element.Heat?new Color(1,.49f,.30f):element==PlayerData.Card.Element.Pressure?new Color(.40f,.9f,1):new Color(1,.88f,.30f);}
    private void OnDestroy()
    {foreach(var source in pausedMusic)if(source!=null)source.UnPause();}
}
