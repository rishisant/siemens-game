using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlugStats : MonoBehaviour
{
    public bool connected = false;
    public string color;
    // Start is called before the first frame update
    void Start()
    {
        color = gameObject.tag;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
