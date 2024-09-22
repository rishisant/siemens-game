using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{
    // A Test behaves as an ordinary method
    [Test]
    public void NewTestScriptSanityCheck()
    {
        // Use the Assert class to test conditions
        Assert.AreEqual(1, 1, "1 == 1");
    }
}
