using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float followSpeed = 10f;     // Speed at which the camera follows the planet
    public float smoothTime = 0.3f;     // Time for smoothing the movement
    public float followDistance = 10f;  // Fixed distance from the planet

    private Camera cam;                 // Reference to the camera component
    private Transform targetPlanet;     // The planet to follow
    private Vector3 velocity = Vector3.zero; // For smoothing the movement
    private Vector3 originalPosition;   // The original position of the camera
    private Quaternion originalRotation; // The original rotation of the camera
    private bool returningToOriginal = false; // Flag to determine if returning to original view

    void Start()
    {
        cam = GetComponent<Camera>(); // Get the camera component
        originalPosition = transform.position; // Store the original position
        originalRotation = transform.rotation; // Store the original rotation
    }

    void Update()
    {
        HandleMouseInput();

        if (targetPlanet != null && !returningToOriginal)
        {
            // Calculate the desired position relative to the target planet
            Vector3 desiredPosition = targetPlanet.position - transform.forward * followDistance;

            // Smoothly move the camera to the desired position
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

            // Optionally, make the camera look at the target planet
            transform.LookAt(targetPlanet);
        }
        else if (returningToOriginal)
        {
            // Smoothly move the camera back to the original position and rotation
            transform.position = Vector3.SmoothDamp(transform.position, originalPosition, ref velocity, smoothTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, smoothTime);

            // Check if the camera has returned to the original position and rotation
            if (Vector3.Distance(transform.position, originalPosition) < 0.1f &&
                Quaternion.Angle(transform.rotation, originalRotation) < 1f)
            {
                returningToOriginal = false; // Stop returning when close enough
                targetPlanet = null; // Ensure we stop following the planet
            }
        }
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Planet"))
                {
                    targetPlanet = hit.collider.transform;
                    returningToOriginal = false; // Stop returning to original view when starting to follow a planet
                }
            }
        }
        else if (Input.GetMouseButtonDown(1)) // Right click
        {
            returningToOriginal = true; // Start returning to the original view
        }
    }
}
