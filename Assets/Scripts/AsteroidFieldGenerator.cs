using UnityEngine;
using System.Collections.Generic;

public class AsteroidFieldGenerator : MonoBehaviour
{
    public int numberOfAsteroids = 100; // Number of asteroids to generate
    public float minSize = 0.1f; // Minimum size of asteroids
    public float maxSize = 1.0f; // Maximum size of asteroids
    public float fieldRadius = 50.0f; // Radius of the asteroid field
    public float solarSystemBoundary = 100.0f; // Boundary of the solar system
    public Material asteroidMaterial; // Material for the asteroids

    private List<GameObject> asteroids = new List<GameObject>(); // List to keep track of asteroids

    void Start()
    {
        GenerateAsteroidField();
    }

    void Update()
    {
        CheckAsteroidBoundaries();
    }

    void GenerateAsteroidField()
    {
        for (int i = 0; i < numberOfAsteroids; i++)
        {
            // Generate a random position within the asteroid field radius
            Vector3 position = Random.insideUnitSphere * fieldRadius;
            position.y = 0; // Keep asteroids on the same plane

            // Create an asteroid
            CreateAsteroid(position);
        }
    }

    void CreateAsteroid(Vector3 position)
    {
        float size = Random.Range(minSize, maxSize); // Random size for the asteroid
        GameObject asteroid = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        asteroid.transform.position = position;
        asteroid.transform.localScale = new Vector3(size, size, size);

        // Add a Rigidbody to the asteroid
        Rigidbody rb = asteroid.AddComponent<Rigidbody>();
        rb.useGravity = false; // No gravity for asteroids
        rb.mass = size; // Set mass based on size

        // Set the asteroid material
        Renderer renderer = asteroid.GetComponent<Renderer>();
        if (renderer != null && asteroidMaterial != null)
        {
            renderer.material = asteroidMaterial;
        }

        // Add asteroid to the list
        asteroids.Add(asteroid);
    }

    void CheckAsteroidBoundaries()
    {
        for (int i = asteroids.Count - 1; i >= 0; i--)
        {
            if (Vector3.Distance(Vector3.zero, asteroids[i].transform.position) > solarSystemBoundary)
            {
                Destroy(asteroids[i]);
                asteroids.RemoveAt(i);
            }
        }
    }
}
