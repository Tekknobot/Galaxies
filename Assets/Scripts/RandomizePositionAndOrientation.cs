using System.Collections;
using UnityEngine;

public class RandomizePositionAndOrientation : MonoBehaviour
{
    public string sunTag = "Sun"; // Tag for the sun
    public float minX = -1000f; // Minimum X position
    public float maxX = 1000f;  // Maximum X position
    public float minY = -1000f; // Minimum Y position
    public float maxY = 1000f;  // Maximum Y position
    public float minZ = -1000f; // Minimum Z position
    public float maxZ = 1000f;  // Maximum Z position
    public float maxRetries = 10; // Maximum number of retries
    public float retryDelay = 0.5f; // Delay between retries

    private void Start()
    {
        RandomizePosition();
        StartCoroutine(TryOrientTowardsSun());
    }

    private void RandomizePosition()
    {
        // Generate random position within the defined space
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        float randomZ = Random.Range(minZ, maxZ);

        // Set the position of the spacecraft
        transform.position = new Vector3(randomX, randomY, randomZ);
    }

    private IEnumerator TryOrientTowardsSun()
    {
        int retries = 0;
        GameObject sunObject;

        while (retries < maxRetries)
        {
            sunObject = GameObject.FindGameObjectWithTag(sunTag);

            if (sunObject != null)
            {
                // Get the position of the sun
                Vector3 directionToSun = sunObject.transform.position - transform.position;

                // Calculate the rotation required to face the sun
                Quaternion targetRotation = Quaternion.LookRotation(directionToSun);

                // Apply the rotation
                transform.rotation = targetRotation;
                yield break;
            }

            retries++;
            yield return new WaitForSeconds(retryDelay);
        }

        Debug.LogError("Failed to find GameObject with tag '" + sunTag + "' after " + maxRetries + " retries.");
    }
}
