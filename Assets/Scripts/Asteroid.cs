using System.Collections;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float minSpeed = 3f; // Minimum speed of the asteroid
    public float maxSpeed = 8f; // Maximum speed of the asteroid
    public GameObject fragmentPrefab; // Prefab for asteroid fragments
    public int fragmentCount = 5; // Number of fragments on collision
    public float fragmentForce = 50f; // Force applied to fragments on collision
    public Transform targetPlanet; // Reference to the target planet

    // Trail Renderer attributes
    public float trailTime = 1.5f; // Duration of the trail
    public float trailStartWidth = 0.5f; // Width at the start of the trail
    public float trailEndWidth = 0.1f; // Width at the end of the trail
    public Material trailMaterial; // Public material for the trail renderer

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
        float speed = Random.Range(minSpeed, maxSpeed); // Select random speed within the range
        rb.velocity = direction * speed;

        Debug.Log("Asteroid " + gameObject.name + " initialized towards the center of " + targetPlanet.name + " with speed " + speed);

        // Add and configure the trail renderer for the asteroid
        TrailRenderer trail = gameObject.AddComponent<TrailRenderer>();
        ConfigureTrailRenderer(trail);

        // Optional: Add a light to enhance the trail effect
        Light trailLight = gameObject.AddComponent<Light>();
        trailLight.type = LightType.Point;
        trailLight.color = Color.yellow;
        trailLight.range = 5f;
        trailLight.intensity = 2f;
        trailLight.shadows = LightShadows.None; // Disable shadows for the light
    }

    private void ConfigureTrailRenderer(TrailRenderer trail)
    {
        // Set trail time and width
        trail.time = trailTime; // Duration of the trail
        trail.startWidth = trailStartWidth; // Width at the start of the trail
        trail.endWidth = trailEndWidth; // Width at the end of the trail

        // Set color gradient
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(Color.yellow, 0.0f),    // Start with yellow
                new GradientColorKey(Color.red, 0.5f),       // Transition to red
                new GradientColorKey(new Color(1f, 0f, 0f, 0f), 1.0f) // End transparent
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0.0f),    // Fully opaque at the start
                new GradientAlphaKey(1f, 0.3f),    // Opaque at mid-point
                new GradientAlphaKey(0f, 1.0f)     // Fully transparent at the end
            }
        );
        trail.colorGradient = gradient;

        // Assign or create trail material
        if (trailMaterial != null)
        {
            // Use the provided material
            trail.material = trailMaterial;
        }
        else
        {
            // Create a default material with standard properties
            trail.material = new Material(Shader.Find("Particles/Standard Unlit"));
            trail.material.SetColor("_TintColor", Color.white); // White base color for the tint
        }

        // Configure trail properties
        trail.minVertexDistance = 0.1f; // Distance between vertices for smoother trails
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // Disable shadows for trail
        trail.autodestruct = true; // Automatically destroy the trail after the asteroid is destroyed
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the asteroid has already collided
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
