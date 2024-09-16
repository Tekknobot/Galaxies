using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public GameObject fragmentPrefab; // Prefab for the fragments

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Planet"))
        {
            BreakApart();
        }
    }

    void BreakApart()
    {
        // Destroy the current asteroid
        //Destroy(gameObject);

        // Create fragments
        for (int i = 0; i < 34; i++) // Example: 10 fragments
        {
            // Instantiate fragment
            GameObject fragment = Instantiate(fragmentPrefab, transform.position, Random.rotation);

            // Set scale for fragment
            float scale = Random.Range(0.02f, 0.05f); // Example scale range for fragments
            fragment.transform.localScale = new Vector3(scale, scale, scale);

            // Add Rigidbody to fragments
            Rigidbody rb = fragment.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.mass = Mathf.Pow(scale, 3) * 10f; // Adjust based on desired mass scaling
                rb.AddExplosionForce(0f, transform.position, 5f); // Add explosion force to simulate the breakup
            }

            // Optionally, destroy fragments after some time
            Destroy(fragment, 15f); // Destroy after 15 seconds
        }
    }
}
