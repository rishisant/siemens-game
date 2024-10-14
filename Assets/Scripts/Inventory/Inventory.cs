using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Inventory : MonoBehaviour
{
    public List<CosmeticItem> cosmeticItems = new List<CosmeticItem>();

    public void AddItem(CosmeticItem item)
    {
        cosmeticItems.Add(item);
    }

    public void RemoveItem(CosmeticItem item)
    {
        cosmeticItems.Remove(item);
    }
}
