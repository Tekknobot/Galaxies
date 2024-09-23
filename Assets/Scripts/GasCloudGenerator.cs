using UnityEngine;

public class GasCloudParticleGenerator : MonoBehaviour
{
    public GameObject[] gasCloudParticlePrefabs; // Array of prefabs for the gas cloud particle systems
    public int maxCloudsPerPlanet = 3; // Maximum number of gas clouds per selected planet
    public float minDistanceFromPlanet = 2f; // Minimum distance from the planet
    public float maxDistanceFromPlanet = 10f; // Maximum distance from the planet
    public int numberOfSelectedPlanets = 3; // Number of planets to create gas clouds around

    void Start()
    {
        CreateGasClouds();
    }

    void CreateGasClouds()
    {
        // Find all planets in the scene
        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");

        // Randomly shuffle the array of planets
        planets.Shuffle();

        // Select a random number of planets to create gas clouds around
        int selectedPlanetsCount = Random.Range(1, numberOfSelectedPlanets);

        for (int i = 0; i < selectedPlanetsCount; i++)
        {
            GameObject planet = planets[i];

            // Randomly decide how many clouds to create around this planet
            int cloudsAroundPlanet = Random.Range(1, maxCloudsPerPlanet + 1);

            for (int j = 0; j < cloudsAroundPlanet; j++)
            {
                // Generate a random position around the planet
                Vector3 randomDirection = Random.onUnitSphere; // Random direction
                float randomDistance = Random.Range(minDistanceFromPlanet, maxDistanceFromPlanet);
                Vector3 position = planet.transform.position + randomDirection * randomDistance;

                // Randomly pick a prefab from the array
                GameObject chosenPrefab = gasCloudParticlePrefabs[Random.Range(0, gasCloudParticlePrefabs.Length)];

                // Instantiate the chosen prefab at the calculated position
                Instantiate(chosenPrefab, position, Quaternion.identity);
            }
        }
    }
}

// Extension method for shuffling arrays
public static class ArrayExtensions
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
    }
}
