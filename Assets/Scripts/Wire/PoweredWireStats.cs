using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PoweredWireStats : MonoBehaviour
{
    public bool movable = false;
    public bool moving = false;
    public bool connected = false;
    public Vector3 startPosition;
    public string color;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
