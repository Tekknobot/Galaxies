using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsteroidGenerator : MonoBehaviour
{
    public GameObject asteroidPrefab; // Prefab for the asteroid
    public float spawnRate = 2f; // Rate of asteroid spawning in seconds
    public float minAsteroidScale = 0.1f; // Minimum scale for asteroids
    public float maxAsteroidScale = 0.5f; // Maximum scale for asteroids
    public float minLaunchSpeed = 1f; // Minimum launch speed
    public float maxLaunchSpeed = 5f; // Maximum launch speed
    public float minMass = 0.5f; // Minimum mass for asteroids
    public float maxMass = 2.0f; // Maximum mass for asteroids
    public float spawnAreaRadius = 100f; // Radius within which asteroids can spawn
    public Material trailMaterial; // Material for the trail renderer
    public float fadeDuration = 1f; // Duration of the fade effect before destruction

    // Public thickness for the TrailRenderer
    public float trailStartWidth = 0.5f; // Starting width of the trail
    public float trailEndWidth = 0.3f; // Ending width of the trail

    private List<GameObject> planets = new List<GameObject>();

    void Start()
    {
        StartCoroutine(InitializeAndSpawnAsteroids());
    }

    IEnumerator InitializeAndSpawnAsteroids()
    {
        // Wait until all planets are found
        yield return new WaitUntil(() => FindAllPlanets());

        // Start spawning asteroids
        StartCoroutine(SpawnAsteroids());
    }

    bool FindAllPlanets()
    {
        planets.Clear();
        planets.AddRange(GameObject.FindGameObjectsWithTag("Planet"));
        return planets.Count > 0; // Check if planets are found
    }

    IEnumerator SpawnAsteroids()
    {
        while (true)
        {
            // Instantiate an asteroid
            SpawnAsteroid();
            yield return new WaitForSeconds(spawnRate); // Wait for the specified spawn rate
        }
    }

    void SpawnAsteroid()
    {
        // Select a random planet for potential targeting
        GameObject targetPlanet = planets[Random.Range(0, planets.Count)];
        Vector3 launchPosition = RandomLaunchPosition();

        // Create the asteroid
        GameObject asteroid = Instantiate(asteroidPrefab, launchPosition, Quaternion.identity);
        float scale = Random.Range(minAsteroidScale, maxAsteroidScale);
        asteroid.transform.localScale = new Vector3(scale, scale, scale);
        float mass = Random.Range(minMass, maxMass);
        Rigidbody rb = asteroid.AddComponent<Rigidbody>();
        rb.mass = mass;
        rb.useGravity = false; // Prevent gravitational influence from other bodies

        // Calculate launch direction towards the planet
        Vector3 launchDirection = (targetPlanet.transform.position - launchPosition).normalized;
        float launchSpeed = Random.Range(minLaunchSpeed, maxLaunchSpeed);
        rb.velocity = launchDirection * launchSpeed;

        // Add Trail Renderer if trail material is assigned
        if (trailMaterial != null)
        {
            TrailRenderer trailRenderer = asteroid.AddComponent<TrailRenderer>();
            trailRenderer.material = trailMaterial;
            trailRenderer.time = 2f; // Increase the time the trail lasts
            trailRenderer.startWidth = trailStartWidth; // Use public thickness for the start
            trailRenderer.endWidth = trailEndWidth; // Use public thickness for the end

            // Create a color gradient for the trail
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.red, 0.0f), // Start color
                    new GradientColorKey(Color.yellow, 0.5f), // Middle color
                    new GradientColorKey(Color.clear, 1.0f) // End color
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1.0f, 0.0f), // Fully opaque at the start
                    new GradientAlphaKey(1.0f, 0.5f), // Fully opaque in the middle
                    new GradientAlphaKey(0.0f, 1.0f) // Fully transparent at the end
                }
            );

            trailRenderer.colorGradient = gradient; // Assign the gradient to the trail
            trailRenderer.autodestruct = true; // Automatically destroy trail when asteroid is destroyed
        }

        // Add collision detection
        asteroid.AddComponent<AsteroidCollision>().fadeDuration = fadeDuration;
    }

    Vector3 RandomLaunchPosition()
    {
        float x = Random.Range(-spawnAreaRadius, spawnAreaRadius);
        float y = Random.Range(-spawnAreaRadius, spawnAreaRadius); // Height for launch position
        float z = Random.Range(-spawnAreaRadius, spawnAreaRadius);
        return new Vector3(x, y, z);
    }
}

// New script to handle asteroid collision and fading
public class AsteroidCollision : MonoBehaviour
{
    public float fadeDuration;

    private void OnCollisionEnter(Collision collision)
    {
        // Start the fade-out effect and destruction
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        Renderer renderer = GetComponent<Renderer>();
        Color startColor = renderer.material.color;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            Color newColor = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(1, 0, normalizedTime));
            renderer.material.color = newColor;
            yield return null;
        }

        // Finally, destroy the object
        Destroy(gameObject);
    }
}
