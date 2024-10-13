using System.Collections;
using System.Collections.Generic;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;

public class PoweredWireBehavior : MonoBehaviour
{
    bool mouseDown = false;
    public PoweredWireStats powerWireS;
    LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {
        powerWireS = gameObject.GetComponent<PoweredWireStats>();
        line = gameObject.GetComponentInParent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveWire();
    }

    void OnMouseDown()
    {
        mouseDown = true;
    }

    void OnMouseOver()
    {
        if (powerWireS.connected)
        {
            return;
        }

        powerWireS.movable = true;
    }

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

    public void UpdateLine()
    {
        line.SetPosition(1, new Vector3(gameObject.transform.position.x - .1f, gameObject.transform.position.y, 0));
    }
}