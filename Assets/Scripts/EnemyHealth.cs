using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f; // Maximum health points of the enemy
    private float currentHealth; // Current health points of the enemy
    public float flashDuration = 0.1f; // Duration of the flash effect
    public int flashCount = 5; // Number of times to flash
    private Renderer enemyRenderer; // Renderer of the enemy for flashing effect
    private Color originalColor; // Original color of the enemy
    private Coroutine flashCoroutine; // Reference to the flash coroutine

    private void Start()
    {
        currentHealth = maxHealth; // Initialize current health
        enemyRenderer = GetComponent<Renderer>();
        originalColor = enemyRenderer.material.color; // Store original color
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with an object that should damage the enemy
        if (collision.gameObject.CompareTag("Projectile")) // Change to your projectile tag
        {
            TakeDamage(10f); // Reduce health by 10 for each collision (adjust as needed)
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"Enemy damaged! Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die(); // Call the Die method if health reaches zero
        }
        else
        {
            if (flashCoroutine != null)
                StopCoroutine(flashCoroutine); // Stop any ongoing flash effect

            flashCoroutine = StartCoroutine(FlashEffect()); // Start flashing effect
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject); // Destroy the enemy object
    }

    private IEnumerator FlashEffect()
    {
        Color flashColor = Color.red; // Color to flash
        Color transparentFlashColor = new Color(flashColor.r, flashColor.g, flashColor.b, originalColor.a); // Keep original alpha

        for (int i = 0; i < flashCount; i++)
        {
            // Set the material to flash color with original alpha
            enemyRenderer.material.color = transparentFlashColor; // Change to flash color
            yield return new WaitForSeconds(flashDuration);
            enemyRenderer.material.color = originalColor; // Change back to original color
            yield return new WaitForSeconds(flashDuration);
        }
    }
}
