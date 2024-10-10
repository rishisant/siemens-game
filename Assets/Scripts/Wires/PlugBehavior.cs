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

    // Logic to handle collisions with wires and plugs
    public void OnTriggerEnter2D(Collider2D other)
    {
        SpriteRenderer otherSpriteRenderer = other.GetComponent<SpriteRenderer>();
        if (otherSpriteRenderer == null)
        {
            Debug.Log("otherSpriteRenderer is null");
            return;
        }

        SpriteRenderer thisSpriteRenderer = GetComponent<SpriteRenderer>();
        if (otherSpriteRenderer.color == thisSpriteRenderer.color)
        {
            other.gameObject.transform.position = new Vector3(transform.position.x - 0.4f, transform.position.y, transform.position.z);
            other.gameObject.GetComponent<PoweredWireStats>().connected = true;
            other.gameObject.GetComponent<PoweredWireBehavior>().UpdateLine();

            plugS.connected = true;
        }
    }
}