using UnityEngine;

public class AsteroidGenerator : MonoBehaviour
{
    public GameObject asteroidPrefab;          // Prefab for the asteroid
    public GameObject fragmentPrefab;          // Prefab for the fragments
    public float minDistanceFromSun = 30.0f;   // Minimum distance from the Sun to spawn asteroids
    public float maxDistanceFromSun = 50.0f;   // Maximum distance from the Sun to spawn asteroids
    public float minSpeed = 20.0f;              // Minimum speed of the asteroid
    public float maxSpeed = 50.0f;              // Maximum speed of the asteroid
    public float spawnRate = 5.0f;              // Time interval between asteroid spawns

    private SolarSystemGenerator solarSystemGenerator;
    private float nextSpawnTime;

    void Start()
    {
        // Find the SolarSystemGenerator in the scene
        solarSystemGenerator = FindObjectOfType<SolarSystemGenerator>();

        if (solarSystemGenerator == null)
        {
            Debug.LogError("SolarSystemGenerator not found in the scene.");
            return;
        }

        nextSpawnTime = Time.time;
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnAsteroid();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnAsteroid()
    {
        if (asteroidPrefab == null || solarSystemGenerator.Planets.Count == 0)
        {
            Debug.LogWarning("Asteroid Prefab or Planets not set.");
            return;
        }

        // Randomly select a planet to target
        GameObject targetPlanet = solarSystemGenerator.Planets[Random.Range(0, solarSystemGenerator.Planets.Count)];

        // Random position outside the solar system
        Vector3 randomDirection = Random.onUnitSphere;
        float randomDistance = Random.Range(maxDistanceFromSun, maxDistanceFromSun * 2.0f);
        Vector3 spawnPosition = randomDirection * randomDistance;

        // Instantiate asteroid
        GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);

        // Set random scale
        float asteroidScale = Random.Range(0.1f, 0.1f); // Example scale range
        asteroid.transform.localScale = new Vector3(asteroidScale, asteroidScale, asteroidScale);

        // Set mass based on the scale
        Rigidbody rb = asteroid.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = Mathf.Pow(asteroidScale, 3) * 30f; // Adjust 30f based on desired mass scaling
            Vector3 directionToPlanet = (targetPlanet.transform.position - spawnPosition).normalized;
            float speed = Random.Range(minSpeed, maxSpeed);
            rb.velocity = directionToPlanet * speed;
        }
        
        // Add collision handling to the asteroid
        Asteroid asteroidScript = asteroid.AddComponent<Asteroid>();
        asteroidScript.fragmentPrefab = fragmentPrefab;
    }
}
