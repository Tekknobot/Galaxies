using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance { get; private set; }

    public HashSet<Transform> changedPlanets = new HashSet<Transform>(); // Made public
    public int totalPlanets; // Made public

    public float minWidth = 10f; // Minimum width
    public float maxWidth = 100f; // Maximum width
    public float pulseSpeed = 1f; // Speed of the pulsing effect

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        UpdateTotalPlanets();
    }

    private void Update()
    {
        UpdateTotalPlanets(); // Continually check for total planets
    }

    private void UpdateTotalPlanets()
    {
        int currentPlanetCount = GameObject.FindGameObjectsWithTag("Planet").Length;
        if (currentPlanetCount != totalPlanets)
        {
            totalPlanets = currentPlanetCount;
            Debug.Log($"Total Planets Updated: {totalPlanets}");
        }

        if (changedPlanets.Count == totalPlanets && totalPlanets > 0)
        {
            CreatePulsingLine();
        }
    }

    public void RegisterChangedPlanet(Transform planet)
    {
        if (changedPlanets.Add(planet))
        {
            Debug.Log($"Changed Planet Color: {planet.name}. Total Changed: {changedPlanets.Count}");
        }
    }

    public int GetChangedPlanetsCount()
    {
        return changedPlanets.Count;
    }

    public void ResetPlanets()
    {
        changedPlanets.Clear();
        totalPlanets = 0; // Reset total planets
        Debug.Log("Planets reset on restart.");
    }

    private void CreatePulsingLine()
    {
        GameObject lineObject = new GameObject("PulsingLine");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        lineRenderer.startWidth = minWidth;
        lineRenderer.endWidth = maxWidth;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.up * 100000);
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = GetRandomNeonColor();
        lineRenderer.endColor = lineRenderer.startColor;

        StartCoroutine(PulseLineColor(lineRenderer));
    }

    private Color GetRandomNeonColor()
    {
        Color[] neonColors = new Color[]
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
        return neonColors[Random.Range(0, neonColors.Length)];
    }

    private IEnumerator PulseLineColor(LineRenderer lineRenderer)
    {
        while (true)
        {
            Color originalColor = lineRenderer.startColor;
            float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            Color pulsedColor = new Color(originalColor.r, originalColor.g, originalColor.b, pulse);
            lineRenderer.startColor = pulsedColor;
            lineRenderer.endColor = pulsedColor;

            float pulsedWidth = Mathf.Lerp(minWidth, maxWidth, pulse);
            lineRenderer.startWidth = pulsedWidth;
            lineRenderer.endWidth = pulsedWidth;

            yield return null; // Wait for the next frame
        }
    }
}
