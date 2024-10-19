using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class WirePrefabTest
{
    private string wireEntryPrefabPath = "Assets/Prefabs/Wires/WireEntry.prefab";
    private string wirePlugPrefabPath = "Assets/Prefabs/Wires/WirePlug.prefab";

    [Test]
    public void WirePrefabTestSimplePasses()
    {
        GameObject wireEntry = AssetDatabase.LoadAssetAtPath<GameObject>(wireEntryPrefabPath);
        GameObject wirePlug = AssetDatabase.LoadAssetAtPath<GameObject>(wirePlugPrefabPath);

        Assert.IsNotNull(wireEntry, "wireEntry is not at " + wireEntryPrefabPath);
        Assert.IsNotNull(wirePlug, "wirePlug is not at " + wirePlugPrefabPath);
    }
}
