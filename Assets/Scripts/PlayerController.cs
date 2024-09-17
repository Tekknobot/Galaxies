using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f; // Speed of the player movement
    public Camera mainCamera; // Reference to the main camera
    public CameraControl cameraControl; // Reference to the CameraControl script
    public float orbitDistance = 15f; // Distance from the planet while orbiting
    public float orbitSpeed = 20f; // Speed at which the player orbits the planet
    public TerraformingEffect terraformingEffect; // Reference to the TerraformingEffect script
    public GameObject clonePrefab; // Prefab to clone
    public int numberOfClones = 3; // Number of clones to create

    private Transform targetPlanet; // The planet the player will orbit
    private bool isOrbiting = false; // Flag to determine if the player is orbiting

    private void Update()
    {
        // Get input from keyboard
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Handle vertical movement with Q and E keys
        if (Input.GetKey(KeyCode.Q))
        {
            Move(Vector3.down); // Move down along the Y-axis
        }
        else if (Input.GetKey(KeyCode.E))
        {
            Move(Vector3.up); // Move up along the Y-axis
        }
        else if (!isOrbiting)
        {
            // Handle movement relative to the camera's direction
            MoveRelativeToCamera(horizontal, vertical);
        }
        else if (isOrbiting && targetPlanet != null)
        {
            OrbitPlanet(); // Orbit the planet diagonally
        }

        // Optionally focus the camera on the player or the planet it's orbiting when pressing 'F'
        if (cameraControl != null && Input.GetKeyDown(KeyCode.F))
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

        // If the player presses 'G', detach from orbit
        if (isOrbiting && Input.GetKeyDown(KeyCode.G))
        {
            DetachFromOrbit(); // Detach from orbit
        }

        // Check for mouse click on the player
        if (Input.GetMouseButtonDown(0)) // Left mouse button click
        {
            HandleMouseClick();
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
            cameraControl.FocusOnPlayer(transform); // Focus the camera on the player

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

        // Stop the terraforming effect
        if (terraformingEffect != null)
        {
            terraformingEffect.StopTerraformingEffect();
        }
    }

    void HandleMouseClick()
    {
        // Create a ray from the camera to the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform the raycast to check if the player object was clicked
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object is the player
            if (hit.transform == transform && isOrbiting && targetPlanet != null)
            {
                CreateClonesAtOrbit();
            }
        }
    }

void CreateClonesAtOrbit()
{
    float adjustedOrbitDistance = 1f; // Adjust this value to set the desired orbit distance

    // Create multiple clones
    for (int i = 0; i < numberOfClones; i++)
    {
        // Calculate a unique starting angle for each clone
        float angle = i * (360f / numberOfClones); // Spread the clones evenly around the orbit

        // Calculate a position offset for each clone
        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        Vector3 offset = rotation * Vector3.forward * adjustedOrbitDistance;
        Vector3 clonePosition = targetPlanet.position + offset;

        // Instantiate the clone
        GameObject clone = Instantiate(clonePrefab, clonePosition, Quaternion.identity);

        // Set up the clone's behavior
        PlayerCloneController cloneController = clone.GetComponent<PlayerCloneController>();
        if (cloneController != null)
        {
            cloneController.InitializeClone(targetPlanet, adjustedOrbitDistance, orbitSpeed, angle);
        }
    }
}

}
