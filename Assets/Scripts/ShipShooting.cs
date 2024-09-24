using UnityEngine;

public class ShipShooting : MonoBehaviour
{
    // Reference to the projectile prefab
    public GameObject projectilePrefab;

    // Two spawn points for shooting projectiles
    public Transform spawnPoint1;
    public Transform spawnPoint2;

    // Fire rate and time since last shot
    public float fireRate = 0.5f; // Time in seconds between shots
    private float lastShotTime;

    // Reference to the ship's Rigidbody
    private Rigidbody rb;

    void Start()
    {
        // Get the Rigidbody component attached to the ship
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check if the ship is not moving
        if (IsShipStationary() && Time.time > lastShotTime + fireRate)
        {
            // Check if the fire button is held down
            if (Input.GetButton("Fire1"))
            {
                // Shoot projectiles
                ShootProjectiles();

                // Update last shot time
                lastShotTime = Time.time;
            }
        }
    }

    bool IsShipStationary()
    {
        // Check if the ship's velocity is approximately zero
        return rb.velocity.magnitude < 0.1f; // Adjust threshold as needed
    }

    void ShootProjectiles()
    {
        // Check if projectile prefab and spawn points are assigned
        if (projectilePrefab != null && spawnPoint1 != null && spawnPoint2 != null)
        {
            // Instantiate the projectile at the first spawn point
            Instantiate(projectilePrefab, spawnPoint1.position, spawnPoint1.rotation);

            // Instantiate the projectile at the second spawn point
            Instantiate(projectilePrefab, spawnPoint2.position, spawnPoint2.rotation);

            // Optional: Play shooting sound or animation here
            // e.g., PlayShootingSound();
            // e.g., PlayShootingAnimation();
        }
        else
        {
            Debug.LogWarning("Projectile prefab or spawn points not assigned.");
        }
    }
}
