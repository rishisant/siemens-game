using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Statistics class for wirePlug. This is mainly a dataclass that
 * This keeps track of whether the wire is connected
 * 
 * All logic for moving the wire is in PlugBehavior
 * @see PlugBehavior
 */
public class PlugStats : MonoBehaviour
{
    public bool connected = false;
}