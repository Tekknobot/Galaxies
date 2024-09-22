using System.Collections;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float speed = 5f; // Speed of the asteroid
    public GameObject fragmentPrefab; // Prefab for asteroid fragments
    public int fragmentCount = 5; // Number of fragments on collision
    public float fragmentForce = 50f; // Force applied to fragments on collision
    public Transform targetPlanet; // Reference to the target planet

    // Trail Renderer attributes
    public float trailTime = 1.5f; // Duration of the trail
    public float trailStartWidth = 0.5f; // Width at the start of the trail
    public float trailEndWidth = 0.1f; // Width at the end of the trail
    public Color trailStartColor = new Color(1f, 1f, 0f, 1f); // Start color (yellow)
    public Color trailEndColor = new Color(1f, 0.5f, 0f, 0f); // End color (orange to transparent)

    public Vector3 asteroidScale = new Vector3(1f, 1f, 1f); // Scale for the asteroid
    public float fadeDuration = 1f; // Duration of the fade-out effect

    private Rigidbody rb;
    private bool hasCollided = false; // Track if the asteroid has collided

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Scale the asteroid
        transform.localScale = asteroidScale;

        // Check if targetPlanet is set
        if (targetPlanet == null)
        {
            Debug.LogError("Target Planet is not set for " + gameObject.name);
            return; // Exit if no target
        }

        // Calculate direction and set velocity towards the center of the planet
        Vector3 direction = (targetPlanet.position - transform.position).normalized;
        rb.velocity = direction * speed;

        Debug.Log("Asteroid " + gameObject.name + " initialized towards the center of " + targetPlanet.name);
        
        // Add and configure the trail renderer for the asteroid
        TrailRenderer trail = gameObject.AddComponent<TrailRenderer>();
        trail.time = trailTime; // Duration of the trail
        trail.startWidth = trailStartWidth; // Width at the start of the trail
        trail.endWidth = trailEndWidth; // Width at the end of the trail
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.startColor = trailStartColor; // Start color
        trail.endColor = trailEndColor; // End color
        trail.autodestruct = true; // Automatically destroy the trail
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // Disable shadows for trail
        trail.minVertexDistance = 0.1f; // Distance between vertices for smoother trails

        // Optional: Add a light to enhance the trail effect
        Light trailLight = gameObject.AddComponent<Light>();
        trailLight.type = LightType.Point;
        trailLight.color = Color.yellow;
        trailLight.range = 5f;
        trailLight.intensity = 2f;
        trailLight.shadows = LightShadows.None; // Disable shadows for the light
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the asteroid has already collided
        if (!hasCollided)
        {
            hasCollided = true; // Set the flag to true to prevent further collisions

            // Fragmentation logic
            for (int i = 0; i < fragmentCount; i++)
            {
                GameObject fragment = Instantiate(fragmentPrefab, transform.position, Random.rotation);
                Rigidbody fragmentRb = fragment.GetComponent<Rigidbody>();
                if (fragmentRb != null)
                {
                    fragmentRb.AddExplosionForce(fragmentForce, transform.position, 5f);
                }
            }

            // Start fade-out before destroying the asteroid
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

        // Ensure the asteroid is completely transparent before destroying
        renderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        Destroy(gameObject);
    }
}
