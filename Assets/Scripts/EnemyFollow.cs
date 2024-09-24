using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player; // Reference to the player transform
    public float moveSpeed = 3f; // Speed at which the enemy follows the player
    public float stoppingDistance = 2f; // Distance at which the enemy stops moving towards the player

    private void Update()
    {
        if (player != null)
        {
            // Calculate the distance to the player
            float distance = Vector3.Distance(transform.position, player.position);

            // Check if the enemy is further away than the stopping distance
            if (distance > stoppingDistance)
            {
                // Move towards the player's position
                Vector3 direction = (player.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;

                // Optional: Face the player
                transform.LookAt(player);
            }
        }
    }
}
