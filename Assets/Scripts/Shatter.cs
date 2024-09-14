using UnityEngine;

public class Shatter : MonoBehaviour
{
    public GameObject fragmentPrefab; // Prefab for the fragments
    public int numberOfFragments = 1; // Number of fragments

    public void ShatterIntoFragments()
    {
        for (int i = 0; i < numberOfFragments; i++)
        {
            // Create a fragment at the sphere's position
            GameObject fragment = Instantiate(fragmentPrefab, transform.position, Random.rotation);

            // Set the fragment's scale randomly to make it look different
            fragment.transform.localScale = Vector3.one * Random.Range(0.1f, 0.1f);

            // Add a Rigidbody to the fragment
            Rigidbody rb = fragment.AddComponent<Rigidbody>();
            rb.AddExplosionForce(100f, transform.position, 5f);

            // Optionally, destroy the fragment after a certain time
            Destroy(fragment, 5f);
        }

        // Destroy the original planet
        Destroy(gameObject);
    }
}
