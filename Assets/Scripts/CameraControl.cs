using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float followSpeed = 10f;         // Speed at which the camera follows the planet
    public float smoothTime = 0.3f;         // Time for smoothing the movement
    public float followDistance = 10f;      // Fixed distance from the planet
    public float zoomInDistance = 5f;       // Distance to zoom in after collision
    public float zoomSpeed = 5f;            // Speed of zooming

    public Transform player;                // Reference to the player object
    public bool focusOnPlayer = false;      // Flag to determine if camera should focus on player

    private Camera cam;                     // Reference to the camera component
    private Transform targetPlanet;         // The planet to follow
    private Vector3 velocity = Vector3.zero; // For smoothing the movement
    private Vector3 originalPosition;       // The original position of the camera
    private Quaternion originalRotation;    // The original rotation of the camera
    private bool returningToOriginal = false; // Flag to determine if returning to original view
    private bool isZooming = false;         // Flag to indicate if we are zooming in on collision
    private bool isReturningToPlayer = false; // Flag to determine if returning to player

    void Start()
    {
        cam = GetComponent<Camera>(); // Get the camera component
        originalPosition = transform.position; // Store the original position
        originalRotation = transform.rotation; // Store the original rotation
    }

    void Update()
    {
        HandleMouseInput();
        HandleZoom();

        if (focusOnPlayer && player != null)
        {
            FocusOnPlayer();
        }
        else if (isZooming)
        {
            PerformZoom();
        }
        else if (targetPlanet != null && !returningToOriginal)
        {
            FocusOnPlanet();
        }
        else if (returningToOriginal)
        {
            ReturnToOriginalView();
        }
        else if (isReturningToPlayer && player != null)
        {
            FocusOnPlayer();
        }
    }

    void FocusOnPlayer()
    {
        if (player == null)
            return;

        // Calculate the desired position relative to the player
        Vector3 desiredPosition = player.position - transform.forward * followDistance;

        // Smoothly move the camera to the desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

        // Make the camera look at the player
        transform.LookAt(player);
    }

    void FocusOnPlanet()
    {
        if (targetPlanet == null)
            return;

        // Calculate the desired position relative to the target planet
        Vector3 desiredPosition = targetPlanet.position - transform.forward * followDistance;

        // Smoothly move the camera to the desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

        // Make the camera look at the target planet
        transform.LookAt(targetPlanet);
    }

    void ReturnToOriginalView()
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
            // Set flag to return to player view
            //isReturningToPlayer = true;
        }
    }

    void HandleZoom()
    {
        if (targetPlanet != null && !isZooming) // Only zoom when following a planet and not in zoom mode
        {
            // Scroll wheel to zoom in/out
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            // Modify follow distance based on scroll input, with clamping to avoid extreme distances
            followDistance = Mathf.Clamp(followDistance - scrollInput * 5f, 2f, 50f);  // Adjust the multiplier for sensitivity
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
                    focusOnPlayer = false; // Stop focusing on the player when focusing on a planet
                }
            }
        }
        else if (Input.GetMouseButtonDown(1)) // Right click
        {
            returningToOriginal = true; // Start returning to the original view
            isZooming = false; // Ensure zooming is stopped
            focusOnPlayer = false; // Stop focusing on the player
            isReturningToPlayer = false; // Ensure returning to player is stopped
        }
    }

    public void FocusOnPlanet(Transform planet)
    {
        targetPlanet = planet;
        returningToOriginal = false; // Stop returning to original view
        focusOnPlayer = false; // Stop focusing on the player

        // Start zooming
        isZooming = true;
    }

    public void FocusOnPlayer(Transform playerTransform)
    {
        player = playerTransform;
        targetPlanet = null; // Ensure we stop following the planet
        returningToOriginal = false; // Stop returning to the original view

        // Set flag to focus on the player
        focusOnPlayer = true;
        isReturningToPlayer = false; // Stop returning to player
    }

    void PerformZoom()
    {
        if (targetPlanet == null)
            return;

        // Calculate the zoom target position
        Vector3 zoomPosition = targetPlanet.position - transform.forward * zoomInDistance;

        // Smoothly move the camera towards the zoom target
        transform.position = Vector3.SmoothDamp(transform.position, zoomPosition, ref velocity, zoomSpeed * Time.deltaTime);

        // Look at the planet during zoom
        transform.LookAt(targetPlanet);

        // Stop zooming when close enough
        if (Vector3.Distance(transform.position, zoomPosition) < 0.1f)
        {
            isZooming = false; // Stop zooming
        }
    }
}
