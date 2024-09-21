using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform sun;               // The central object around which this object will orbit (e.g., Sun or Planet)
    public float orbitSpeed = 10.0f;    // Speed of the orbit
    public float distanceFromSun = 10.0f;  // Distance from the central object (sun or planet)

    private Vector3 orbitAxis = Vector3.up;  // The axis around which the object will orbit

    void Update()
    {
        // Check if the sun (central object) is assigned
        if (sun == null)
        {
            Debug.LogWarning($"{name} has no central object assigned for orbiting.");
            return;
        }

        // Calculate the orbit position
        transform.RotateAround(sun.position, orbitAxis, orbitSpeed * Time.deltaTime);

        // Ensure the object stays at the correct distance from the sun
        Vector3 desiredPosition = (transform.position - sun.position).normalized * distanceFromSun + sun.position;
        transform.position = desiredPosition;
    }
}
