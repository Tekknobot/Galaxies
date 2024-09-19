using UnityEngine;
using UnityEngine.InputSystem;

public class SpacecraftController : MonoBehaviour
{
    public float thrustForce = 500f; // Forward thrust force
    public float strafeForce = 250f; // Strafing force
    public float rotationSpeed = 30f; // Speed of rotation
    public float pitchSensitivity = 1f; // Sensitivity for pitch control
    public float yawSensitivity = 1f; // Sensitivity for yaw control
    public float rollSensitivity = 1f; // Sensitivity for roll control

    private Rigidbody rb; // Rigidbody component
    private float currentThrust = 0f; // Current thrust value
    private float thrustAcceleration = 5f; // Acceleration rate for thrust
    private float thrustDeceleration = 5f; // Deceleration rate for thrust

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing!");
        }
    }

    private void FixedUpdate()
    {
        if (Gamepad.current == null)
        {
            Debug.LogWarning("No gamepad detected. Please connect a gamepad.");
            return;
        }

        // Get input from the gamepad
        Vector2 leftStickInput = Gamepad.current.leftStick.ReadValue(); // Pitch control: left stick Y-axis
        Vector2 rightStickInput = Gamepad.current.rightStick.ReadValue(); // Yaw control: right stick X-axis
        float thrustInput = Gamepad.current.rightTrigger.ReadValue(); // Forward thrust (right trigger)
        float reverseThrustInput = Gamepad.current.leftTrigger.ReadValue(); // Reverse thrust (left trigger)

        // Calculate thrust direction and smooth thrust control
        float targetThrust = Mathf.Clamp(thrustInput - reverseThrustInput, 0f, 1f) * thrustForce;
        currentThrust = Mathf.MoveTowards(currentThrust, targetThrust, thrustAcceleration * Time.deltaTime);
        Vector3 thrustDirection = transform.forward * currentThrust;

        // Apply thrust force
        rb.AddForce(thrustDirection * Time.deltaTime);

        // Apply strafing (left/right and up/down)
        Vector2 strafeInput = Gamepad.current.leftStick.ReadValue();
        Vector3 strafeDirection = transform.right * strafeInput.x + transform.up * strafeInput.y;
        rb.AddForce(strafeDirection * strafeForce * Time.deltaTime);

        // Apply rotation (pitch, yaw, and roll)
        float pitch = -leftStickInput.y * rotationSpeed * pitchSensitivity * Time.deltaTime; // Negative to invert pitch
        float yaw = rightStickInput.x * rotationSpeed * yawSensitivity * Time.deltaTime;

        // Create rotation around the local X and Y axes
        Quaternion pitchRotation = Quaternion.AngleAxis(pitch, transform.right);
        Quaternion yawRotation = Quaternion.AngleAxis(yaw, transform.up);

        // Combine rotations
        Quaternion targetRotation = pitchRotation * yawRotation * transform.rotation;

        // Smoothly rotate the spacecraft
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime));
    }
}
