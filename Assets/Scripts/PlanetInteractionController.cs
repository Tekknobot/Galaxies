using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class PlanetInteractionController : MonoBehaviour
{
    public float interactionDistance = 1000f;
    public float lineWidth = 0.1f;
    public Material lineMaterial;
    public float curveHeight = 5f;

    private Transform closestPlanet;
    private LineRenderer currentLineRenderer;
    public TMP_Text planetNameText; // Reference to the TMP Text component for the name
    public TMP_Text planetBioText; // Reference to the TMP Text component for the bio

    private Color[] neonColors = new Color[]
    {
        Color.red, Color.green, new Color(1f, 0f, 1f), new Color(1f, 0.647f, 0f),
        new Color(1f, 0.75f, 0.8f), Color.cyan, Color.yellow, new Color(0.5f, 0f, 1f),
        new Color(0.75f, 1f, 0f), new Color(1f, 0.5f, 0.31f), new Color(0.2f, 1f, 0.8f),
        new Color(0f, 1f, 0.5f), new Color(1f, 0.36f, 0.36f), new Color(0f, 0.78f, 1f),
        new Color(0.94f, 0f, 0.54f), new Color(1f, 1f, 0.2f), new Color(0.4f, 0.8f, 1f),
        new Color(0.9f, 0.1f, 0.2f)
    };

    void Update()
    {
        FindClosestPlanet();

        if (closestPlanet != null)
        {
            float distanceToPlanet = Vector3.Distance(transform.position, closestPlanet.position);
            if (distanceToPlanet < interactionDistance)
            {
                ShowPlanetName(closestPlanet);
                ShowPlanetBio(closestPlanet);

                if (Gamepad.current != null && Gamepad.current.aButton.wasPressedThisFrame)
                {
                    InteractWithPlanet(closestPlanet);
                }
            }
            else
            {
                HidePlanetName();
                HidePlanetBio();
            }
        }
        else
        {
            HidePlanetName();
            HidePlanetBio();
        }

        if (currentLineRenderer != null && currentLineRenderer.enabled)
        {
            UpdateLineColor(currentLineRenderer);
        }
    }

    private void ShowPlanetName(Transform planet)
    {
        if (planetNameText != null)
        {
            planetNameText.text = planet.GetComponent<Planet>().name; // Assuming you have a Planet component with a name
            planetNameText.gameObject.SetActive(true);
        }
    }

    private void HidePlanetName()
    {
        if (planetNameText != null)
        {
            planetNameText.gameObject.SetActive(false);
        }
    }

    private void ShowPlanetBio(Transform planet)
    {
        if (planetBioText != null)
        {
            Planet planetComponent = planet.GetComponent<Planet>();
            planetBioText.text = planetComponent != null ? planetComponent.GetResource() : "No Bio Available";
            planetBioText.gameObject.SetActive(true);
        }
    }

    private void HidePlanetBio()
    {
        if (planetBioText != null)
        {
            planetBioText.gameObject.SetActive(false);
        }
    }

    private void FindClosestPlanet()
    {
        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");
        if (planets.Length == 0)
        {
            closestPlanet = null;
            return;
        }

        GameObject nearestPlanet = planets[0];
        float closestDistance = Vector3.Distance(transform.position, nearestPlanet.transform.position);

        foreach (GameObject planet in planets)
        {
            float distance = Vector3.Distance(transform.position, planet.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestPlanet = planet;
            }
        }

        closestPlanet = nearestPlanet.transform;
    }

    private void InteractWithPlanet(Transform planet)
    {
        Color neonColor = neonColors[Random.Range(0, neonColors.Length)];
        StartCoroutine(ChangePlanetColor(planet, neonColor));
        ConnectionManager.Instance.RegisterChangedPlanet(planet);

        Transform nearestPlanet = FindNearestPlanet(planet);
        if (nearestPlanet != null)
        {
            StartCoroutine(ChangePlanetColor(nearestPlanet, neonColor));
            ConnectionManager.Instance.RegisterChangedPlanet(nearestPlanet);
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
            if (p.transform != planet)
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

        int segments = 20;
        Vector3[] points = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 controlPoint = (start + end) / 2 + Vector3.up * curveHeight;
            points[i] = Vector3.Lerp(Vector3.Lerp(start, controlPoint, t), Vector3.Lerp(controlPoint, end, t), t);
        }

        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
        lineRenderer.enabled = true;

        return lineRenderer;
    }

    private void UpdateLineColor(LineRenderer lineRenderer)
    {
        Color originalColor = lineRenderer.startColor;
        float pulse = Mathf.PingPong(Time.time, 1f);
        lineRenderer.startColor = new Color(originalColor.r, originalColor.g, originalColor.b, pulse);
        lineRenderer.endColor = lineRenderer.startColor;
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
            planetRenderer.material.color = targetColor;
        }
    }
}
