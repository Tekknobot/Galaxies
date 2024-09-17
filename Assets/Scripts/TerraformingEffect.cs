using UnityEngine;

public class TerraformingEffect : MonoBehaviour
{
    public LineRenderer lineRenderer; // Reference to the LineRenderer component
    public float lineWidth = 0.2f;    // Width of the line
    public Color lineColor = Color.green; // Color of the line

    private Transform targetPlanet;    // The planet the player is orbiting
    private bool isOrbiting = false;   // Flag to check if the player is orbiting

    private void Start()
    {
        // Ensure the LineRenderer is initially disabled
        lineRenderer.enabled = false;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
    }

    private void Update()
    {
        // Update the LineRenderer if the player is orbiting
        if (isOrbiting && targetPlanet != null)
        {
            // Ensure the LineRenderer is enabled
            lineRenderer.enabled = true;

            // Update the LineRenderer positions
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, targetPlanet.position);
        }
        else
        {
            // Disable the LineRenderer if not orbiting
            lineRenderer.enabled = false;
        }
    }

    public void StartOrbiting(Transform planet)
    {
        targetPlanet = planet;
        isOrbiting = true;
    }

    public void StopTerraformingEffect()
    {
        // Disable the LineRenderer
        lineRenderer.enabled = false;
        isOrbiting = false;
    }
}
