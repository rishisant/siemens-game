using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/** @brief A pixel-styled room panel shared by desktop and touch players. */
public class TownMultiplayerUI : MonoBehaviour
{
    private TownMultiplayer network;
    private GameObject panel, launcher, connectionFields, roomFields;
    private TMP_InputField nameInput, roomInput, serverInput, chatInput;
    private TMP_Text status, chat, launcherText;
    private ScrollRect chatScroll;
    private string lastChat;
    private bool restoreMovement;
    private Character_Movement stoppedPlayer;
    private static readonly Color Ink=new Color(0.12f,0.105f,0.18f);
    private static readonly Color Paper=new Color(0.86f,0.85f,0.93f);
    private void Start()
    {
        network=GetComponent<TownMultiplayer>();
        var canvas=new GameObject("Social UI",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));canvas.transform.SetParent(transform,false);
        canvas.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;canvas.GetComponent<Canvas>().sortingOrder=80;
        var scaler=canvas.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1920,1080);scaler.matchWidthOrHeight=0.5f;
        var launch=WorldUi.Button("Chat",canvas.transform,"Chat [C]",new Vector2(154,63),Toggle);
        WorldUi.Place(launch.GetComponent<RectTransform>(),Vector2.zero,Vector2.zero,new Vector2(27,27),new Vector2(154,63));
        launch.GetComponentInChildren<TMP_Text>().fontSize=22.5f;
        var launchRect=launch.GetComponent<RectTransform>();launchRect.anchorMin=launchRect.anchorMax=Vector2.zero;launcher=launch.gameObject;launcherText=launch.GetComponentInChildren<TMP_Text>();
        var shade=Rect("Room panel",canvas.transform,Vector2.zero,Vector2.one,Vector2.zero,Vector2.zero);shade.gameObject.AddComponent<Image>().color=new Color(0.04f,0.03f,0.09f,0.85f);panel=shade.gameObject;
        var card=Rect("Card",shade,new Vector2(0.5f,0.5f),new Vector2(0.5f,0.5f),new Vector2(-500,-435),new Vector2(500,435));card.gameObject.AddComponent<Image>().color=Paper;
        card.gameObject.AddComponent<PanelMotion>();
        var border=card.gameObject.AddComponent<Outline>();border.effectColor=new Color(0.34f,0.28f,0.45f);border.effectDistance=new Vector2(8,-8);
        Text(card,"BYTE CITY TOGETHER",new Vector2(40,760),new Vector2(820,60),32);
        MakeButton(card,"X",new Vector2(890,774),new Vector2(68,56),Toggle);
        status=Text(card,"",new Vector2(40,696),new Vector2(920,62),20);
        connectionFields=Rect("Join controls",card,Vector2.zero,Vector2.zero,new Vector2(0,470),new Vector2(1000,690)).gameObject;
        Text(connectionFields.transform,"YOUR NAME",new Vector2(40,180),new Vector2(430,36),18);
        nameInput=Input(connectionFields.transform,network.DisplayName,"Your robot's name",new Vector2(40,116),new Vector2(430,62),20);
        Text(connectionFields.transform,"ROOM CODE",new Vector2(510,180),new Vector2(450,36),18);
        roomInput=Input(connectionFields.transform,"","Six-character code",new Vector2(510,116),new Vector2(450,62),6);
        MakeButton(connectionFields.transform,"Join town",new Vector2(40,24),new Vector2(430,70),()=>{network.DisplayName=nameInput.text;network.ServerUrl=serverInput.text;network.ConnectToTown();});
        MakeButton(connectionFields.transform,"Join room",new Vector2(510,24),new Vector2(450,70),()=>Connect(false));
        roomFields=Rect("Room controls",card,Vector2.zero,Vector2.zero,new Vector2(0,530),new Vector2(1000,680)).gameObject;
        MakeButton(roomFields.transform,"Copy room code",new Vector2(40,24),new Vector2(430,70),()=>{GUIUtility.systemCopyBuffer=network.RoomCode;GameToast.Show("Room code copied",network.RoomCode);});
        MakeButton(roomFields.transform,"Leave room",new Vector2(510,24),new Vector2(450,70),()=>network.Disconnect());
        Text(card,"ROOM CHAT",new Vector2(40,450),new Vector2(800,40),22);
        var chatViewport=Rect("Chat history",card,Vector2.zero,Vector2.zero,new Vector2(40,205),new Vector2(960,435));
        chatViewport.gameObject.AddComponent<Image>().color=Paper;
        chatViewport.gameObject.AddComponent<RectMask2D>();
        chat=Text(chatViewport,"Everyone on this host joins the same town.\nWASD / arrows to move. Hold movement or Shift to sprint.",Vector2.zero,new Vector2(920,230),20);
        chat.rectTransform.anchorMin=new Vector2(0,1);chat.rectTransform.anchorMax=Vector2.one;chat.rectTransform.pivot=new Vector2(0.5f,1);chat.rectTransform.anchoredPosition=Vector2.zero;chat.rectTransform.sizeDelta=Vector2.zero;
        chat.gameObject.AddComponent<ContentSizeFitter>().verticalFit=ContentSizeFitter.FitMode.PreferredSize;
        chatScroll=chatViewport.gameObject.AddComponent<ScrollRect>();chatScroll.viewport=chatViewport;chatScroll.content=chat.rectTransform;chatScroll.horizontal=false;chatScroll.movementType=ScrollRect.MovementType.Clamped;
        chat.alignment=TextAlignmentOptions.TopLeft;chat.richText=false;
        chatInput=Input(card,"","Say hello...",new Vector2(40,127),new Vector2(690,62),140);
        MakeButton(card,"Send",new Vector2(752,127),new Vector2(208,62),SendChat);
        serverInput=Input(card,network.ServerUrl,"Server address",new Vector2(40,35),new Vector2(920,54),200);serverInput.gameObject.SetActive(false);
        MakeButton(card,"Connection settings",new Vector2(40,35),new Vector2(500,54),()=>{serverInput.gameObject.SetActive(true);UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(serverInput.gameObject);});
        MakeButton(card,"Sensei lesson",new Vector2(580,35),new Vector2(380,54),()=>{Toggle();LocalPlaytest.ReplayTutorial();});
        serverInput.transform.SetAsLastSibling();panel.SetActive(false);
    }
    private void Connect(bool create)
    {
        network.DisplayName=nameInput.text;network.RoomCode=roomInput.text;network.ServerUrl=serverInput.text;network.Connect(create);
    }
    private void SendChat() { network.SendChat(chatInput.text);chatInput.text=""; }
    public void Toggle()
    {
        bool show=!panel.activeSelf;panel.SetActive(show);
        if(show)
        {
            if(!network.Connected&&!network.Connecting){nameInput.text=network.DisplayName;serverInput.text=network.ServerUrl;}
            stoppedPlayer=FindObjectOfType<Character_Movement>();restoreMovement=stoppedPlayer!=null&&stoppedPlayer.CanMove;
            if(restoreMovement)stoppedPlayer.StopPlayer();
        }
        else if(restoreMovement&&stoppedPlayer!=null)stoppedPlayer.UnstopPlayer();
    }
    private void Update()
    {
        if(network==null||launcher==null)return;
        string scene=SceneManager.GetActiveScene().name;
        var player = FindObjectOfType<Character_Movement>();
        bool available = player != null && player.CanMove && (scene=="Town_Square"||scene=="Laboratory_Main"||scene=="Casino_Main");
        launcher.SetActive(available && !panel.activeSelf);
        var map=FindObjectOfType<TownMinimap>();
        var launchRect=launcher.GetComponent<RectTransform>();
        float scale=launcher.GetComponentInParent<Canvas>().scaleFactor;
        launchRect.anchoredPosition=new Vector2(map!=null && !MobileControls.Enabled?map.RightEdgeOnScreen/Mathf.Max(.01f,scale)+18:MobileControls.Enabled?440:27,27);
        launcherText.text=network.Connected?"Chat [C]":"Offline [C]";
        if (available && !panel.activeSelf && UnityEngine.Input.GetKeyDown(KeyCode.C)) Toggle();
        status.text=network.Connected?"ROOM "+network.RoomCode+"  -  "+network.PlayerCount+" / 8 PLAYERS\n"+network.Status:network.Status;
        connectionFields.SetActive(!network.Connected);roomFields.SetActive(network.Connected);
        chatInput.interactable=network.Connected;
        string messages=network.Chat.Count>0?string.Join("\n",network.Chat):"Everyone on this host joins the same town.\nWASD / arrows to move. Hold movement or Shift to sprint.";
        if(messages!=lastChat) { lastChat=messages;chat.text=messages;Canvas.ForceUpdateCanvases();chatScroll.verticalNormalizedPosition=0; }
        if(panel.activeSelf&&UnityEngine.Input.GetKeyDown(KeyCode.Escape))Toggle();
        if(panel.activeSelf&&chatInput.isFocused&&UnityEngine.Input.GetKeyDown(KeyCode.Return))SendChat();
    }
    internal static RectTransform Rect(string name,Transform parent,Vector2 min,Vector2 max,Vector2 lower,Vector2 upper)
    {
        var rect=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();rect.SetParent(parent,false);rect.anchorMin=min;rect.anchorMax=max;rect.offsetMin=lower;rect.offsetMax=upper;return rect;
    }
    internal static TMP_Text Text(Transform parent,string value,Vector2 pos,Vector2 size,float fontSize)
    {
        var rect=Rect("Text",parent,Vector2.zero,Vector2.zero,pos,pos+size);var text=rect.gameObject.AddComponent<TextMeshProUGUI>();text.font=ByteCityTheme.Font;text.text=value;text.fontSize=fontSize;text.color=Ink;text.raycastTarget=false;text.alignment=TextAlignmentOptions.MidlineLeft;return text;
    }
    internal static Button MakeButton(Transform parent,string title,Vector2 pos,Vector2 size,UnityEngine.Events.UnityAction callback)
    {
        var rect=Rect(title,parent,Vector2.zero,Vector2.zero,pos,pos+size);var image=rect.gameObject.AddComponent<Image>();image.color=new Color(0.70f,0.69f,0.81f);
        if(ByteCityTheme.Current!=null&&ByteCityTheme.Current.button!=null){image.sprite=ByteCityTheme.Current.button;image.color=Color.white;}
        var button=rect.gameObject.AddComponent<Button>();button.targetGraphic=image;button.onClick.AddListener(callback);
        button.navigation=new Navigation {mode=Navigation.Mode.None};
        button.gameObject.AddComponent<ButtonMotion>();
        var label=Text(rect,title,new Vector2(18,6),size-new Vector2(36,12),22);label.alignment=TextAlignmentOptions.Center;label.enableAutoSizing=true;label.fontSizeMin=14;label.fontSizeMax=22;
        return button;
    }
    internal static TMP_InputField Input(Transform parent,string value,string placeholder,Vector2 pos,Vector2 size,int limit)
    {
        var rect=Rect("Input",parent,Vector2.zero,Vector2.zero,pos,pos+size);rect.gameObject.AddComponent<Image>().color=Color.white;rect.gameObject.AddComponent<RectMask2D>();
        var text=Text(rect,value,new Vector2(12,5),size-new Vector2(24,10),20);text.richText=false;
        var hint=Text(rect,placeholder,new Vector2(12,5),size-new Vector2(24,10),18);hint.color=new Color(0.42f,0.4f,0.5f);
        var input=rect.gameObject.AddComponent<TMP_InputField>();input.textViewport=rect;input.textComponent=(TextMeshProUGUI)text;input.placeholder=hint;input.characterLimit=limit;input.text=value;return input;
    }
}
