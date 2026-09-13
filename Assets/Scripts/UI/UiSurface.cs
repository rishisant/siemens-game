using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/** @brief Resizes original UI artwork while preserving the pixel corners. */
public class UiSurface : Image
{
    private static readonly Dictionary<string,Sprite> slices=new Dictionary<string,Sprite>();
    protected override void Awake()
    {
        base.Awake();
        if(sprite==null && ByteCityTheme.Current!=null)Use(ByteCityTheme.Current.panel,80,8);
    }
    public void Use(Sprite source,float border=0,float multiplier=1)
    {
        if(source==null)return;
        source.texture.filterMode=FilterMode.Point;
        if(border>0)
        {
            string key=source.GetInstanceID()+":"+border;
            Sprite sliced;
            if(!slices.TryGetValue(key,out sliced)||sliced==null)
            {
                sliced=Sprite.Create(source.texture,source.rect,new Vector2(.5f,.5f),source.pixelsPerUnit,0,SpriteMeshType.FullRect,new Vector4(border,border,border,border));
                sliced.name=source.name+" (resizable)";slices[key]=sliced;
            }
            sprite=sliced;type=Type.Sliced;pixelsPerUnitMultiplier=multiplier;
        }
        else {sprite=source;type=Type.Simple;}
        color=Color.white;
    }
}
