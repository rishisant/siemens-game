using UnityEngine;

/** @brief A quick, restrained panel reveal that uses real time while gameplay is paused. */
public class PanelMotion : MonoBehaviour
{
    private CanvasGroup group;
    private Vector3 resting;
    private float age;
    private void Awake() { resting=transform.localScale;group=GetComponent<CanvasGroup>();if(group==null)group=gameObject.AddComponent<CanvasGroup>(); }
    private void OnEnable() { age=0;group.alpha=0;transform.localScale=resting*.98f; }
    private void Update()
    {
        age+=Time.unscaledDeltaTime;float t=Mathf.Clamp01(age/.16f);float ease=1-Mathf.Pow(1-t,3);
        group.alpha=ease;transform.localScale=resting*Mathf.Lerp(.98f,1,ease);
    }
    private void OnDisable() { transform.localScale=resting;group.alpha=1; }
}
