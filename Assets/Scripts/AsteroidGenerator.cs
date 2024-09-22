using System.Collections;
using UnityEngine;

public class AsteroidGenerator : MonoBehaviour
{
    public GameObject asteroidPrefab; // Prefab of the asteroid to spawn
    public float spawnInterval = 2f; // Time interval between spawns
    public string planetTag = "Planet"; // Tag to identify target planets
    public float spawnRadius = 10000f; // Radius within which to spawn asteroids

    private Transform[] targetPlanets;

    private void Start()
    {
        // Start the coroutine to wait for planets and spawn asteroids
        StartCoroutine(WaitForPlanetsAndSpawnAsteroids());
    }

    private IEnumerator WaitForPlanetsAndSpawnAsteroids()
    {
        // Continuously search for planets until at least one is found
        while (true)
        {
            // Find planets by tag and assign to targetPlanets array
            GameObject[] planetObjects = GameObject.FindGameObjectsWithTag(planetTag);
            if (planetObjects.Length > 0)
            {
                targetPlanets = new Transform[planetObjects.Length];
                for (int i = 0; i < planetObjects.Length; i++)
                {
                    targetPlanets[i] = planetObjects[i].transform;
                }
                break; // Exit the loop if planets are found
            }
            else
            {
                Debug.Log("Waiting for planets to spawn...");
            }
            yield return null; // Wait for the next frame
        }

        // Wait for 1 second before starting to spawn asteroids
        yield return new WaitForSeconds(1f);

        // Now all planets are available, start spawning asteroids
        StartCoroutine(SpawnAsteroid());
    }

    private IEnumerator SpawnAsteroid()
    {
        while (true)
        {
            // Randomly select a target planet
            Transform randomPlanet = targetPlanets[Random.Range(0, targetPlanets.Length)];

            // Generate a random spawn position within the defined radius
            Vector3 spawnPosition = Random.insideUnitSphere * spawnRadius + transform.position;

            GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Random.rotation);
            asteroid.GetComponent<Asteroid>().targetPlanet = randomPlanet; // Assign the randomly selected target planet

            yield return new WaitForSeconds(spawnInterval); // Wait for the specified interval before spawning the next asteroid
        }
    }
}
