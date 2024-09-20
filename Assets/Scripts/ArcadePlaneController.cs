using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ArcadePlaneController : MonoBehaviour
{
    public float thrustForce = 5000f;
    public float strafeForce = 2000f;
    public float pitchSensitivity = 1.5f;
    public float yawSensitivity = 1.5f;
    public float smoothStopRate = 2f;
    public float lookAtCenterSpeed = 2f;
    public float dashForceMultiplier = 2f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 1f;

    public ParticleSystem mainThrusterEffect;  // Main thruster effect
    public ParticleSystem leftThrusterEffect;   // Left thruster effect
    public ParticleSystem rightThrusterEffect;  // Right thruster effect

    private Rigidbody rb;
    private bool isLookingAtCenter = false;
    private bool isDashing = false;
    private float dashTime = 0f;
    private float cooldownTime = 0f;

    private InputAction restartAction; // InputAction for the restart button

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing!");
        }

        // Initialize the InputAction for the restart button
        restartAction = new InputAction(type: InputActionType.Button, binding: "<Gamepad>/start");
        restartAction.performed += OnRestart;
    }

    private void OnEnable()
    {
        restartAction.Enable(); // Enable the restart action
    }

    private void OnDisable()
    {
        restartAction.Disable(); // Disable the restart action
    }

    private void Start()
    {
        // Rotate spacecraft to ensure correct orientation
        transform.Rotate(0f, 180f, 0f);

        // Ensure thruster effects are off initially
        StopAllThrusterEffects();
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
        float stopInput = Gamepad.current.leftTrigger.ReadValue(); // Stop input (left trigger)

        // Toggle thruster effects
        HandleThrusterEffects(thrustInput);

        // Check if the left trigger is pressed to stop movement
        if (stopInput > 0f)
        {
            // Smoothly stop the ship by reducing forces to zero
            Vector3 velocity = rb.velocity;
            rb.velocity = Vector3.Lerp(velocity, Vector3.zero, Time.fixedDeltaTime * smoothStopRate);
            return; // Exit the method to skip further movement calculation
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

    private void HandleThrusterEffects(float thrustInput)
    {
        if (thrustInput > 0f)
        {
            if (mainThrusterEffect != null && !mainThrusterEffect.isPlaying)
            {
                mainThrusterEffect.Play(); // Play main thruster
            }

            // Play left and right thruster effects if boosting
            if (Gamepad.current.xButton.isPressed) // If boosting
            {
                if (leftThrusterEffect != null && !leftThrusterEffect.isPlaying)
                {
                    leftThrusterEffect.Play(); // Play left thruster
                }
                if (rightThrusterEffect != null && !rightThrusterEffect.isPlaying)
                {
                    rightThrusterEffect.Play(); // Play right thruster
                }
            }
        }
        else
        {
            StopAllThrusterEffects(); // Stop all effects if no thrust
        }
    }

    private void StopAllThrusterEffects()
    {
        if (mainThrusterEffect != null && mainThrusterEffect.isPlaying) mainThrusterEffect.Stop();
        if (leftThrusterEffect != null && leftThrusterEffect.isPlaying) leftThrusterEffect.Stop();
        if (rightThrusterEffect != null && rightThrusterEffect.isPlaying) rightThrusterEffect.Stop();
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

    private void OnRestart(InputAction.CallbackContext context)
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
