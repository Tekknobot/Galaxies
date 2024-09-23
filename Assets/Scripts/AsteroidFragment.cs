using UnityEngine;

public class AsteroidFragment : MonoBehaviour
{
    public float minLifetime = 2f; // Minimum lifetime of the fragment
    public float maxLifetime = 5f; // Maximum lifetime of the fragment

    private void Start()
    {
        // Assign a random lifetime between minLifetime and maxLifetime
        float randomLifetime = Random.Range(minLifetime, maxLifetime);

        // Destroy the fragment after the random lifetime
        Destroy(gameObject, randomLifetime);
    }
}
