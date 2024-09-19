using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlanetInteractionController : MonoBehaviour
{
    public float interactionDistance = 1000f; // Distance at which interaction is allowed
    public float fadeDuration = 1f; // Duration for color fade-in effect

    private Transform closestPlanet;
    private Color targetColor;

    private Color[] neonColors = new Color[]
    {
        Color.red,          // Red
        Color.green,        // Green
        new Color(1f, 0f, 1f), // Magenta
        new Color(1f, 0.647f, 0f), // Orange
        new Color(1f, 0.75f, 0.8f) // Pink
    };

    void Update()
    {
        // Find the closest planet
        FindClosestPlanet();

        if (closestPlanet != null)
        {
            // Check if the distance to the closest planet is within the interaction distance
            float distanceToPlanet = Vector3.Distance(transform.position, closestPlanet.position);
            if (distanceToPlanet < interactionDistance)
            {
                // Check for gamepad South button press to trigger interaction
                if (Gamepad.current != null && Gamepad.current.aButton.wasPressedThisFrame)
                {
                    StartCoroutine(InteractWithPlanet(closestPlanet));
                }
            }
        }
    }

    private void FindClosestPlanet()
    {
        // Find all objects with the "Planet" tag
        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");
        if (planets.Length == 0)
        {
            closestPlanet = null;
            return;
        }

        // Initialize the closest planet
        GameObject nearestPlanet = planets[0];
        float closestDistance = Vector3.Distance(transform.position, nearestPlanet.transform.position);

        // Iterate through all planets to find the closest one
        foreach (GameObject planet in planets)
        {
            float distance = Vector3.Distance(transform.position, planet.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestPlanet = planet;
            }
        }

        // Set the closest planet
        closestPlanet = nearestPlanet.transform;
    }

    private IEnumerator InteractWithPlanet(Transform planet)
    {
        Renderer planetRenderer = planet.GetComponent<Renderer>();
        if (planetRenderer != null)
        {
            // Choose a random neon color from the predefined list
            Color neonColor = neonColors[Random.Range(0, neonColors.Length)];
            targetColor = neonColor;

            // Fade in effect
            Color initialColor = planetRenderer.material.color;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                planetRenderer.material.color = Color.Lerp(initialColor, targetColor, elapsedTime / fadeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            planetRenderer.material.color = targetColor; // Ensure final color is set
        }
    }
}
