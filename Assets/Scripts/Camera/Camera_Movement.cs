using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Movement : MonoBehaviour
{
    [SerializeField] private Transform target; // The player transform to follow
    [SerializeField] private Vector3 offset; // Offset from the player

    // Update is called once per frame
    void LateUpdate()
    {
        // Check if the target is not null (player/target is selected)
        if (target != null)
        {
            // Move the camera to the player's position
            transform.position = target.position + offset;
            transform.LookAt(target);
        }
    }
}
