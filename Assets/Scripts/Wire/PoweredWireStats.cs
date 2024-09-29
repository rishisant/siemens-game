using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Colors { blue, red, yellow, green, white };

public class PoweredWireStats : MonoBehaviour
{
    public bool movable = false;
    public bool moving = false;
    public Vector3 startPosition;
    public Colors objectColor;

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
