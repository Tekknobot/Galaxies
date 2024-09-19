using UnityEngine;
using UnityEngine.InputSystem;

public class ArcadePlaneController : MonoBehaviour
{
    public float thrustForce = 1000f; // Forward thrust force
    public float strafeForce = 500f; // Strafing force
    public float pitchSensitivity = 1f; // Sensitivity for pitch control
    public float yawSensitivity = 1f; // Sensitivity for yaw control
    public float smoothStopRate = 5f; // Rate at which the ship slows down to a stop

    private Rigidbody rb; // Rigidbody component
    private Vector3 lastVelocity; // Store the last velocity to handle smooth stopping

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing!");
        }

        // Rotate spacecraft to ensure correct orientation
        transform.Rotate(0f, 180f, 0f);
    }

    private void FixedUpdate()
    {
        if (Gamepad.current == null)
        {
            Debug.LogWarning("No gamepad detected. Please connect a gamepad.");
            return;
        }

        // Get input from the gamepad
        Vector2 lookInput = Gamepad.current.leftStick.ReadValue(); // Pitch and yaw: left stick
        Vector2 movementInput = Gamepad.current.rightStick.ReadValue(); // Strafing: right stick
        float thrustInput = Gamepad.current.rightTrigger.ReadValue(); // Forward thrust (right trigger)

        // Calculate thrust direction
        Vector3 thrustDirection = transform.forward * thrustInput * thrustForce;
        rb.AddForce(thrustDirection * Time.deltaTime, ForceMode.Acceleration);

        // Calculate strafing direction
        Vector3 strafeDirection = transform.right * movementInput.x + transform.up * movementInput.y;
        rb.AddForce(strafeDirection * strafeForce * Time.deltaTime, ForceMode.Acceleration);

        // Calculate pitch and yaw
        float pitch = -lookInput.y * pitchSensitivity;
        float yaw = lookInput.x * yawSensitivity;

        // Create rotation around the local X and Y axes
        Quaternion pitchRotation = Quaternion.AngleAxis(pitch, transform.right);
        Quaternion yawRotation = Quaternion.AngleAxis(yaw, Vector3.up);

        // Combine rotations
        Quaternion targetRotation = pitchRotation * yawRotation * transform.rotation;

        // Smoothly rotate the spacecraft
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f));

        // Smoothly stop the ship when there is no input
        if (thrustInput == 0 && movementInput == Vector2.zero)
        {
            Vector3 velocity = rb.velocity;
            rb.velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * smoothStopRate);
        }
    }
}
