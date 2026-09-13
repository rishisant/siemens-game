using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/** A schematic locator: labeled destinations and a moving player arrow. */
public class TownMinimap : MonoBehaviour
{
    private class Place { public string name; public Vector2 position; public Color color; }
    private readonly List<Place> places=new List<Place>();
    private Character_Movement player;
    private RectTransform frame, marker, safe;
    private Rect lastSafe;
    private Canvas canvas;
    private TMP_Text location;
    private Vector2 center, previousPosition;
    private float halfSize;
    private const float MapPixels=178;
    private static readonly Color Mint=new Color(.42f,1,.78f), Blue=new Color(.4f,.82f,1), Gold=new Color(1,.8f,.38f), Pink=new Color(1,.55f,.8f);
    public float RightEdgeOnScreen { get { return frame==null?0:RectTransformUtility.WorldToScreenPoint(null,frame.TransformPoint(new Vector3(frame.rect.xMax,0,0))).x; } }
    public int LandmarkCount { get { return places.Count; } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register(){SceneManager.sceneLoaded-=OnScene;SceneManager.sceneLoaded+=OnScene;}
    private static void OnScene(Scene scene,LoadSceneMode mode)
    {
        if(scene.name=="Town_Square" || scene.name=="Laboratory_Main" || scene.name=="Casino_Main" || scene.name=="Tutorial")
            new GameObject("Town minimap").AddComponent<TownMinimap>();
    }
    private IEnumerator Start()
    {
        yield return null;
        player=FindObjectOfType<Character_Movement>();
        if(player==null){Destroy(gameObject);yield break;}
        previousPosition=player.transform.position;
        foreach(var door in FindObjectsOfType<EnterRoom>())if(door.MapDestination!="")AddPlace(door.MapDestination,door.transform.position,door.MapDestination=="LAB"?Blue:Gold);
        foreach(var shop in FindObjectsOfType<ShopOwner_Dialogue>())AddPlace("SHOP",shop.transform.position,Pink);
        foreach(var pipes in FindObjectsOfType<Pipe_Interact>())AddPlace("PIPES",pipes.transform.position,Blue);
        foreach(var wires in FindObjectsOfType<Wire_Interact>())AddPlace("WIRES",wires.transform.position,Gold);
        foreach(var cards in FindObjectsOfType<Deckmaster_Interact>())AddPlace("CARDS",cards.transform.position,Pink);
        foreach(var host in FindObjectsOfType<Casino_Dialogue>())AddPlace("HOST",host.transform.position,Pink);
        if(places.Count==0)foreach(var tutor in FindObjectsOfType<TutorialManager>())AddPlace("START",player.transform.position,Blue);
        Bounds bounds=new Bounds(player.transform.position,Vector3.one*12);
        foreach(var place in places)bounds.Encapsulate(place.position);
        center=bounds.center;halfSize=Mathf.Max(12,Mathf.Max(bounds.extents.x,bounds.extents.y)+8);

        var root=WorldUi.Canvas("Minimap",25);root.SetParent(transform,false);canvas=root.GetComponent<Canvas>();
        root.GetComponent<CanvasScaler>().screenMatchMode=CanvasScaler.ScreenMatchMode.Expand;
        safe=WorldUi.Rect("Safe area",root,Vector2.zero,Vector2.zero,Vector2.zero,Vector2.zero);WorldUi.SafeArea(safe);lastSafe=Screen.safeArea;
        frame=WorldUi.Rect("Map frame",safe,Vector2.zero,Vector2.zero,new Vector2(16,MobileControls.Enabled?300:16),new Vector2(204,237));
        frame.gameObject.AddComponent<UiSurface>().raycastTarget=false;
        Text(frame,"AREA MAP",new Vector2(13,210),new Vector2(140,18),12,WorldUi.Ink);
        Text(frame,"N ^",new Vector2(153,210),new Vector2(38,18),10,WorldUi.Ink);
        var view=WorldUi.Rect("Map diagram",frame,Vector2.zero,Vector2.zero,new Vector2(13,31),Vector2.one*MapPixels);
        Fill(view,new Color(.075f,.09f,.14f));
        // Quiet coordinate guides support orientation without drawing scenery or invented roads.
        for(int i=1;i<4;i++)
        {
            var x=WorldUi.Rect("Grid",view,Vector2.zero,Vector2.zero,new Vector2(i*MapPixels/4,0),new Vector2(1,MapPixels));Fill(x,new Color(.14f,.17f,.24f));
            var y=WorldUi.Rect("Grid",view,Vector2.zero,Vector2.zero,new Vector2(0,i*MapPixels/4),new Vector2(MapPixels,1));Fill(y,new Color(.14f,.17f,.24f));
        }
        var occupied=new List<Rect>();
        foreach(var place in places)
        {
            Vector2 position=Project(place.position);
            var dot=WorldUi.Rect(place.name+" destination",view,Vector2.one*.5f,Vector2.one*.5f,position,Vector2.one*10);
            dot.gameObject.AddComponent<MapMarkerGraphic>().color=place.color;
            Vector2 labelPosition=position+new Vector2(-27,-19);
            for(int attempt=0;attempt<4;attempt++)
            {
                labelPosition.x=Mathf.Clamp(labelPosition.x,-MapPixels*.5f+2,MapPixels*.5f-56);
                labelPosition.y=Mathf.Clamp(labelPosition.y,-MapPixels*.5f+2,MapPixels*.5f-14);
                Rect candidate=new Rect(labelPosition,new Vector2(54,13));
                if(!occupied.Exists(r=>r.Overlaps(candidate))){occupied.Add(candidate);break;}
                labelPosition=position+new Vector2(-27,attempt%2==0?9:-31);
            }
            Text(view,place.name,labelPosition+Vector2.one*MapPixels*.5f,new Vector2(54,13),10,place.color).alignment=TextAlignmentOptions.Center;
        }
        marker=WorldUi.Rect("You are here",view,Vector2.one*.5f,Vector2.one*.5f,Vector2.zero,Vector2.one*16);
        var arrow=marker.gameObject.AddComponent<MapMarkerGraphic>();arrow.arrow=true;arrow.color=Mint;
        Text(frame,"YOU",new Vector2(13,8),new Vector2(35,17),10,WorldUi.Ink);
        var key=WorldUi.Rect("You key",frame,Vector2.zero,Vector2.zero,new Vector2(49,12),Vector2.one*9);var keySymbol=key.gameObject.AddComponent<MapMarkerGraphic>();keySymbol.arrow=true;keySymbol.color=Mint;
        location=Text(frame,"",new Vector2(64,8),new Vector2(126,17),9,WorldUi.Ink);location.alignment=TextAlignmentOptions.MidlineRight;
    }
    private void AddPlace(string name,Vector2 position,Color color)
    {
        // Several machines can share the same activity; show one destination for the cluster.
        if(places.Exists(p=>p.name==name && Vector2.Distance(p.position,position)<7))return;
        places.Add(new Place{name=name,position=position,color=color});
    }
    private Vector2 Project(Vector2 position)
    {var p=(position-center)/(halfSize*2)*MapPixels;return new Vector2(Mathf.Clamp(p.x,-82,82),Mathf.Clamp(p.y,-82,82));}
    private static void Fill(RectTransform rect,Color color){var image=rect.gameObject.AddComponent<Image>();image.color=color;image.raycastTarget=false;}
    private static TMP_Text Text(Transform parent,string value,Vector2 position,Vector2 size,float font,Color color)
    {var text=WorldUi.Text(value,parent,value,font,color);WorldUi.Place(text.rectTransform,Vector2.zero,Vector2.zero,position,size);text.enableAutoSizing=true;text.fontSizeMin=8;text.fontSizeMax=font;text.enableWordWrapping=false;return text;}
    private void LateUpdate()
    {
        if(marker==null || player==null)return;
        if(Screen.safeArea!=lastSafe){WorldUi.SafeArea(safe);lastSafe=Screen.safeArea;}
        canvas.enabled=!WorldUiFocus.Blocked;
        Vector2 position=player.transform.position;
        marker.anchoredPosition=Project(position);
        Vector2 movement=position-previousPosition;
        if(movement.sqrMagnitude>.00001f)marker.localRotation=Quaternion.Euler(0,0,Mathf.Atan2(movement.y,movement.x)*Mathf.Rad2Deg-90);
        marker.localScale=Vector3.one*(1+.09f*Mathf.Sin(Time.unscaledTime*4));previousPosition=position;
        Place nearest=null;float distance=float.MaxValue;
        foreach(var place in places){float d=Vector2.Distance(position,place.position);if(d<distance){nearest=place;distance=d;}}
        location.text=nearest!=null && distance<6?"NEAR "+nearest.name:"";
    }
}

/** Crisp dots and a directional arrow, drawn as small UI shapes. */
[RequireComponent(typeof(CanvasRenderer))]
public class MapMarkerGraphic : MaskableGraphic
{
    public bool arrow;
    protected override void Awake(){base.Awake();raycastTarget=false;}
    protected override void OnPopulateMesh(VertexHelper mesh)
    {
        mesh.Clear();float radius=Mathf.Min(rectTransform.rect.width,rectTransform.rect.height)*.5f;
        Shape(mesh,radius,new Color(.035f,.045f,.07f));Shape(mesh,radius*.68f,color);
    }
    private void Shape(VertexHelper mesh,float radius,Color tint)
    {
        int start=mesh.currentVertCount;mesh.AddVert(Vector3.zero,tint,Vector2.zero);
        int count=arrow?3:12;
        for(int i=0;i<count;i++){float angle=(90+i*360f/count)*Mathf.Deg2Rad;mesh.AddVert(new Vector3(Mathf.Cos(angle)*radius,Mathf.Sin(angle)*radius,0),tint,Vector2.zero);}
        for(int i=0;i<count;i++)mesh.AddTriangle(start,start+1+(i+1)%count,start+1+i);
    }
}
