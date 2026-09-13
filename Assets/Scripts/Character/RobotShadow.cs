using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/** @brief A lightweight stepped contact shadow, anchored to a robot's feet. */
public class RobotShadow : MonoBehaviour
{
    private static Sprite footprint;
    private static readonly HashSet<string> NpcNames = new HashSet<string> { "Random_Robot", "ShopOwner", "Deckmaster", "CasinoOwner" };
    private SpriteRenderer body;
    private readonly List<SpriteRenderer> shadows = new List<SpriteRenderer>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register()
    {
        SceneManager.sceneLoaded -= OnScene;
        SceneManager.sceneLoaded += OnScene;
    }
    private static void OnScene(Scene scene, LoadSceneMode mode)
    {
        foreach (var root in scene.GetRootGameObjects())
            foreach (var sprite in root.GetComponentsInChildren<SpriteRenderer>())
                if (sprite.GetComponent<Character_Movement>() != null || NpcNames.Contains(sprite.name)) Attach(sprite);
    }
    public static void Attach(SpriteRenderer source)
    {
        if (source == null || source.GetComponent<RobotShadow>() != null) return;
        var component = source.gameObject.AddComponent<RobotShadow>();
        component.body = source;
        component.Create();
    }
    private void Create()
    {
        if (footprint == null)
        {
            // Geometry over a plain white texture keeps the footprint crisp at any resolution.
            var vertices = new List<Vector2>(); var triangles = new List<ushort>();
            float[] widths = {0.48f, 0.74f, 0.91f, 1f, 0.91f, 0.74f, 0.48f};
            for (int row = 0; row < widths.Length; row++)
            {
                float x = widths[row] * 0.5f, y = (row - 3.5f) * 0.045f;
                ushort index = (ushort)vertices.Count;
                vertices.Add(new Vector2(-x,y)); vertices.Add(new Vector2(x,y));
                vertices.Add(new Vector2(x,y+0.045f)); vertices.Add(new Vector2(-x,y+0.045f));
                triangles.AddRange(new[] {index,(ushort)(index+2),(ushort)(index+1),index,(ushort)(index+3),(ushort)(index+2)});
            }
            footprint = Sprite.Create(Texture2D.whiteTexture,new Rect(0,0,1,1),new Vector2(0.5f,0.5f),1);
            footprint.name = "Pixel contact shadow";
            for(int i=0;i<vertices.Count;i++)vertices[i]+=new Vector2(0.5f,0.5f);
            footprint.OverrideGeometry(vertices.ToArray(),triangles.ToArray());
        }
        var bounds = body.bounds;
        float width = Mathf.Clamp(bounds.size.x * 1.12f,0.6f,2f);
        for(int layer=0;layer<2;layer++)
        {
            var shape = new GameObject("Contact shadow");shape.transform.SetParent(transform,false);
            shape.transform.position = new Vector3(bounds.center.x,bounds.min.y+0.035f,transform.position.z+0.01f);
            float size=width*(layer==0?1f:0.78f);
            shape.transform.localScale=new Vector3(size/Mathf.Abs(transform.lossyScale.x),size/Mathf.Abs(transform.lossyScale.y),1);
            var shadow=shape.AddComponent<SpriteRenderer>();shadow.sprite=footprint;shadow.sharedMaterial=body.sharedMaterial;
            shadow.color=new Color(0.025f,0.03f,0.05f,layer==0?0.12f:0.24f);shadows.Add(shadow);
        }
        LateUpdate();
    }
    private void LateUpdate()
    {
        if(body==null)return;
        foreach(var shadow in shadows)
        {
            shadow.enabled=body.enabled&&body.sprite!=null;
            shadow.sortingLayerID=body.sortingLayerID;shadow.sortingOrder=body.sortingOrder-1;
        }
    }
}
