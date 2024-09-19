using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlanetaryCamera : MonoBehaviour
{
    public Camera mainCamera; // Reference to the main camera
    public Camera planetCamera; // Reference to the planetary camera
    public float rotationSpeed = 1f; // Speed at which the camera rotates to look at the planet
    public string planetTag = "Planet"; // Tag used to identify planet objects
    public float retryInterval = 1f; // Interval in seconds to retry finding planets

    // Camera offset variables to control position relative to the planet
    public float cameraOffsetX = 0f; // Horizontal offset
    public float cameraOffsetY = 20f; // Vertical offset
    public float cameraOffsetZ = 50f; // Distance offset along Z-axis

    // Reference to the player movement script
    public MonoBehaviour playerMovement; // Replace with your actual player movement script or controller

    private List<Transform> planets = new List<Transform>(); // List of planets to focus on
    private int currentPlanetIndex = 0; // Index of the current planet in the list
    private bool isPlanetaryCameraActive = false; // Is the planetary camera active
    private Transform currentTarget; // Current planet to focus on
    private float timeSinceLastRetry = 0f; // Timer for retrying to find planets

    void Start()
    {
        planetCamera.enabled = false; // Ensure the planetary camera is disabled at the start
        FindPlanets(); // Initial attempt to find planets
    }

    void Update()
    {
        // Retry finding planets if the list is empty
        if (planets.Count == 0)
        {
            timeSinceLastRetry += Time.deltaTime;
            if (timeSinceLastRetry >= retryInterval)
            {
                FindPlanets();
                timeSinceLastRetry = 0f;
            }
        }

        // Check for gamepad input to switch cameras
        HandleCameraSwitchInput();

        // If the planetary camera is not active, return early
        if (!isPlanetaryCameraActive || planets.Count == 0)
            return;

        // Smoothly rotate the camera to look at the current planet
        if (currentTarget != null)
        {
            // Position the camera relative to the target using the offsets
            PositionCameraRelativeToTarget(currentTarget);

            // Smoothly rotate the camera to look at the planet
            Vector3 directionToPlanet = currentTarget.position - planetCamera.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlanet);
            planetCamera.transform.rotation = Quaternion.Slerp(planetCamera.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void FindPlanets()
    {
        planets.Clear(); // Clear existing list to refresh it

        // Find all planets with the specified tag and store them in the list
        GameObject[] planetObjects = GameObject.FindGameObjectsWithTag(planetTag);
        foreach (GameObject planet in planetObjects)
        {
            planets.Add(planet.transform);
        }

        if (planets.Count == 0)
        {
            Debug.LogWarning("No planets found with tag: " + planetTag);
        }
        else
        {
            Debug.Log(planets.Count + " planets found with tag: " + planetTag);
        }
    }

    private void HandleCameraSwitchInput()
    {
        if (Gamepad.current != null)
        {
            // East button (e.g., B button) to cycle to the next planet or activate the planetary camera
            if (Gamepad.current.bButton.wasPressedThisFrame)
            {
                if (isPlanetaryCameraActive)
                {
                    CycleToNextPlanet();
                }
                else
                {
                    ActivatePlanetaryCamera();
                }
            }

            // South button (e.g., A button) to revert to the main camera
            if (Gamepad.current.aButton.wasPressedThisFrame && isPlanetaryCameraActive)
            {
                DeactivatePlanetaryCamera();
            }
        }
    }

    public void ActivatePlanetaryCamera()
    {
        if (mainCamera != null)
        {
            mainCamera.enabled = false; // Disable the main camera
        }

        if (planetCamera != null)
        {
            planetCamera.enabled = true; // Enable the planetary camera
        }

        isPlanetaryCameraActive = true;
        currentPlanetIndex = 0; // Start with the first planet in the list
        SetCurrentTarget();
        if (playerMovement != null)
        {
            playerMovement.enabled = false; // Disable player movement
        }
    }

    public void DeactivatePlanetaryCamera()
    {
        if (planetCamera != null)
        {
            planetCamera.enabled = false; // Disable the planetary camera
        }

        if (mainCamera != null)
        {
            mainCamera.enabled = true; // Enable the main camera
        }

        isPlanetaryCameraActive = false;
        currentTarget = null;
        if (playerMovement != null)
        {
            playerMovement.enabled = true; // Re-enable player movement
        }
    }

    public void CycleToNextPlanet()
    {
        if (planets == null || planets.Count == 0)
            return;

        currentPlanetIndex = (currentPlanetIndex + 1) % planets.Count; // Cycle to the next planet
        SetCurrentTarget();
    }

    public void CycleToPreviousPlanet()
    {
        if (planets == null || planets.Count == 0)
            return;

        currentPlanetIndex = (currentPlanetIndex - 1 + planets.Count) % planets.Count; // Cycle to the previous planet
        SetCurrentTarget();
    }

    private void SetCurrentTarget()
    {
        if (planets != null && planets.Count > 0)
        {
            currentTarget = planets[currentPlanetIndex];
        }
    }

    private void PositionCameraRelativeToTarget(Transform target)
    {
        if (target != null)
        {
            // Position the camera using the X, Y, and Z offsets relative to the planet
            Vector3 offset = new Vector3(cameraOffsetX, cameraOffsetY, cameraOffsetZ);
            planetCamera.transform.position = target.position + offset;
        }
    }

    private void OnDrawGizmos()
    {
        // Optional: Draw a line from the camera to the current target for debugging purposes
        if (currentTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(planetCamera.transform.position, currentTarget.position);
        }
    }
}
