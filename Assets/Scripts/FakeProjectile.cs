using UnityEngine;

public class FakeProjectile : MonoBehaviour
{
    // Lifetime of the projectile
    public float lifetime = 5f; // Time in seconds before the projectile is destroyed

    private void Start()
    {
        // Schedule destruction of the projectile after its lifetime
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the projectile collides with an object that is NOT the player
        if (!collision.gameObject.CompareTag("Player"))
        {
            // Handle collision with other objects
            // Example: Destroy the projectile upon hitting something else
            Destroy(gameObject);

            // Optional: Play impact effect, apply damage, etc.
            Debug.Log("Hit " + collision.gameObject.name);
        }
    }
}
