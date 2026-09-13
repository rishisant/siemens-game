using TMPro;
using UnityEngine;
using UnityEngine.UI;

/** @brief A locally remembered display name, with no legacy account registration. */
public class GuestProfileUI : MonoBehaviour
{
    private static GuestProfileUI instance;
    private TMP_InputField nameInput;
    private bool entering;

    public static void Show()
    {
        if (instance != null) return;
        instance = new GameObject("Choose your robot name").AddComponent<GuestProfileUI>();
    }
    private void Start()
    {
        var canvas = gameObject.AddComponent<Canvas>(); canvas.renderMode=RenderMode.ScreenSpaceOverlay; canvas.sortingOrder=90;
        var scaler=gameObject.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1920,1080);scaler.matchWidthOrHeight=0.5f;
        gameObject.AddComponent<GraphicRaycaster>();
        var shade=TownMultiplayerUI.Rect("Shade",transform,Vector2.zero,Vector2.one,Vector2.zero,Vector2.zero);
        shade.gameObject.AddComponent<Image>().color=new Color(0.04f,0.03f,0.09f,0.85f);
        var card=TownMultiplayerUI.Rect("Guest profile",shade,new Vector2(0.5f,0.5f),new Vector2(0.5f,0.5f),new Vector2(-430,-260),new Vector2(430,260));
        card.gameObject.AddComponent<Image>().color=new Color(0.86f,0.85f,0.93f);
        var outline=card.gameObject.AddComponent<Outline>();outline.effectColor=new Color(0.34f,0.28f,0.45f);outline.effectDistance=new Vector2(8,-8);
        TownMultiplayerUI.Text(card,"WELCOME TO BYTE CITY",new Vector2(44,412),new Vector2(720,64),32);
        TownMultiplayerUI.Text(card,"Choose a name your friends will see.",new Vector2(44,332),new Vector2(772,54),22);
        nameInput=TownMultiplayerUI.Input(card,PlayerPrefs.GetString("ByteCity.DisplayName",""),"Your robot's name",new Vector2(44,246),new Vector2(772,68),20);
        TownMultiplayerUI.Text(card,"Guest play - no account needed.\nProgress lasts for this session.",new Vector2(44,138),new Vector2(772,88),20);
        TownMultiplayerUI.MakeButton(card,"Back",new Vector2(44,44),new Vector2(220,68),()=>Destroy(gameObject));
        TownMultiplayerUI.MakeButton(card,"Enter Byte City",new Vector2(288,44),new Vector2(528,68),Enter);
        nameInput.onSubmit.AddListener(unused=>Enter());
        nameInput.ActivateInputField();
    }
    private void Enter()
    {
        if(entering)return;entering=true;
        string displayName=nameInput.text.Replace("<","").Replace(">","").Trim();
        if(string.IsNullOrWhiteSpace(displayName))displayName="Explorer";
        PlayerPrefs.SetString("ByteCity.DisplayName",displayName);PlayerPrefs.Save();
        if(TownMultiplayer.Instance!=null)TownMultiplayer.Instance.DisplayName=displayName;
        if(LocalPlaytest.IsActive&&PlayerData.Instance!=null)PlayerData.Instance.username=displayName;
        Destroy(gameObject);LocalPlaytest.StartGuest();
    }
    private void Update() { if(UnityEngine.Input.GetKeyDown(KeyCode.Escape))Destroy(gameObject); }
}
