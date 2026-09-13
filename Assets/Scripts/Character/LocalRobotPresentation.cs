using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

/** @brief Local-only name and pixel silhouette through foreground scenery. */
[DefaultExecutionOrder(500)]
public class LocalRobotPresentation : MonoBehaviour
{
    private readonly List<SpriteRenderer> sources = new List<SpriteRenderer>();
    private readonly List<SpriteRenderer> outlines = new List<SpriteRenderer>();
    private Renderer[] scenery;
    private Material highlight, maskMaterial;
    private RenderTexture mask;
    private TextMeshPro nameplate;
    private CommandBuffer commands;
    private int overlayLayer;
    private void Start()
    {
        int highest=int.MinValue;foreach(var layer in SortingLayer.layers) if(layer.value>highest){highest=layer.value;overlayLayer=layer.id;}
        highlight = new Material(Resources.Load<Shader>("Effects/SpriteHighlight"));highlight.SetFloat("_Occluded",1);
        highlight.SetColor("_Color",new Color(.70f,.88f,1));
        maskMaterial = new Material(Resources.Load<Shader>("Effects/OccluderMask"));
        scenery = FindObjectsOfType<Renderer>();
        foreach(var source in GetComponentsInChildren<SpriteRenderer>())
        {
            if(source.name == "Contact shadow") continue;
            sources.Add(source);
            var outline = new GameObject("Local silhouette").AddComponent<SpriteRenderer>();
            outline.transform.SetParent(source.transform,false);outline.sharedMaterial = highlight;
            outline.sortingLayerID = overlayLayer;outline.sortingOrder = 30000;RobotDepth.Overlay(outline.gameObject,30000);outlines.Add(outline);
        }
        nameplate = new GameObject("Your name").AddComponent<TextMeshPro>();
        nameplate.transform.SetParent(transform,false);nameplate.transform.localPosition = new Vector3(0,1.15f,-.1f);
        nameplate.font = ByteCityTheme.Font;nameplate.fontSize = 2.1f;nameplate.richText = false;
        nameplate.alignment = TextAlignmentOptions.Center;nameplate.rectTransform.sizeDelta = new Vector2(4,.7f);
        RobotDepth.Overlay(nameplate.gameObject,30001);
        nameplate.color = new Color(.85f,.94f,1);nameplate.GetComponent<MeshRenderer>().sortingLayerID = overlayLayer;nameplate.GetComponent<MeshRenderer>().sortingOrder = 30001;
        commands = new CommandBuffer { name = "Local robot occlusion" };
        InteractionHighlight.Install(gameObject.scene);
    }
    private void LateUpdate()
    {
        if(nameplate == null || Camera.main == null) return;
        nameplate.transform.position = new Vector3(sources[0].bounds.center.x,sources[0].bounds.max.y+.24f,transform.position.z-.1f);
        nameplate.text = PlayerData.Instance != null ? PlayerData.Instance.username : "Explorer";
        var camera = Camera.main;
        int w=Mathf.Max(1,camera.pixelWidth/2),h=Mathf.Max(1,camera.pixelHeight/2);
        if(mask == null || mask.width != w || mask.height != h)
        {
            if(mask != null) { mask.Release();Destroy(mask); }
            mask = new RenderTexture(w,h,0,RenderTextureFormat.ARGB32);mask.Create();
        }
        commands.Clear();commands.SetRenderTarget(mask);commands.ClearRenderTarget(false,true,Color.clear);
        commands.SetViewProjectionMatrices(camera.worldToCameraMatrix,GL.GetGPUProjectionMatrix(camera.projectionMatrix,true));
        Bounds bounds = sources[0].bounds;
        foreach(var source in sources) bounds.Encapsulate(source.bounds);
        bounds.extents = new Vector3(bounds.extents.x,bounds.extents.y,1000);
        foreach(var renderer in scenery)
        {
            if(renderer == null || !renderer.enabled || !renderer.gameObject.activeInHierarchy || renderer.transform.IsChildOf(transform)) continue;
            if(!(renderer is SpriteRenderer) && !(renderer is TilemapRenderer)) continue;
            if(!renderer.bounds.Intersects(bounds)) continue;
            int layer = SortingLayer.GetLayerValueFromID(renderer.sortingLayerID);
            if(renderer.GetComponentInParent<TownRemoteAvatar>()!=null)continue;
            var robotGroup=GetComponent<SortingGroup>();
            int ownLayer = SortingLayer.GetLayerValueFromID(robotGroup!=null?robotGroup.sortingLayerID:sources[0].sortingLayerID);
            bool ahead = layer > ownLayer || (layer == ownLayer && (renderer.sortingOrder > sources[0].sortingOrder || (renderer.sortingOrder == sources[0].sortingOrder && renderer.transform.position.z < transform.position.z)));
            if(ahead) commands.DrawRenderer(renderer,maskMaterial);
        }
        Graphics.ExecuteCommandBuffer(commands);highlight.SetTexture("_ByteCityOccluders",mask);
        for(int i=0;i<sources.Count;i++)
        {
            var source=sources[i];var outline=outlines[i];outline.sprite=source.sprite;
            outline.enabled=source.enabled;outline.flipX=source.flipX;outline.flipY=source.flipY;
            outline.sortingLayerID=overlayLayer;
        }
    }
    private void OnDestroy()
    {
        if(commands != null) commands.Release();
        if(mask != null) { mask.Release();Destroy(mask); }
        if(highlight != null) Destroy(highlight);if(maskMaterial != null) Destroy(maskMaterial);
    }
}
