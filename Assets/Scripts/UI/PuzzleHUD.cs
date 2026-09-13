using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/** @brief Shared, responsive presentation for the two laboratory puzzles. */
public class PuzzleHUD : MonoBehaviour
{
    public static readonly Color Accent = new Color(0.76f, 0.71f, 0.91f);
    private static readonly Color Ink = new Color(0.035f, 0.07f, 0.12f, 0.97f);
    private RectTransform safeRoot;
    private TMP_Text progress, clock, feedback;
    private GameObject modal;
    private Rect lastSafe;
    private int lastWidth, lastHeight;
    private float boardHalfWidth, boardHalfHeight;
    private string lastTime;
    private Image flowMeter;
    private ArcadeFeedback effects;

    public static string FormatTime(TimeSpan time)
    {
        return string.Format("{0:00}:{1:00}.{2:00}", (int)time.TotalMinutes, time.Seconds, time.Milliseconds / 10);
    }

    public void Initialize(string title, string instructions, float halfWidth, float halfHeight)
    {
        boardHalfWidth = halfWidth; boardHalfHeight = halfHeight;
        foreach (var image in FindObjectsOfType<Image>())
        {
            if (image.name == "BackgroundImage" || image.name == "Background")
            {
                image.raycastTarget = false;
                image.color = Color.white;
                if(title == "PECULIAR PIPES" && ByteCityTheme.Current != null) image.sprite = ByteCityTheme.Current.pipeBackground;
            }
        }
        var canvasObject = new GameObject("Puzzle HUD", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);
        var canvas = canvasObject.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 30;
        var scaler = canvasObject.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920,1080); scaler.matchWidthOrHeight = 0.5f;
        safeRoot = Rect("Safe area", canvasObject.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        var header = Panel("Header", safeRoot, new Vector2(0,1), Vector2.one, new Vector2(28,-128), new Vector2(-28,-24), Ink);
        Label(header, title, new Vector2(24,48), new Vector2(650,52), 34, Accent);
        progress = Label(header, "", new Vector2(24,8), new Vector2(800,40), 22, Color.white);
        clock = Label(header, "00:00.00", new Vector2(-280,20), new Vector2(256,62), 36, Color.white, true);
        feedback = Label(safeRoot, instructions, new Vector2(-710,114), new Vector2(1420,56), 24, Color.white);
        feedback.rectTransform.anchorMin = feedback.rectTransform.anchorMax = new Vector2(0.5f,0);
        feedback.alignment = TextAlignmentOptions.Center;
        var back = Button(safeRoot, "Back to lab", new Vector2(-238,32), new Vector2(210,62), ReturnToLab);
        back.GetComponent<RectTransform>().anchorMin = back.GetComponent<RectTransform>().anchorMax = Vector2.right;
        if(title == "PECULIAR PIPES")
        {
            var track=Panel("Coolant gauge",header,Vector2.right,Vector2.right,new Vector2(-280,8),new Vector2(-24,14),new Color(.17f,.22f,.28f));
            var fill=Panel("Flow pressure",track,Vector2.zero,Vector2.one,Vector2.zero,Vector2.zero,new Color(.4f,1,.84f));
            flowMeter=fill.GetComponent<Image>();fill.anchorMax=new Vector2(0,1);flowMeter.raycastTarget=false;track.GetComponent<Image>().raycastTarget=false;
            effects=gameObject.AddComponent<ArcadeFeedback>();
        }
        UpdateLayout();
    }

    public void SetFlow(float fraction)
    { if(flowMeter!=null)flowMeter.rectTransform.anchorMax=new Vector2(Mathf.Clamp01(fraction),1); }
    public void Celebrate()
    { if(effects!=null)effects.Burst(safeRoot,Vector2.zero,new Color(.4f,1,.84f),36); }
    public void SetProgress(string value) { progress.text = value; }
    public void SetTime(TimeSpan value)
    {
        string formatted = FormatTime(value);
        if (formatted != lastTime) { clock.text = formatted; lastTime = formatted; }
    }
    public void SetFeedback(string value, bool error = false)
    {
        feedback.text = value; feedback.color = error ? new Color(1,0.67f,0.54f) : Color.white;
    }
    public void SetPrimaryButton(UnityEngine.UI.Button button, string label)
    {
        button.transform.SetParent(safeRoot, false);
        var rect = button.GetComponent<RectTransform>(); rect.anchorMin = rect.anchorMax = new Vector2(0.5f,0);
        rect.pivot = new Vector2(0.5f,0); rect.anchoredPosition = new Vector2(0,32); rect.sizeDelta = new Vector2(300,62); rect.localScale = Vector3.one;
        var image = button.GetComponent<Image>(); if (image != null) { image.sprite = ByteCityTheme.Current != null ? ByteCityTheme.Current.button : null; image.color = image.sprite != null ? Color.white : Accent; }
        foreach (var text in button.GetComponentsInChildren<TMP_Text>())
        {
            text.font = ByteCityTheme.Font;
            text.text = label; text.color = Ink; text.fontSize = 26; text.enableAutoSizing = true; text.fontSizeMin = 18; text.fontSizeMax = 26;
            text.rectTransform.anchorMin = Vector2.zero; text.rectTransform.anchorMax = Vector2.one; text.rectTransform.offsetMin = new Vector2(12,4); text.rectTransform.offsetMax = new Vector2(-12,-4);
        }
        button.navigation = new Navigation { mode = Navigation.Mode.None };
    }

    public void ShowResult(string title, string details, string primary, Action onPrimary, Action onExit)
    {
        if (modal != null) Destroy(modal);
        var shade = Panel("Result overlay", safeRoot, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(0.01f,0.025f,0.05f,0.88f));
        modal = shade.gameObject;
        var card = Panel("Result card", shade, new Vector2(0.5f,0.5f), new Vector2(0.5f,0.5f), new Vector2(-400,-225), new Vector2(400,225), Ink);
        Label(card, title, new Vector2(40,322), new Vector2(720,70), 40, Accent).alignment = TextAlignmentOptions.Center;
        Label(card, details, new Vector2(40,142), new Vector2(720,175), 28, Color.white).alignment = TextAlignmentOptions.Center;
        Button(card, primary, new Vector2(48,48), new Vector2(336,68), () => { Destroy(modal); onPrimary(); });
        Button(card, "Back to lab", new Vector2(416,48), new Vector2(336,68), onExit);
    }

    public static void ReturnToLab()
    {
        SceneManager.LoadScene("Laboratory_Main");
    }
    private void Update() { if (Screen.safeArea != lastSafe || Screen.width != lastWidth || Screen.height != lastHeight) UpdateLayout(); }
    private void UpdateLayout()
    {
        if (safeRoot == null || Screen.width == 0 || Screen.height == 0) return;
        lastSafe = Screen.safeArea; lastWidth = Screen.width; lastHeight = Screen.height;
        safeRoot.anchorMin = new Vector2(lastSafe.xMin / Screen.width,lastSafe.yMin / Screen.height);
        safeRoot.anchorMax = new Vector2(lastSafe.xMax / Screen.width,lastSafe.yMax / Screen.height);
        var camera = Camera.main;
        if (camera != null && camera.orthographic)
            camera.orthographicSize = Mathf.Max(boardHalfHeight / 0.66f, boardHalfWidth / Mathf.Max(0.1f,camera.aspect) / 0.88f);
    }
    private static RectTransform Rect(string name, Transform parent, Vector2 min, Vector2 max, Vector2 lower, Vector2 upper)
    {
        var rect = new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>(); rect.SetParent(parent,false);
        rect.anchorMin=min; rect.anchorMax=max; rect.offsetMin=lower; rect.offsetMax=upper; return rect;
    }
    private static RectTransform Panel(string name, Transform parent, Vector2 min, Vector2 max, Vector2 lower, Vector2 upper, Color color)
    {
        var rect=Rect(name,parent,min,max,lower,upper); rect.gameObject.AddComponent<Image>().color=color; return rect;
    }
    private static TMP_Text Label(Transform parent,string text,Vector2 pos,Vector2 size,float fontSize,Color color,bool right=false)
    {
        var rect=Rect("Label",parent,right ? Vector2.right : Vector2.zero,right ? Vector2.right : Vector2.zero,pos,pos+size);
        var label=rect.gameObject.AddComponent<TextMeshProUGUI>(); label.font=ByteCityTheme.Font; label.text=text; label.fontSize=fontSize; label.color=color;
        label.raycastTarget=false; label.enableWordWrapping=true; label.alignment=TextAlignmentOptions.MidlineLeft;
        return label;
    }
    private static UnityEngine.UI.Button Button(Transform parent,string title,Vector2 pos,Vector2 size,Action action)
    {
        var rect=Panel(title,parent,Vector2.zero,Vector2.zero,pos,pos+size,Accent);
        if(ByteCityTheme.Current != null && ByteCityTheme.Current.button != null) { rect.GetComponent<Image>().sprite=ByteCityTheme.Current.button; rect.GetComponent<Image>().color=Color.white; }
        var button=rect.gameObject.AddComponent<UnityEngine.UI.Button>(); button.targetGraphic=rect.GetComponent<Image>();
        button.navigation=new Navigation { mode=Navigation.Mode.None };
        button.onClick.AddListener(()=>action());
        var text=Label(rect,title,new Vector2(12,4),size-new Vector2(24,8),26,Ink); text.alignment=TextAlignmentOptions.Center;text.enableAutoSizing=true;text.fontSizeMin=14;text.fontSizeMax=26;text.enableWordWrapping=false;
        return button;
    }
}
