using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Networking.PlayerConnection;
using UnityEngine.Networking;
using UnityEngine;

/**
 * Behavior logic for a poweredWire
 * 
 * Makes heavy use of the PoweredWireStats class to keep track of variables
 * @see PoweredWireStats
 */
public class PoweredWireBehavior : MonoBehaviour
{
    // bool for if the mouse is down
    bool mouseDown = false;

    // statistics for this individual wire
    public PoweredWireStats powerWireS;

    // the line renderer that makes these sprites work together to look like a wire
    LineRenderer line;

    /**
     * Start() is a Unity function that is called before the first frame update
     * 
     * This just grabs the appropriate members using the GetComponent() Unity function
     * both powerWireStats and the LineRenderer are grabbed
     */
    void Start()
    {
        powerWireS = gameObject.GetComponent<PoweredWireStats>();
        line = gameObject.GetComponentInParent<LineRenderer>();
    }

    /**
     * Update() is a Unity function that is called every frame update
     *
     * This calls the MoveWire() function
     */
    void Update()
    {
        MoveWire();
    }

    /**
     * OnMouseDown() is a Unity function that runs whenever the mouse is down
     * 
     * This just sets the mouseDown variable to true
     */
    void OnMouseDown()
    {
        mouseDown = true;
    }

    /**
     * OnMouseOver() is a Unity function that runs whenever the mouse is over this gameObject
     * 
     * This sets the wire to be movable if it's not already connected
     */
    void OnMouseOver()
    {
        if (powerWireS.connected)
        {
            return;
        }

        powerWireS.movable = true;
    }

    /**
     * OnMouseExit() is a Unity function that runs whenever the mouse stops hovering over the GUI element
     * 
     * This sets the wire to no longer be movable if it's not currently moving
     */
    void OnMouseExit()
    {
        if (powerWireS.connected)
        {
            return;
        }

        if (!powerWireS.moving)
        {
            powerWireS.movable = false;
        }
    }

    /**
     * OnMouseUp() is a Unity function that runs whenever the user has released the mouse button
     * 
     * This resets the wire to its start position and calls the UpdateLine() function
     */
    void OnMouseUp()
    {
        if (powerWireS.connected)
        {
            return;
        }

        mouseDown = false;
        gameObject.transform.position = powerWireS.startPosition;
        UpdateLine();
    }

    /**
     * MoveWire() handles the logic for moving the wire
     * 
     * The wire will follow the position of the Input, which is normally the mouse or the finger
     */
    void MoveWire()
    {
        if (powerWireS.connected)
        {
            return;
        }

        if (mouseDown && powerWireS.movable)
        {
            powerWireS.moving = true;
            float mouseX = Input.mousePosition.x;
            float mouseY = Input.mousePosition.y;

            gameObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mouseX, mouseY, 1));
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, transform.parent.transform.position.z);

            UpdateLine();
        }
        else
        {
            powerWireS.moving = false;
        }
    }

    /**
     * UpdateLine() will handle the LineRenderer updates that need to happen
     * The LineRenderer trails behind the front object (wireLength) by 0.1 units
     */
    public void UpdateLine()
    {
        line.SetPosition(1, new Vector3(gameObject.transform.position.x - .1f, gameObject.transform.position.y, 0));
    }
}