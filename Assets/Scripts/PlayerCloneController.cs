using UnityEngine;

public class PlayerCloneController : MonoBehaviour
{
    public float orbitDistance = 15f; // Distance from the planet while orbiting
    public float orbitSpeed = 20f; // Speed at which the player orbits the planet
    public TerraformingEffect terraformingEffect; // Reference to the TerraformingEffect script

    private Transform targetPlanet; // The planet the clone will orbit
    private bool isOrbiting = false; // Flag to determine if the clone is orbiting

    private void Start()
    {
        // Automatically start orbiting if a target planet is set
        if (targetPlanet != null)
        {
            isOrbiting = true;
            // Start the terraforming effect if it exists
            if (terraformingEffect != null)
            {
                terraformingEffect.StartOrbiting(targetPlanet);
            }
        }
    }

    private void Update()
    {
        if (isOrbiting && targetPlanet != null)
        {
            OrbitPlanet();
        }
    }

    void OrbitPlanet()
    {
        if (targetPlanet == null)
            return;

        // Calculate diagonal axis for orbiting
        Vector3 diagonalAxis = (Vector3.up + Vector3.right).normalized; // Combining up and right for diagonal

        // Rotate the clone diagonally around the planet using the diagonal axis
        transform.RotateAround(targetPlanet.position, diagonalAxis, orbitSpeed * Time.deltaTime);

        // Ensure the clone maintains the correct orbit distance from the planet
        Vector3 directionFromPlanet = (transform.position - targetPlanet.position).normalized;
        transform.position = targetPlanet.position + directionFromPlanet * orbitDistance;
    }

    public void InitializeClone(Transform planet, float distance, float speed)
    {
        targetPlanet = planet;
        orbitDistance = distance;
        orbitSpeed = speed;
        isOrbiting = true;

        // Start the terraforming effect for the clone
        if (terraformingEffect != null)
        {
            terraformingEffect.StartOrbiting(targetPlanet);
        }
    }
}
