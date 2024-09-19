using UnityEngine;
using UnityEngine.InputSystem;

public class ArcadePlaneController : MonoBehaviour
{
    public float thrustForce = 5000f; // Increased forward thrust force
    public float strafeForce = 2000f; // Increased strafing force
    public float pitchSensitivity = 1.5f; // Increased sensitivity for pitch control
    public float yawSensitivity = 1.5f; // Increased sensitivity for yaw control
    public float smoothStopRate = 2f; // Lower rate at which the ship slows down to a stop for quicker deceleration
    public float lookAtCenterSpeed = 2f; // Speed at which the ship rotates to look at the center
    public float dashForceMultiplier = 2f; // Multiplier for dash force
    public float dashDuration = 0.5f; // Duration of the dash
    public float dashCooldown = 1f; // Cooldown time between dashes

    public ParticleSystem thrusterEffect; // Reference to the thruster particle system

    private Rigidbody rb; // Rigidbody component
    private bool isLookingAtCenter = false; // Flag to check if the ship is looking at the center
    private bool isDashing = false; // Flag to indicate if dashing
    private float dashTime = 0f; // Timer to keep track of dash duration
    private float cooldownTime = 0f; // Timer to keep track of cooldown time

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing!");
        }

        // Rotate spacecraft to ensure correct orientation
        transform.Rotate(0f, 180f, 0f);

        // Ensure thruster effect is off initially
        if (thrusterEffect != null)
        {
            thrusterEffect.Stop();
        }
    }

    private void Update()
    {
        if (Gamepad.current == null)
        {
            Debug.LogWarning("No gamepad detected. Please connect a gamepad.");
            return;
        }

        // Check for left bumper input to trigger look at center
        if (Gamepad.current.leftShoulder.wasPressedThisFrame)
        {
            isLookingAtCenter = true; // Start looking at the center immediately
        }

        // Check for dash input
        bool isWestButtonPressed = Gamepad.current.xButton.isPressed;
        bool isRightTriggerPressed = Gamepad.current.rightTrigger.ReadValue() > 0f;

        // Dash mechanic
        if (isWestButtonPressed && isRightTriggerPressed && !isDashing && cooldownTime <= 0f)
        {
            StartDash();
        }

        // Update cooldown and dash timers
        cooldownTime -= Time.deltaTime;
        if (isDashing)
        {
            dashTime -= Time.deltaTime;
            if (dashTime <= 0f)
            {
                EndDash();
            }
        }
    }

    private void FixedUpdate()
    {
        if (isLookingAtCenter)
        {
            LookAtCenter(); // Rotate towards the center if triggered
        }
        else
        {
            HandleMovement(); // Handle movement and rotation
        }
    }

    private void HandleMovement()
    {
        // Get input from the gamepad
        Vector2 lookInput = Gamepad.current.leftStick.ReadValue(); // Pitch and yaw: left stick
        Vector2 movementInput = Gamepad.current.rightStick.ReadValue(); // Strafing and thrust: right stick
        float thrustInput = Gamepad.current.rightTrigger.ReadValue(); // Forward thrust (right trigger)

        // Toggle thruster effect based on thrust input
        if (thrustInput > 0f)
        {
            if (thrusterEffect != null && !thrusterEffect.isPlaying)
            {
                thrusterEffect.Play();
            }
        }
        else
        {
            if (thrusterEffect != null && thrusterEffect.isPlaying)
            {
                thrusterEffect.Stop();
            }
        }

        // Apply dash force if dashing
        float currentThrustForce = isDashing ? thrustForce * dashForceMultiplier : thrustForce;

        // Calculate thrust direction with multiplier
        Vector3 thrustDirection = transform.forward * thrustInput * currentThrustForce;
        rb.AddForce(thrustDirection * Time.fixedDeltaTime, ForceMode.VelocityChange);

        // Calculate strafing direction with multiplier
        Vector3 strafeDirection = (transform.right * movementInput.x + transform.up * movementInput.y) * strafeForce;
        rb.AddForce(strafeDirection * Time.fixedDeltaTime, ForceMode.VelocityChange);

        // Calculate pitch and yaw with increased sensitivity
        float pitch = -lookInput.y * pitchSensitivity;
        float yaw = lookInput.x * yawSensitivity;

        // Create rotation around the local X and Y axes
        Quaternion pitchRotation = Quaternion.AngleAxis(pitch, transform.right);
        Quaternion yawRotation = Quaternion.AngleAxis(yaw, Vector3.up);

        // Combine rotations
        Quaternion targetRotation = pitchRotation * yawRotation * transform.rotation;

        // Smoothly rotate the spacecraft
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5f));

        // Smoothly stop the ship when there is no input
        if (thrustInput == 0 && movementInput == Vector2.zero)
        {
            Vector3 velocity = rb.velocity;
            rb.velocity = Vector3.Lerp(velocity, Vector3.zero, Time.fixedDeltaTime * smoothStopRate);
        }
    }

    private void LookAtCenter()
    {
        // Calculate the direction to look at the center (0, 0, 0)
        Vector3 directionToCenter = (Vector3.zero - transform.position).normalized;

        // Calculate the target rotation to face the center
        Quaternion targetRotation = Quaternion.LookRotation(directionToCenter, Vector3.up);

        // Smoothly rotate towards the center
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * lookAtCenterSpeed));

        // Stop looking at the center once the ship is facing it
        if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
        {
            isLookingAtCenter = false; // Stop looking at the center
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTime = dashDuration; // Reset dash timer
        cooldownTime = dashCooldown; // Reset cooldown timer
    }

    private void EndDash()
    {
        isDashing = false;
    }
}
