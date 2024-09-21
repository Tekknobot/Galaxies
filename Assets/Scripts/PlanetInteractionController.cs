using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlanetInteractionController : MonoBehaviour
{
    public float interactionDistance = 1000f; // Distance at which interaction is allowed
    public float lineWidth = 0.1f; // Width of the LineRenderer
    public Material lineMaterial; // Material for the LineRenderer
    public float curveHeight = 5f; // Height of the curve

    private Transform closestPlanet;
    private LineRenderer currentLineRenderer;

    // Extended list of neon colors for vibrant interactions
    private Color[] neonColors = new Color[]
    {
        Color.red,
        Color.green,
        new Color(1f, 0f, 1f),
        new Color(1f, 0.647f, 0f),
        new Color(1f, 0.75f, 0.8f),
        Color.cyan,
        Color.yellow,
        new Color(0.5f, 0f, 1f),
        new Color(0.75f, 1f, 0f),
        new Color(1f, 0.5f, 0.31f),
        new Color(0.2f, 1f, 0.8f),
        new Color(0f, 1f, 0.5f),
        new Color(1f, 0.36f, 0.36f),
        new Color(0f, 0.78f, 1f),
        new Color(0.94f, 0f, 0.54f),
        new Color(1f, 1f, 0.2f),
        new Color(0.4f, 0.8f, 1f),
        new Color(0.9f, 0.1f, 0.2f)
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
                    InteractWithPlanet(closestPlanet);
                }
            }
        }

        // Update line color if it's active
        if (currentLineRenderer != null && currentLineRenderer.enabled)
        {
            UpdateLineColor(currentLineRenderer);
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

    private void InteractWithPlanet(Transform planet)
    {
        // Choose a random neon color from the predefined list for the planet
        Color neonColor = neonColors[Random.Range(0, neonColors.Length)];

        // Change the planet color
        StartCoroutine(ChangePlanetColor(planet, neonColor));

        // Create a line renderer to connect to the nearest planet
        Transform nearestPlanet = FindNearestPlanet(planet);
        if (nearestPlanet != null)
        {
            currentLineRenderer = CreateCurvedLine(planet.position, nearestPlanet.position, neonColor);
        }
    }

    private Transform FindNearestPlanet(Transform planet)
    {
        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");
        Transform nearest = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject p in planets)
        {
            if (p.transform != planet) // Don't consider the planet itself
            {
                float distance = Vector3.Distance(planet.position, p.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearest = p.transform;
                }
            }
        }

        return nearest;
    }

    private LineRenderer CreateCurvedLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject lineObject = new GameObject("CurvedLine");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        int segments = 20; // Number of segments for the curve
        Vector3[] points = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            // Quadratic Bezier curve calculation with upward control point
            Vector3 controlPoint = (start + end) / 2 + Vector3.up * curveHeight;
            points[i] = Vector3.Lerp(Vector3.Lerp(start, controlPoint, t), Vector3.Lerp(controlPoint, end, t), t);
        }

        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
        lineRenderer.enabled = true; // Enable the line renderer

        return lineRenderer;
    }

    private void UpdateLineColor(LineRenderer lineRenderer)
    {
        // Pulse the line color
        Color originalColor = lineRenderer.startColor;
        float pulse = Mathf.PingPong(Time.time, 1f);
        Color pulsedColor = new Color(originalColor.r, originalColor.g, originalColor.b, pulse);
        lineRenderer.startColor = pulsedColor;
        lineRenderer.endColor = pulsedColor;
    }

    private IEnumerator ChangePlanetColor(Transform planet, Color targetColor)
    {
        Renderer planetRenderer = planet.GetComponent<Renderer>();
        if (planetRenderer != null)
        {
            Color initialColor = planetRenderer.material.color;
            float elapsedTime = 0f;
            float fadeDuration = 1f;

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
