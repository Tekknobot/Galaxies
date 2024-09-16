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
        for (int i = 0; i < 34; i++)
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

                // Set a custom velocity
                Vector3 explosionDirection = Random.onUnitSphere; // Random direction
                float speed = Random.Range(0.005f, 0.005f); // Control the speed here
                rb.velocity = explosionDirection * speed;

                // Optionally, add a small force for some variation
                rb.AddForce(explosionDirection * speed * 0.01f, ForceMode.Impulse);
            }

            // Optionally, destroy fragments after some time
            Destroy(fragment, 15f); // Destroy after 15 seconds
        }
    }

}
