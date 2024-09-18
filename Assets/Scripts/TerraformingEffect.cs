using UnityEngine;
using System.Collections;

public class TerraformingEffect : MonoBehaviour
{
    public LineRenderer lineRenderer;       // Reference to the LineRenderer component
    public float lineWidth = 0.2f;          // Width of the line
    public Color lineColor = Color.green;   // Color of the line
    public float pulseSpeed = 2f;           // Speed at which the line pulsates
    public float pulseAmount = 0.1f;        // Maximum amount of pulse change
    public GameObject planetPrefab;         // Prefab for creating the new sphere during terraforming
    public Material newMaterial;            // The new material to apply to the target planet
    public Material newSphereMaterial;      // The material for the new sphere
    public float growthDuration = 5f;       // Duration for the new sphere to grow to planet size
    public float shrinkDuration = 2f;       // Duration for the new sphere to shrink into nothing
    public float maxSphereAge = 25f;        // Maximum time before the sphere starts shrinking

    private Transform targetPlanet;         // The planet being terraformed
    private bool isOrbiting = false;        // Flag to check if the player is orbiting
    private bool isTerraforming = false;    // Flag to check if terraforming is in progress
    private float originalWidth;            // Original width of the LineRenderer
    private GameObject newSphere;           // The new sphere for terraforming effect
    private float terraformingStartTime;    // The time terraforming started
    private float sphereCreationTime;       // The time the new sphere was created
    private Vector3 targetPlanetScale;      // The original scale of the planet being terraformed
    private Coroutine shrinkCoroutine;      // Reference to the shrinking coroutine

    private void Start()
    {
        // Initialize the LineRenderer
        lineRenderer.enabled = false;
        originalWidth = lineWidth;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
    }

    private void Update()
    {
        // Handle LineRenderer pulsating effect when orbiting
        if (isOrbiting && targetPlanet != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, targetPlanet.position);

            float pulse = Mathf.PingPong(Time.time * pulseSpeed, pulseAmount);
            lineRenderer.startWidth = originalWidth + pulse;
            lineRenderer.endWidth = originalWidth + pulse;
        }
        else
        {
            lineRenderer.enabled = false;
        }

        // Handle sphere growth during terraforming
        if (isTerraforming && newSphere != null)
        {
            // Follow the planet's orbit
            newSphere.transform.position = targetPlanet.position;

            float elapsedTime = Mathf.Clamp01((Time.time - terraformingStartTime) / growthDuration);
            newSphere.transform.localScale = Vector3.Lerp(Vector3.zero, targetPlanetScale * 1.0f, elapsedTime); // Grow 10% larger

            if (elapsedTime >= 1f)
            {
                // Start shrinking the new sphere immediately
                shrinkCoroutine = StartCoroutine(ShrinkAndDestroySphere());

                // Delay material replacement by 20 seconds
                StartCoroutine(DelayedMaterialReplacement());

                isTerraforming = false;
            }
        }

        // Check if the new sphere has been around for longer than the max age
        if (newSphere != null && Time.time - sphereCreationTime > maxSphereAge)
        {
            // Start shrinking and destroy the sphere if it has exceeded maxSphereAge
            if (isTerraforming)
            {
                StartCoroutine(ShrinkAndDestroySphere());
                isTerraforming = false; // Stop terraforming
            }
        }
    }

    // Coroutine to delay the material replacement
    private IEnumerator DelayedMaterialReplacement()
    {
        // Wait for 20 seconds
        yield return new WaitForSeconds(20f);

        // Apply the new material to the planet after the delay
        if (newMaterial != null)
        {
            Renderer planetRenderer = targetPlanet.GetComponent<Renderer>();
            if (planetRenderer != null)
            {
                planetRenderer.material = newMaterial;
            }
        }
    }

    public void StartOrbiting(Transform planet)
    {
        targetPlanet = planet;
        isOrbiting = true;

        StartTerraforming(targetPlanet);
    }

    public void StartTerraforming(Transform planet)
    {
        targetPlanet = planet;
        isTerraforming = true;
        terraformingStartTime = Time.time;
        targetPlanetScale = targetPlanet.localScale;  // Save the planet's original scale

        // Instantiate the new sphere at 0 scale (invisible) and grow it over time
        if (planetPrefab != null)
        {
            newSphere = Instantiate(planetPrefab, targetPlanet.position, targetPlanet.rotation);
            newSphere.transform.localScale = Vector3.zero;  // Start at 0 scale

            // Make the new sphere a child of the target planet so it follows the target's orbit
            newSphere.transform.SetParent(targetPlanet);

            // Apply the new sphere material
            if (newSphereMaterial != null)
            {
                Renderer newSphereRenderer = newSphere.GetComponent<Renderer>();
                if (newSphereRenderer != null)
                {
                    newSphereRenderer.material = newSphereMaterial;
                }
            }

            // Record the time the sphere was created
            sphereCreationTime = Time.time;
        }

        // Start material replacement and then begin shrinking the sphere
        StartCoroutine(MaterialReplacementAndShrink());
    }

    private IEnumerator MaterialReplacementAndShrink()
    {
        // Wait for the material replacement to complete
        yield return DelayedMaterialReplacement();

        // Begin shrinking the new sphere
        shrinkCoroutine = StartCoroutine(ShrinkAndDestroySphere());
    }

    private IEnumerator ShrinkAndDestroySphere()
    {
        if (newSphere == null)
            yield break;

        float elapsedTime = 0f;
        Vector3 initialScale = newSphere.transform.localScale;
        Vector3 targetScale = Vector3.zero;

        // Shrink the new sphere until it reaches % of its initial scale
        while (newSphere != null && newSphere.transform.localScale.magnitude > initialScale.magnitude * 0.99f)
        {
            float t = elapsedTime / shrinkDuration;
            newSphere.transform.localScale = Vector3.Lerp(initialScale, initialScale * 0.99f, t);
            elapsedTime += Time.deltaTime;
            yield return null;  // Wait for the next frame
        }

        // Wait for a moment at % of the scale
        yield return new WaitForSeconds(0.5f);  // Adjust delay if necessary

        // Continue shrinking to zero
        elapsedTime = 0f;
        while (newSphere != null && elapsedTime < shrinkDuration)
        {
            float t = elapsedTime / shrinkDuration;
            newSphere.transform.localScale = Vector3.Lerp(initialScale * 0.99f, targetScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;  // Wait for the next frame
        }

        // Check again before destroying to prevent accessing destroyed objects
        if (newSphere != null)
        {
            // Ensure the sphere is fully shrunk
            newSphere.transform.localScale = Vector3.zero;

            // Destroy the sphere
            Destroy(newSphere);
        }
    }

    public void StopTerraformingEffect()
    {
        isOrbiting = false;
        lineRenderer.enabled = false;

        // Stop terraforming and destroy the new sphere if it exists
        if (isTerraforming)
        {
            isTerraforming = false;

            // Stop the shrink coroutine if it's running
            if (shrinkCoroutine != null)
            {
                StopCoroutine(shrinkCoroutine);
            }

            // Destroy the new sphere if it exists
            if (newSphere != null)
            {
                Destroy(newSphere);
            }
        }
    }
}
