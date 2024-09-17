using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour
{
    public GameObject fragmentPrefab;   // Prefab for the fragments
    public float slowMotionDuration = 2f; // Duration for slow-motion
    public float slowMotionFactor = 0.1f; // How slow time gets (0.2 means 20% of normal speed)

    private CameraControl cameraControl;  // Reference to the CameraControl script

    void Start()
    {
        // Find the CameraControl script on the main camera
        cameraControl = Camera.main.GetComponent<CameraControl>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Planet") || collision.gameObject.CompareTag("Sun"))
        {
            // Trigger the bullet-time effect
            //StartCoroutine(BulletTimeEffect());

            // Disable gameObject renderer
            gameObject.GetComponent<Renderer>().enabled = false;
            
            // Break apart the asteroid
            BreakApart();

            // Focus and zoom on the planet the asteroid collided with
            //cameraControl.FocusOnPlanet(collision.transform);
        }
    }

    void BreakApart()
    {
        // Create fragments
        for (int i = 0; i < 34; i++)
        {
            // Instantiate fragment
            GameObject fragment = Instantiate(fragmentPrefab, transform.position, Random.rotation);

            // Set scale for fragment
            float scale = Random.Range(0.01f, 0.025f); // Example scale range for fragments
            fragment.transform.localScale = new Vector3(scale, scale, scale);

            // Add Rigidbody to fragments
            Rigidbody rb = fragment.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.mass = Mathf.Pow(scale, 3) * 10f; // Adjust based on desired mass scaling

                // Set a custom velocity
                Vector3 explosionDirection = Random.onUnitSphere; // Random direction
                float speed = Random.Range(0.0f, 0.0f); // Control the speed here
                rb.velocity = explosionDirection * speed;

                // Optionally, add a small force for some variation
                //rb.AddForce(explosionDirection * speed * 0.0f, ForceMode.Impulse);
            }

            // Optionally, destroy fragments after some time
            Destroy(fragment, 15f); // Destroy after 15 seconds
        }
    }

    // Coroutine to handle bullet time effect
    IEnumerator BulletTimeEffect()
    {
        // Slow down time
        Time.timeScale = slowMotionFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f; // Adjust physics step with the time scale

        // Wait for the slow motion duration
        yield return new WaitForSecondsRealtime(slowMotionDuration);

        // Gradually restore time back to normal
        float elapsedTime = 0f;
        float originalTimeScale = 1f; // Normal time scale

        while (elapsedTime < slowMotionDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(slowMotionFactor, originalTimeScale, elapsedTime / slowMotionDuration);
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            yield return null;  // Wait for the next frame
        }

        // Ensure time is fully restored
        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = 0.02f;  // Reset physics step to the default value
    }
}
