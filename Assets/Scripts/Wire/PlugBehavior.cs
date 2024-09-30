using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlugBehavior : MonoBehaviour
{
    public PlugStats plugS;
    // Start is called before the first frame update
    void Start()
    {
        plugS = gameObject.GetComponent<PlugStats>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(plugS.color))
        {
            other.gameObject.transform.position = new Vector3(transform.position.x - 0.57f, transform.position.y, transform.position.z);
            other.gameObject.GetComponent<PoweredWireStats>().connected = true;
            other.gameObject.GetComponent<PoweredWireBehavior>().UpdateLine();

        }
        else
        {
            // reset all other wires since there was a fail
        }
    }
}
