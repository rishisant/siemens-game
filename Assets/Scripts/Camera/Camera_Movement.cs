using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Other fields
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset;

    // How much the camera takes up on the screen
    private float cameraZoomSize = 3.0f;
    
    // Given a target, pan the camera to the target
    private void SwitchTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Coroutine to switch target for a certain duration and switch back to old target
    public void PanCamera(Transform newTarget)
    {
        // Call the coroutine
        StartCoroutine(PanCameraCoroutine(newTarget));
    }

    private IEnumerator PanCameraCoroutine(Transform newTarget)
    {
        // Save the old target
        Transform oldTarget = target;

        // Switch to the new target
        SwitchTarget(newTarget);

        // Make camera zoom in
        // Old size
        float oldSize = Camera.main.orthographicSize;
        Camera.main.orthographicSize = cameraZoomSize;

        // Wait for 2 seconds
        yield return new WaitForSeconds(2.0f);

        // Switch back to the old target
        SwitchTarget(oldTarget);

        // Make camera zoom out
        Camera.main.orthographicSize = oldSize;
    }

    private void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        smoothedPosition.z = transform.position.z; // Keep Z-Position the Same (2D)
        transform.position = smoothedPosition;
    }
}