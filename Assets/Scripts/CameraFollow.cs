using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform spacecraft; // Reference to the spacecraft
    public Vector3 offset = new Vector3(0f, 5f, -10f); // Offset from the spacecraft
    public float smoothSpeed = 0.125f; // Speed of smoothing the camera movement

    private void LateUpdate()
    {
        if (spacecraft == null)
        {
            Debug.LogWarning("Spacecraft reference is missing!");
            return;
        }

        // Desired position of the camera
        Vector3 desiredPosition = spacecraft.position + spacecraft.TransformDirection(offset);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Make the camera follow the rotation of the spacecraft
        transform.rotation = Quaternion.Slerp(transform.rotation, spacecraft.rotation, smoothSpeed);
    }
}
