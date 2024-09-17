using UnityEngine;

public class SatelliteController : MonoBehaviour
{
    public Transform targetPlanet; // The planet that the satellite orbits
    public float orbitDistance = 10f; // The distance the satellite will maintain from the planet
    public float orbitSpeed = 30f; // The speed at which the satellite orbits the planet
    public int orbitSegments = 100; // Number of segments for the orbital line

    public LineRenderer lineRenderer; // Public reference to the LineRenderer component

    private void Start()
    {
        // If targetPlanet is not assigned in the Inspector, try to find a planet with a specific tag
        if (targetPlanet == null)
        {
            GameObject planetObject = GameObject.FindWithTag("Planet");
            if (planetObject != null)
            {
                targetPlanet = planetObject.transform;
            }
            else
            {
                Debug.LogError("No object with tag 'Planet' found! Please assign the target planet manually or ensure an object with the 'Planet' tag exists.");
                return; // Exit if no planet found
            }
        }

        // Check if LineRenderer is assigned in the inspector, otherwise add it dynamically
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Set basic properties of the LineRenderer for the orbit path
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2; // Two points: satellite and planet

        // Set the material and color of the LineRenderer
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.cyan;
        lineRenderer.endColor = Color.cyan;

        // Set the satellite's initial orbit position
        InitializeSatellite(targetPlanet, orbitDistance, orbitSpeed);
    }

    private void Update()
    {
        // Continuously move the satellite in an orbit
        OrbitPlanet();

        // Update the LineRenderer to draw a line from the satellite to the planet
        DrawLineToPlanet();
    }

    public void InitializeSatellite(Transform target, float distance, float speed)
    {
        // Assign parameters for satellite behavior
        targetPlanet = target;
        orbitDistance = distance;
        orbitSpeed = speed;

        // Set initial position
        transform.position = targetPlanet.position + Vector3.forward * orbitDistance;
    }

    private void OrbitPlanet()
    {
        if (targetPlanet == null)
            return;

        // Rotate the satellite around the planet using the Y-axis
        transform.RotateAround(targetPlanet.position, Vector3.up, orbitSpeed * Time.deltaTime);

        // Maintain the correct orbit distance from the planet
        Vector3 directionFromPlanet = (transform.position - targetPlanet.position).normalized;
        transform.position = targetPlanet.position + directionFromPlanet * orbitDistance;
    }

    private void DrawLineToPlanet()
    {
        if (lineRenderer == null || targetPlanet == null)
            return;

        // Update the LineRenderer to point at the planet
        lineRenderer.SetPosition(0, transform.position);       // Start point (satellite's position)
        lineRenderer.SetPosition(1, targetPlanet.position);    // End point (planet's position)
    }
}
