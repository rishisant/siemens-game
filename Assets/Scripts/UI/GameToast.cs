using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/** @brief Nonblocking, queued notifications with a short slide and fade. */
public class GameToast : MonoBehaviour
{
    private static GameToast instance;
    private readonly Queue<string[]> queue = new Queue<string[]>();
    private CanvasGroup group;
    private RectTransform card;
    private TMP_Text title, detail;
    private float age = 10;
    public static void Show(string heading, string message = "")
    {
        if (instance == null)
        {
            var go = new GameObject("Notifications");
            DontDestroyOnLoad(go);
            instance = go.AddComponent<GameToast>();
        }
        if (instance.queue.Count < 8) instance.queue.Enqueue(new[] {heading, message});
    }
    private void Awake()
    {
        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 200;
        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920,1080); scaler.matchWidthOrHeight = 0.5f;
        card = TownMultiplayerUI.Rect("Toast",transform,Vector2.one,Vector2.one,new Vector2(-550,-240),new Vector2(-24,-116));
        card.gameObject.AddComponent<UiSurface>();
        group = card.gameObject.AddComponent<CanvasGroup>();group.blocksRaycasts = false;group.interactable = false;group.alpha = 0;
        title = TownMultiplayerUI.Text(card,"",new Vector2(24,62),new Vector2(478,48),23);
        detail = TownMultiplayerUI.Text(card,"",new Vector2(24,12),new Vector2(478,48),18);
        title.color = Color.white;detail.color = new Color(0.78f,0.77f,0.87f);
        title.richText = detail.richText = false;
        title.enableAutoSizing = detail.enableAutoSizing = true;
        title.fontSizeMin = 16;title.fontSizeMax = 23;detail.fontSizeMin = 14;detail.fontSizeMax = 18;
    }
    private void Update()
    {
        if (age >= 4.2f && queue.Count > 0)
        {
            var next = queue.Dequeue();title.text = next[0];detail.text = next[1];age = 0;
        }
        age += Time.unscaledDeltaTime;
        group.alpha = Mathf.Min(Mathf.Clamp01(age / 0.2f),Mathf.Clamp01((4.2f-age) / 0.5f));
        card.anchoredPosition = new Vector2(-287 + 24*(1-group.alpha),-178);
    }
    private void OnDestroy() { if(instance == this) instance = null; }
}
