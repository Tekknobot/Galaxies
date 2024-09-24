using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float minSpeed = 10f; // Minimum speed of the projectile
    public float maxSpeed = 20f; // Maximum speed of the projectile
    public float accelerationScale = 1.5f; // Factor by which to increase the speed
    public float accelerationDuration = 5f; // Time duration over which to accelerate
    public GameObject fragmentPrefab; // Prefab for projectile fragments
    public int fragmentCount = 3; // Number of fragments on collision
    public float fragmentForce = 30f; // Force applied to fragments on collision
    public float fadeDuration = 0.5f; // Duration of the fade-out effect

    private Rigidbody rb;
    private bool hasCollided = false; // Track if the projectile has collided

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Check if the Rigidbody component is assigned correctly
        if (rb == null)
        {
            Debug.LogError("No Rigidbody attached to " + gameObject.name);
            return;
        }

        // Set the initial velocity for the projectile
        Vector3 direction = transform.forward; // Use forward direction for the projectile
        float speed = Random.Range(minSpeed, maxSpeed); // Select random speed within the range
        rb.velocity = direction * speed;

        Debug.Log("Projectile " + gameObject.name + " initialized with speed " + speed);

        // Start the acceleration coroutine
        StartCoroutine(IncreaseVelocityOverTime());
    }

    private IEnumerator IncreaseVelocityOverTime()
    {
        // Wait for 2 seconds before starting to increase speed
        yield return new WaitForSeconds(0f);

        float initialSpeed = rb.velocity.magnitude; // Get the current speed
        float targetSpeed = initialSpeed * accelerationScale; // Calculate target speed

        float elapsedTime = 0f;

        // Gradually increase the speed to the target speed
        while (elapsedTime < accelerationDuration)
        {
            // Interpolate between the initial speed and target speed
            float currentSpeed = Mathf.Lerp(initialSpeed, targetSpeed, elapsedTime / accelerationDuration);
            rb.velocity = rb.velocity.normalized * currentSpeed; // Maintain direction and update speed

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the final speed is set to target speed
        rb.velocity = rb.velocity.normalized * targetSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the projectile has already collided
        if (!hasCollided)
        {
            hasCollided = true; // Set the flag to true to prevent further collisions

            // Turn off the mesh renderer
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false; // Disable the mesh renderer
            }

            // Fragmentation logic
            for (int i = 0; i < fragmentCount; i++)
            {
                GameObject fragment = Instantiate(fragmentPrefab, transform.position, Random.rotation);
                Rigidbody fragmentRb = fragment.GetComponent<Rigidbody>();
                if (fragmentRb != null)
                {
                    fragmentRb.AddExplosionForce(fragmentForce, transform.position, 3f);
                }
            }

            // Start fade-out before destroying the projectile
            StartCoroutine(FadeAndDestroy());
        }
    }

    private IEnumerator FadeAndDestroy()
    {
        Renderer renderer = GetComponent<Renderer>();
        Color originalColor = renderer.material.color;
        float time = 0;

        // Fade out
        while (time < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            renderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            time += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the projectile is completely transparent before destroying
        renderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        Destroy(gameObject);
    }
}
