using TMPro;
using UnityEngine;
using UnityEngine.UI;

/** @brief Shared sizing, typography and safe-area layout for the in-world interface. */
public static class WorldUi
{
    public static TMP_FontAsset BodyFont { get { return ByteCityTheme.Font; } }
    public static readonly Color Ink=new Color(.14f,.13f,.20f);
    public static readonly Color Paper=new Color(.96f,.96f,.99f);
    public static readonly Color Muted=new Color(.78f,.77f,.87f);
    public static readonly Color Accent=new Color(.91f,.88f,1f);
    public static RectTransform Canvas(string name,int order)
    {
        var go=new GameObject(name,typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
        var canvas=go.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=order;
        var scaler=go.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1280,720);scaler.matchWidthOrHeight=.5f;
        return go.GetComponent<RectTransform>();
    }
    public static RectTransform Rect(string name,Transform parent,Vector2 anchor,Vector2 pivot,Vector2 position,Vector2 size)
    {
        var rect=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();rect.SetParent(parent,false);
        Place(rect,anchor,pivot,position,size);return rect;
    }
    public static void Place(RectTransform rect,Vector2 anchor,Vector2 pivot,Vector2 position,Vector2 size)
    {
        rect.anchorMin=rect.anchorMax=anchor;rect.pivot=pivot;rect.anchoredPosition=position;rect.sizeDelta=size;rect.localScale=Vector3.one;
    }
    public static void SafeArea(RectTransform rect)
    {
        Rect safe=Screen.safeArea;rect.anchorMin=new Vector2(safe.xMin/Screen.width,safe.yMin/Screen.height);rect.anchorMax=new Vector2(safe.xMax/Screen.width,safe.yMax/Screen.height);rect.offsetMin=rect.offsetMax=Vector2.zero;
    }
    public static TMP_Text Text(string name,Transform parent,string value,float size,Color color)
    {
        var rect=Rect(name,parent,Vector2.zero,Vector2.zero,Vector2.zero,new Vector2(100,30));
        var text=rect.gameObject.AddComponent<TextMeshProUGUI>();text.font=BodyFont;text.text=value;text.fontSize=size;text.color=color;
        text.richText=false;text.raycastTarget=false;text.alignment=TextAlignmentOptions.MidlineLeft;return text;
    }
    public static Button Button(string name,Transform parent,string value,Vector2 size,UnityEngine.Events.UnityAction action)
    {
        var rect=Rect(name,parent,Vector2.zero,Vector2.zero,Vector2.zero,size);var surface=rect.gameObject.AddComponent<UiSurface>();surface.Use(ByteCityTheme.Current.button,90,15);
        var button=rect.gameObject.AddComponent<Button>();button.targetGraphic=surface;button.onClick.AddListener(action);button.navigation=new Navigation{mode=Navigation.Mode.None};
        var colors=button.colors;colors.highlightedColor=new Color(1.17f,1.17f,1.17f);colors.pressedColor=new Color(.78f,.84f,.94f);colors.fadeDuration=.08f;button.colors=colors;
        var label=Text("Label",rect,value,14,Ink);label.alignment=TextAlignmentOptions.Center;label.enableAutoSizing=true;label.fontSizeMin=10;label.fontSizeMax=14;
        label.rectTransform.anchorMin=Vector2.zero;label.rectTransform.anchorMax=Vector2.one;label.rectTransform.offsetMin=new Vector2(6,2);label.rectTransform.offsetMax=new Vector2(-6,-2);
        rect.gameObject.AddComponent<ButtonMotion>();return button;
    }
}
