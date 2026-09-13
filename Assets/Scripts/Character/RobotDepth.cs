using UnityEngine;
using UnityEngine.Rendering;

/** Keeps one robot's body and outfit together, ordered by its feet. */
[RequireComponent(typeof(SortingGroup))]
public class RobotDepth : MonoBehaviour
{
    private SortingGroup group;
    private void Awake(){group=GetComponent<SortingGroup>();group.sortingLayerName="Clothing";}
    private void LateUpdate(){group.sortingOrder=Mathf.Clamp(Mathf.RoundToInt(-transform.position.y*100),-30000,30000);}
    public static void Overlay(GameObject item,int order)
    {
        var group=item.AddComponent<SortingGroup>();group.sortAtRoot=true;
        int highest=int.MinValue;foreach(var layer in SortingLayer.layers)if(layer.value>highest){highest=layer.value;group.sortingLayerID=layer.id;}
        group.sortingOrder=order;
    }
}
