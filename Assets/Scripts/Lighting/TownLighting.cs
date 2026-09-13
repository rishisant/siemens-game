using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

/** @brief Balanced ambient visibility, warm pools of light and gentle fire/equipment motion. */
public class TownLighting : MonoBehaviour
{
    private struct AnimatedLight { public Light2D light; public float intensity, amplitude, speed, phase; }
    private readonly List<AnimatedLight> animated=new List<AnimatedLight>();
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register() { SceneManager.sceneLoaded-=OnScene;SceneManager.sceneLoaded+=OnScene; }
    private static void OnScene(Scene scene,LoadSceneMode mode)
    {
        if(scene.name=="Town_Square"||scene.name=="Tutorial"||scene.name=="Laboratory_Main"||scene.name=="Casino_Main")
            new GameObject("Town atmosphere").AddComponent<TownLighting>().Apply(scene.name);
    }
    private void Apply(string scene)
    {
        foreach(var light in FindObjectsOfType<Light2D>(true))
        {
            string name=light.name.ToLowerInvariant();
            if(light.lightType==Light2D.LightType.Global)
            {
                light.intensity=scene=="Laboratory_Main"?.42f:scene=="Casino_Main"?.55f:.62f;
                light.color=scene=="Laboratory_Main"?new Color(.82f,.92f,1):new Color(.84f,.88f,1);
                continue;
            }
            if(name.Contains("fire"))
            {
                light.color=new Color(1,.48f,.18f);light.intensity=1.5f;
                if(light.lightType==Light2D.LightType.Point) {light.pointLightInnerRadius=.18f;light.pointLightOuterRadius=2.4f;}
                light.falloffIntensity=.65f;Animate(light,.075f,3.4f);
            }
            else if(name.Contains("street")||name.Contains("falloff light"))
            {
                light.color=new Color(1,.86f,.61f);light.intensity=name.Contains("falloff")?.70f:1.05f;
                if(light.lightType==Light2D.LightType.Point) { light.pointLightOuterRadius=Mathf.Max(light.pointLightOuterRadius,.85f);light.pointLightInnerRadius=.08f; }
                light.falloffIntensity=.65f;
            }
            else if(name.Contains("computer")||name.Contains("generator")||name.Contains("tube"))
            {
                light.color=new Color(.46f,.85f,1);light.intensity=Mathf.Min(light.intensity,1.25f);Animate(light,.025f,.7f);
            }
            else if(name.Contains("hall")) {light.color=new Color(.85f,.93f,1);light.intensity=Mathf.Min(light.intensity,1.15f);}
            else if(scene=="Casino_Main") {light.intensity=Mathf.Min(light.intensity,1.65f);Animate(light,.03f,.65f);}
        }
    }
    private void Animate(Light2D light,float amplitude,float speed)
    {
        animated.Add(new AnimatedLight{light=light,intensity=light.intensity,amplitude=amplitude,speed=speed,phase=Mathf.Abs(light.transform.position.x*.71f+light.transform.position.y*.43f)});
    }
    private void Update()
    {
        foreach(var item in animated) if(item.light!=null)
        {
            float wave=(Mathf.PerlinNoise(item.phase,Time.time*item.speed)-.5f)*2;
            item.light.intensity=item.intensity*(1+wave*item.amplitude);
        }
    }
}
