using UnityEngine;
using UnityEngine.InputSystem;

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
    private float originalFieldOfView;

    private void Start()
    {
        // Store the original camera settings
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.position;
            originalCameraRotation = mainCamera.transform.rotation;
            originalFieldOfView = mainCamera.fieldOfView;
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

        if (isOrbiting && (Keyboard.current.gKey.wasPressedThisFrame || Gamepad.current.buttonWest.wasPressedThisFrame)) // Map to Fire2
        {
            DetachFromOrbit(); // Detach from orbit
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
            mainCamera.fieldOfView = originalFieldOfView;
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
}
