using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform spacecraft; // Reference to the spacecraft
    public Transform anchor; // Public anchor position for the camera
    public float smoothSpeed = 0.125f; // Speed of smoothing the camera movement

    private void LateUpdate()
    {
        if (spacecraft == null || anchor == null)
        {
            Debug.LogWarning("Spacecraft or anchor reference is missing!");
            return;
        }

        // Set the camera position to the anchor's position
        transform.position = anchor.position;

        // Smoothly look at the spacecraft
        Quaternion desiredRotation = Quaternion.LookRotation(spacecraft.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothSpeed);
    }
}
