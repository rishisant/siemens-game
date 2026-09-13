using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/** @brief A breathing edge and traveling glint on nearby interactive scenery. */
public class InteractionHighlight : MonoBehaviour
{
    private static readonly HashSet<string> Types = new HashSet<string> {"EnterRoom","Pipe_Interact","Wire_Interact","CardGame_Enter","Deckmaster_Interact","Leaderboard_Interact","ShopOwner_Dialogue","Drunkard_Dialogue","Casino_Dialogue"};
    private readonly List<SpriteRenderer> originals = new List<SpriteRenderer>();
    private readonly List<SpriteRenderer> glows = new List<SpriteRenderer>();
    private Character_Movement player;
    private Material material;
    private LineRenderer marker;
    public static void Install(Scene scene)
    {
        foreach(var root in scene.GetRootGameObjects())
            foreach(var script in root.GetComponentsInChildren<MonoBehaviour>(true))
                if(script != null && Types.Contains(script.GetType().Name) && script.GetComponent<InteractionHighlight>() == null)
                    script.gameObject.AddComponent<InteractionHighlight>();
    }
    private void Start()
    {
        player=FindObjectOfType<Character_Movement>();
        material=new Material(Resources.Load<Shader>("Effects/SpriteHighlight"));material.SetColor("_Color",new Color(.72f,.84f,1));
        var sprites=GetComponentsInChildren<SpriteRenderer>();
        if(sprites.Length == 0 && transform.parent != null) sprites=transform.parent.GetComponentsInChildren<SpriteRenderer>();
        if(sprites.Length == 0)
        {
            SpriteRenderer nearest=null;float best=3f;
            foreach(var candidate in FindObjectsOfType<SpriteRenderer>())
            {
                if(candidate.name=="Contact shadow" || candidate.name=="Interaction glint" || candidate.GetComponentInParent<Character_Movement>()!=null) continue;
                float distance=Vector2.Distance(candidate.bounds.center,transform.position);
                if(distance<best) {best=distance;nearest=candidate;}
            }
            sprites=nearest==null?new SpriteRenderer[0]:new[]{nearest};
        }
        if(sprites.Length == 0)
        {
            marker=new GameObject("Door beacon").AddComponent<LineRenderer>();marker.transform.SetParent(transform,false);
            marker.useWorldSpace=false;marker.loop=true;marker.positionCount=4;marker.widthMultiplier=.035f;
            marker.SetPositions(new[]{new Vector3(0,.24f,0),new Vector3(.17f,0,0),new Vector3(0,-.24f,0),new Vector3(-.17f,0,0)});
            marker.sharedMaterial=new Material(Shader.Find("Sprites/Default"));marker.sortingOrder=200;
        }
        foreach(var source in sprites)
        {
            if(source.name == "Contact shadow" || source.GetComponentInParent<Character_Movement>() != null || Vector3.Distance(source.bounds.center,transform.position)>6) continue;
            originals.Add(source);var glow=new GameObject("Interaction glint").AddComponent<SpriteRenderer>();
            glow.transform.SetParent(source.transform,false);glow.sharedMaterial=material;glows.Add(glow);
        }
    }
    private void LateUpdate()
    {
        if(player == null) return;
        float distance=Vector2.Distance(player.transform.position,transform.position);
        float amount=player.CanMove?Mathf.Clamp01((5-distance)/2):0;
        if(marker != null)
        {
            marker.enabled=amount>0;marker.transform.localPosition=new Vector3(0,.55f+.07f*Mathf.Sin(Time.time*2),-.1f);
            marker.startColor=marker.endColor=new Color(.72f,.84f,1,amount*(.55f+.2f*Mathf.Sin(Time.time*2)));
        }
        for(int i=0;i<originals.Count;i++)
        {
            var source=originals[i];var glow=glows[i];if(source == null) continue;
            glow.enabled=source.enabled && amount>0;glow.sprite=source.sprite;glow.flipX=source.flipX;glow.flipY=source.flipY;
            glow.color=new Color(1,1,1,amount);glow.sortingLayerID=source.sortingLayerID;glow.sortingOrder=source.sortingOrder+1;
        }
    }
    private void OnDestroy() { if(material != null) Destroy(material);if(marker != null) Destroy(marker.sharedMaterial); }
}
