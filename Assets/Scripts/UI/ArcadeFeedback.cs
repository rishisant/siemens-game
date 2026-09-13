using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/** Short arcade tones and pixel sparks; no external effects assets. */
public class ArcadeFeedback : MonoBehaviour
{
    private AudioSource audioSource;
    public void Tone(float frequency,float duration)
    {
        if(audioSource==null){audioSource=gameObject.AddComponent<AudioSource>();audioSource.playOnAwake=false;audioSource.volume=.16f;}
        const int rate=22050;var samples=new float[Mathf.CeilToInt(rate*duration)];
        for(int i=0;i<samples.Length;i++){float t=(float)i/rate;float fade=Mathf.Min(t/.008f,1)*(1-(float)i/samples.Length);samples[i]=Mathf.Sin(2*Mathf.PI*frequency*t)*fade;}
        var clip=AudioClip.Create("Arcade tone",samples.Length,1,rate,false);clip.SetData(samples,0);audioSource.PlayOneShot(clip);Destroy(clip,duration+.1f);
    }
    public void Burst(RectTransform parent,Vector2 origin,Color color,int count)
    {StartCoroutine(Sparks(parent,origin,color,count));}
    private IEnumerator Sparks(RectTransform parent,Vector2 origin,Color color,int count)
    {
        var pixels=new RectTransform[count];var velocities=new Vector2[count];
        for(int i=0;i<count;i++){
            pixels[i]=WorldUi.Rect("Pixel spark",parent,Vector2.one*.5f,Vector2.one*.5f,origin,Vector2.one*Random.Range(3,8));
            var image=pixels[i].gameObject.AddComponent<Image>();image.color=color;image.raycastTarget=false;
            velocities[i]=Random.insideUnitCircle*250;
        }
        for(float t=0;t<.6f;t+=Time.unscaledDeltaTime){for(int i=0;i<count;i++)if(pixels[i]!=null){pixels[i].anchoredPosition=origin+velocities[i]*t+Vector2.down*140*t*t;pixels[i].localScale=Vector3.one*(1-t/.6f);}yield return null;}
        foreach(var pixel in pixels)if(pixel!=null)Destroy(pixel.gameObject);
    }
}
