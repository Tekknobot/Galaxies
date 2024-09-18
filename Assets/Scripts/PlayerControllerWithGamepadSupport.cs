using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerControllerWithGamepadSupport : MonoBehaviour
{
    public float speed = 10f; // Speed of the player movement
    public Camera mainCamera; // Reference to the main camera
    public CameraControl cameraControl; // Reference to the CameraControl script
    public float orbitDistance = 15f; // Distance from the planet while orbiting
    public float orbitSpeed = 20f; // Speed at which the player orbits the planet
    public TerraformingEffect terraformingEffect; // Reference to the TerraformingEffect script
    public LineRenderer lineRenderer; // Reference to the LineRenderer component

    private Transform targetPlanet; // The planet the player will orbit
    private bool isOrbiting = false; // Flag to determine if the player is orbiting

    private Vector2 movementInput;
    private float verticalInput;
    private bool focusOnPlanet;

    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;

    // Zoom settings
    public float zoomSpeed = 5f; // Speed of zoom using bumpers
    public float minZoomDistance = 5f; // Minimum zoom distance to the player/target
    public float maxZoomDistance = 50f; // Maximum zoom distance to the player/target

    private float currentZoomDistance; // Current distance between the camera and the player/target

    // List of all planets in the scene
    private List<Transform> planets = new List<Transform>();
    private int currentPlanetIndex = -1; // Index of the currently focused planet

    private void Start()
    {
        // Store the original camera settings
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.position;
            originalCameraRotation = mainCamera.transform.rotation;
            currentZoomDistance = Vector3.Distance(mainCamera.transform.position, transform.position); // Initialize zoom distance
        }

        // Find all planets in the scene (assuming they are tagged "Planet")
        GameObject[] planetObjects = GameObject.FindGameObjectsWithTag("Planet");
        foreach (GameObject planet in planetObjects)
        {
            planets.Add(planet.transform);
        }
    }

    private void Update()
    {
        // Get input from keyboard or controller
        movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        verticalInput = Input.GetAxis("Vertical");
        focusOnPlanet = Keyboard.current.fKey.wasPressedThisFrame || Gamepad.current.buttonSouth.wasPressedThisFrame; // Map to Fire3

        if (Keyboard.current.qKey.isPressed || Gamepad.current.leftTrigger.isPressed)
        {
            Move(Vector3.down); // Move down along the Y-axis
        }
        else if (Keyboard.current.eKey.isPressed || Gamepad.current.rightTrigger.isPressed)
        {
            Move(Vector3.up); // Move up along the Y-axis
        }
        else if (!isOrbiting)
        {
            // Handle movement relative to the camera's direction (for both keyboard and controller)
            MoveRelativeToCamera(movementInput.x, movementInput.y);
        }
        else if (isOrbiting && targetPlanet != null)
        {
            OrbitPlanet(); // Orbit the planet diagonally
        }

        if (cameraControl != null && focusOnPlanet)
        {
            if (isOrbiting && targetPlanet != null)
            {
                cameraControl.FocusOnPlanet(targetPlanet); // Focus on the planet
            }
            else
            {
                cameraControl.FocusOnPlayer(transform); // Focus on the player
            }
        }

        if (Keyboard.current.gKey.wasPressedThisFrame || Gamepad.current.buttonEast.wasPressedThisFrame) // Map to Fire2
        {
            CycleThroughPlanets(); // Cycle through planets on buttonWest press
        }

        if (isOrbiting && (Keyboard.current.gKey.wasPressedThisFrame || Gamepad.current.buttonWest.wasPressedThisFrame)) // Map to Fire2
        {
            DetachFromOrbit(); // Detach from orbit
        }

        // Handle zoom using the bumpers (left for zoom in, right for zoom out)
        if (Gamepad.current != null)
        {
            HandleZoomWithBumpers();
        }

        // Reset the camera to its original position when the north button (Y or Triangle) is pressed
        if (Gamepad.current.buttonNorth.wasPressedThisFrame)
        {
            ResetCameraToOriginalPosition();
        }
    }

    void MoveRelativeToCamera(float horizontalInput, float verticalInput)
    {
        // Get the forward and right directions of the camera, ignoring the Y-axis
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        // Flatten the camera's forward and right directions on the XZ plane (ignore Y)
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate the movement direction based on the camera's orientation
        Vector3 moveDirection = (cameraForward * verticalInput + cameraRight * horizontalInput).normalized;

        // Move the player in the calculated direction
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    void Move(Vector3 direction)
    {
        // Move the player in the specified direction
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player collided with a planet
        if (collision.gameObject.CompareTag("Planet"))
        {
            targetPlanet = collision.transform; // Set the target planet
            isOrbiting = true; // Start orbiting
            cameraControl.FocusOnPlanet(targetPlanet.transform); // Focus the camera on the planet

            // Position the player at the correct orbit distance from the planet
            Vector3 directionFromPlanet = (transform.position - targetPlanet.position).normalized;
            transform.position = targetPlanet.position + directionFromPlanet * orbitDistance;

            // Start the terraforming effect
            if (terraformingEffect != null)
            {
                terraformingEffect.StartOrbiting(targetPlanet);
            }
        }
    }

    void OrbitPlanet()
    {
        if (targetPlanet == null)
            return;

        // Calculate diagonal axis for orbiting
        Vector3 diagonalAxis = (Vector3.up + Vector3.right).normalized; // Combining up and right for diagonal

        // Rotate the player diagonally around the planet using the diagonal axis
        transform.RotateAround(targetPlanet.position, diagonalAxis, orbitSpeed * Time.deltaTime);

        // Ensure the player maintains the correct orbit distance from the planet
        Vector3 directionFromPlanet = (transform.position - targetPlanet.position).normalized;
        transform.position = targetPlanet.position + directionFromPlanet * orbitDistance;
    }

    void DetachFromOrbit()
    {
        // Stop orbiting
        isOrbiting = false;
        targetPlanet = null; // Clear the target planet to detach

        // Allow the player to freely move again
        cameraControl.FocusOnPlayer(transform); // Re-focus the camera on the player

        // Reset the camera to its original position and settings
        if (mainCamera != null)
        {
            mainCamera.transform.position = originalCameraPosition;
            mainCamera.transform.rotation = originalCameraRotation;
        }

        // Stop the terraforming effect
        if (terraformingEffect != null)
        {
            terraformingEffect.StopTerraformingEffect();
        }

        // Stop LineRenderer rendering
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false; // Disable LineRenderer
        }
    }

    void CycleThroughPlanets()
    {
        if (planets.Count == 0)
            return;

        // Increment the current planet index and wrap around if necessary
        currentPlanetIndex = (currentPlanetIndex + 1) % planets.Count;

        // Get the new target planet
        Transform newTargetPlanet = planets[currentPlanetIndex];

        // Focus the camera on the new target planet
        cameraControl.FocusOnPlanet(newTargetPlanet);
    }

    void HandleZoomWithBumpers()
    {
        // Get the direction to move the camera (towards the player or target)
        Vector3 directionToPlayer = (mainCamera.transform.position - transform.position).normalized;

        // Zoom in when pressing the right shoulder (bumper)
        if (Gamepad.current.rightShoulder.isPressed)
        {
            currentZoomDistance -= zoomSpeed * Time.deltaTime;
        }

        // Zoom out when pressing the left shoulder (bumper)
        if (Gamepad.current.leftShoulder.isPressed)
        {
            currentZoomDistance += zoomSpeed * Time.deltaTime;
        }

        // Clamp the zoom distance between the min and max limits
        currentZoomDistance = Mathf.Clamp(currentZoomDistance, minZoomDistance, maxZoomDistance);

        // Update the camera's position based on the new zoom distance
        mainCamera.transform.position = transform.position + directionToPlayer * currentZoomDistance;
    }

    void ResetCameraToOriginalPosition()
    {
        // Reset the camera's position and rotation to the original values from game start
        if (mainCamera != null)
        {
            // Reset camera's position and rotation
            mainCamera.transform.position = originalCameraPosition;
            mainCamera.transform.rotation = originalCameraRotation;

            // Reset the zoom distance to the original zoom distance
            currentZoomDistance = Vector3.Distance(originalCameraPosition, transform.position);

            // Recalculate the camera's position based on the reset zoom distance
            Vector3 directionToPlayer = (mainCamera.transform.position - transform.position).normalized;
            mainCamera.transform.position = transform.position + directionToPlayer * currentZoomDistance;
        }
    }

}
