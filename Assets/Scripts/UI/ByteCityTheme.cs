using TMPro;
using UnityEngine;

/** @brief Shared references to Byte City's original pixel artwork and fonts. */
public class ByteCityTheme : ScriptableObject
{
    public TMP_FontAsset pixelFont;
    public Sprite button;
    public Sprite panel, dialoguePanel, cardFrame, closeButton;
    public Sprite duelTable, cardBack, pipeBackground;
    public AudioClip cardMusic;
    private static ByteCityTheme cached;
    public static ByteCityTheme Current { get { if(cached==null)cached=Resources.Load<ByteCityTheme>("ByteCityTheme");return cached; } }
    public static TMP_FontAsset Font { get { return Current!=null&&Current.pixelFont!=null?Current.pixelFont:TMP_Settings.defaultFontAsset; } }
}
