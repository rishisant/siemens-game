using TMPro;
using UnityEngine;
using UnityEngine.UI;

/** @brief A compact set of clear actions for playing, collecting and buying cards. */
public class DeckmasterHub : MonoBehaviour
{
    private DeckmasterChoice dealer;
    public static void Show(DeckmasterChoice dealer)
    {
        var canvas=WorldUi.Canvas("Deckmaster hub",90);canvas.gameObject.AddComponent<WorldUiFocus>();canvas.gameObject.AddComponent<DeckmasterHub>().dealer=dealer;
    }
    private void Start()
    {
        var dim=WorldUi.Rect("Dim",transform,Vector2.zero,Vector2.zero,Vector2.zero,Vector2.zero);dim.anchorMax=Vector2.one;dim.offsetMin=dim.offsetMax=Vector2.zero;dim.gameObject.AddComponent<Image>().color=new Color(.015f,.025f,.045f,.78f);
        var card=WorldUi.Rect("Deckmaster",transform,new Vector2(.5f,.5f),new Vector2(.5f,.5f),Vector2.zero,new Vector2(530,400));card.gameObject.AddComponent<UiSurface>();
        var title=WorldUi.Text("Title",card,"DECKMASTER",22,WorldUi.Accent);title.font=ByteCityTheme.Font;WorldUi.Place(title.rectTransform,Vector2.up,Vector2.up,new Vector2(28,-24),new Vector2(420,34));
        var subtitle=WorldUi.Text("Balance",card,""+PlayerData.Instance.coins+" coins",14,WorldUi.Muted);WorldUi.Place(subtitle.rectTransform,Vector2.up,Vector2.up,new Vector2(28,-65),new Vector2(470,32));
        Add(card,"Challenge Deckmaster",238,()=>DeckmasterDuel.Show());
        Add(card,"Browse your collection",178,()=>dealer.viewCardOwned());
        var common=Add(card,"Common pack  ·  125 coins",118,()=>dealer.buyNormal());common.interactable=PlayerData.Instance.coins>=125;
        var rare=Add(card,"Rare pack  ·  500 coins",58,()=>dealer.buyRare());rare.interactable=PlayerData.Instance.coins>=500;
        var close=WorldUi.Button("Close",card,"X",new Vector2(34,32),()=>Destroy(gameObject));close.GetComponent<UiSurface>().Use(ByteCityTheme.Current.closeButton);close.GetComponentInChildren<TMP_Text>().text="";WorldUi.Place(close.GetComponent<RectTransform>(),Vector2.one,Vector2.one,new Vector2(-20,-20),new Vector2(34,32));
        card.gameObject.AddComponent<PanelMotion>();
    }
    private Button Add(Transform parent,string label,float y,UnityEngine.Events.UnityAction callback)
    {
        var button=WorldUi.Button(label,parent,label,new Vector2(474,48),()=>{Destroy(gameObject);callback();});WorldUi.Place(button.GetComponent<RectTransform>(),Vector2.zero,Vector2.zero,new Vector2(28,y),new Vector2(474,48));return button;
    }
    private void Update(){if(Input.GetKeyDown(KeyCode.Escape))Destroy(gameObject);}
}
