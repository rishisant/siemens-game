using System.Collections.Generic;
using TMPro;
using UnityEngine;

/** @brief Remote robot with grouped artwork and collision against the local player only. */
public class TownRemoteAvatar : MonoBehaviour
{
    private Vector3 destination;
    private Rigidbody2D body;
    private readonly List<Animator> animators=new List<Animator>();
    private readonly List<string> prefixes=new List<string>();
    private readonly List<SpriteRenderer> renderers=new List<SpriteRenderer>();
    private CosmeticHandler cosmetics;
    private string animationState="",equipment=null;
    public void Initialize(Character_Movement original,TownPlayerState state)
    {
        cosmetics=FindObjectOfType<CosmeticHandler>();
        transform.position=new Vector3(state.x,state.y,0);destination=transform.position;
        gameObject.AddComponent<RobotDepth>();
        var sourceCollider=original.GetComponent<Collider2D>();
        if(sourceCollider!=null)
        {
            body=gameObject.AddComponent<Rigidbody2D>();body.bodyType=RigidbodyType2D.Kinematic;body.interpolation=RigidbodyInterpolation2D.Interpolate;
            body.constraints=RigidbodyConstraints2D.FreezeRotation;
            var collider=gameObject.AddComponent<BoxCollider2D>();collider.size=sourceCollider.bounds.size;
            collider.offset=Vector2.Scale(sourceCollider.offset,original.transform.lossyScale);
            // Remote state cannot activate doors, NPC conversations or local puzzle triggers.
            foreach(var other in FindObjectsOfType<Collider2D>())if(other!=collider && (other.isTrigger || !other.transform.IsChildOf(original.transform)))Physics2D.IgnoreCollision(collider,other);
        }
        foreach(var source in original.GetComponentsInChildren<SpriteRenderer>())
        {
            if(source.name=="Contact shadow" || source.name=="Local silhouette" || source.name=="Interaction glint")continue;
            var part=new GameObject(source.name);part.transform.SetParent(transform,false);
            part.transform.localPosition=source.transform.position-original.transform.position;
            part.transform.localScale=source.transform.lossyScale;
            var sprite=part.AddComponent<SpriteRenderer>();sprite.sprite=source.sprite;sprite.color=source.color;sprite.sortingLayerID=source.sortingLayerID;sprite.sortingOrder=source.sortingOrder;
            if(source.transform==original.transform)RobotShadow.Attach(sprite);
            var sourceAnimator=source.GetComponent<Animator>();
            if(sourceAnimator!=null)
            {
                var animator=part.AddComponent<Animator>();animator.runtimeAnimatorController=sourceAnimator.runtimeAnimatorController;
                string lower=source.name.ToLowerInvariant();
                string prefix=source.transform==original.transform?"Char":lower.Contains("chest")?"Chest":lower.Contains("leg")?"Leg":lower.Contains("shoe")?"Shoe":"Hat";
                animators.Add(animator);prefixes.Add(prefix);renderers.Add(sprite);
            }
        }
        var nameplate=new GameObject("Name").AddComponent<TextMeshPro>();nameplate.transform.SetParent(transform,false);nameplate.transform.localPosition=new Vector3(0,1.15f,-0.1f);
        nameplate.text=state.name;nameplate.richText=false;nameplate.font=ByteCityTheme.Font;nameplate.fontSize=2.1f;nameplate.alignment=TextAlignmentOptions.Center;nameplate.rectTransform.sizeDelta=new Vector2(4,0.7f);nameplate.raycastTarget=false;
        nameplate.GetComponent<MeshRenderer>().sortingOrder=200;RobotDepth.Overlay(nameplate.gameObject,30001);
    }
    public void Apply(TownPlayerState state)
    {
        destination=new Vector3(state.x,state.y,0);
        string nextEquipment=string.Join(",",state.equipped??new int[0]);
        if(nextEquipment!=equipment)
        {
            equipment=nextEquipment;animationState="";
            for(int i=0;i<animators.Count;i++)
            {
                string prefix=prefixes[i];if(prefix=="Char")continue;
                int category=prefix=="Hat"?100:prefix=="Chest"?200:prefix=="Leg"?300:400;
                int chosen=-1;foreach(int id in state.equipped??new int[0])if(id>=category&&id<category+100)chosen=id;
                if(chosen<0||chosen%100==99) { animators[i].runtimeAnimatorController=null;renderers[i].sprite=null;continue; }
                if(cosmetics!=null)animators[i].runtimeAnimatorController=prefix=="Hat"?cosmetics.GetHatController(chosen-100):prefix=="Chest"?cosmetics.GetChestController(chosen-200):prefix=="Leg"?cosmetics.GetLegController(chosen-300):cosmetics.GetShoeController(chosen-400);
            }
        }
        string next=(state.moving?"Walk_":"Idle_")+state.facing;
        if(next==animationState)return;animationState=next;
        for(int i=0;i<animators.Count;i++)if(animators[i].runtimeAnimatorController!=null)animators[i].Play(prefixes[i]+"_"+next);
    }
    private void FixedUpdate()
    {
        Vector2 next=Vector2.Lerp(transform.position,destination,1-Mathf.Exp(-16*Time.fixedDeltaTime));
        if(Vector3.Distance(transform.position,destination)>5){if(body!=null)body.position=destination;else transform.position=destination;}
        else if(body!=null)body.MovePosition(next);else transform.position=next;
    }
}
