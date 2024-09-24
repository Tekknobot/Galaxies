using UnityEngine;

public class ShipShooting : MonoBehaviour
{
    // Reference to the projectile prefab
    public GameObject projectilePrefab;

    // Reference to the impact effect prefab
    public GameObject impactEffectPrefab;

    // Two spawn points for shooting projectiles
    public Transform spawnPoint1;
    public Transform spawnPoint2;

    // Fire rate and time since last shot
    public float fireRate = 0.5f; // Time in seconds between shots
    private float lastShotTime;

    // Force applied to the projectile when launched
    public float projectileForce = 500f; // Adjust this value as needed

    // Raycast range (distance the ray can travel)
    public float raycastRange = 100f;

    // Reference to the ship's Rigidbody
    private Rigidbody rb;

    void Start()
    {
        // Get the Rigidbody component attached to the ship
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check if enough time has passed since the last shot
        if (Time.time > lastShotTime + fireRate)
        {
            // Check if the fire button is held down
            if (Input.GetButton("Fire1"))
            {
                // Perform shooting
                ShootRaycastsAndProjectiles();

                // Update last shot time
                lastShotTime = Time.time;
            }
        }
    }

    void ShootRaycastsAndProjectiles()
    {
        // Check if spawn points and projectile prefab are assigned
        if (spawnPoint1 != null && spawnPoint2 != null && projectilePrefab != null)
        {
            // Perform raycasting from both spawn points
            RaycastFromSpawnPoint(spawnPoint1);
            RaycastFromSpawnPoint(spawnPoint2);

            // Instantiate projectiles from both spawn points
            LaunchProjectile(spawnPoint1);
            LaunchProjectile(spawnPoint2);
        }
        else
        {
            Debug.LogWarning("Spawn points or projectile prefab not assigned.");
        }
    }

    void LaunchProjectile(Transform spawnPoint)
    {
        // Instantiate the projectile at the spawn point's position and rotation
        GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);

        // Get the Rigidbody component of the projectile
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // Check if the Rigidbody is available
        if (projectileRb != null)
        {
            // Apply force to the projectile in the forward direction
            projectileRb.AddForce(spawnPoint.forward * projectileForce);
        }
        else
        {
            Debug.LogWarning("Projectile prefab does not have a Rigidbody component.");
        }
    }

    void RaycastFromSpawnPoint(Transform spawnPoint)
    {
        // Perform the raycast from the spawn point forward
        RaycastHit hit;
        if (Physics.Raycast(spawnPoint.position, spawnPoint.forward, out hit, raycastRange))
        {
            Debug.Log("Hit " + hit.collider.name + " at " + hit.point);

            // Instantiate the impact effect at the hit point
            if (impactEffectPrefab != null)
            {
                Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            }

            // Optional: Apply damage or other effects to the hit object
            // Example: hit.collider.GetComponent<Health>()?.TakeDamage(damageAmount);
        }
        else
        {
            Debug.Log("Missed, no hit detected.");
        }
    }
}
