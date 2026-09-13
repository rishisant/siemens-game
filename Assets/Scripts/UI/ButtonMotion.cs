using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/** @brief Short, consistent hover and press feedback without changing button actions. */
public class ButtonMotion : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 resting;
    private Button button;
    private bool hovered, pressed, initialized;
    public void CaptureScale() { resting=transform.localScale;initialized=true; }
    private void Awake() { button=GetComponent<Button>();CaptureScale(); }
    private void OnEnable() { hovered=pressed=false; }
    private void OnDisable() { if(initialized)transform.localScale=resting;hovered=pressed=false; }
    public void OnPointerEnter(PointerEventData data) { hovered=true; }
    public void OnPointerExit(PointerEventData data) { hovered=pressed=false; }
    public void OnPointerDown(PointerEventData data) { pressed=true; }
    public void OnPointerUp(PointerEventData data) { pressed=false; }
    private void Update()
    {
        bool active=button != null && button.IsInteractable();
        float size=active?(pressed?.96f:hovered?1.025f:1):1;
        transform.localScale=Vector3.Lerp(transform.localScale,resting*size,1-Mathf.Exp(-24*Time.unscaledDeltaTime));
    }
}
