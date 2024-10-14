using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Statistics class for powered wires. This is mainly a dataclass that
 * keeps track of the wires start position and other helpful flags for an
 * individual wire
 * 
 * All logic for moving the wire is in PoweredWireBehavior
 * @see PoweredWireBehavior
 */
public class PoweredWireStats : MonoBehaviour
{
    // bool for if the wire is able to be moved. mainly for checking that it's not
    // already being acted upon by a mouse or finger, or already connected
    public bool movable = false;

    // bool for if the wire is currently moving
    public bool moving = false;

    // bool for if the wire is connected to the correct wirePlug
    public bool connected = false;

    // the initial position of the wireLength
    public Vector3 startPosition;

    /**
     * Start() is a Unity function that is called before the first frame
     * update. All that happens inside this is that the startPosition member is
     * set to the current transform.position
     */
    void Start()
    {
        startPosition = transform.position;
    }
}