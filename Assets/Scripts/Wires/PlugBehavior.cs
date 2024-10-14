using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Behavior logic for a wirePlug
 * 
 * Makes use of the PlugStats class to keep track of variables
 * @see PlugStats
 */
public class PlugBehavior : MonoBehaviour
{
    // PlugStats member to keep track of various flags
    public PlugStats plugS;

    /**
     * Start() is a Unity function that is called before the first frame update
     * 
     * This just grabs the appropriate members using the GetComponent() Unity function
     * PlugStats for the wire is grabbed
     */
    void Start()
    {
        plugS = gameObject.GetComponent<PlugStats>();
    }

    /**
     * OnTriggerEnter2D() is a Unity function that runs when another Collider2D
     * enters the zone of this gameObject's Collider2D
     * 
     * This function handles the collision of a wireEntry and a wirePlug. If
     * they are the same color then they are locked into place and are marked
     * as connected, otherwise nothing happens.
     * 
     * @param other The other Collider2D that is currently colliding with this gameObject
     */
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