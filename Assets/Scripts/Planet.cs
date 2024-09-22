using UnityEngine;

public class Planet : MonoBehaviour
{
    private string planetName;
    private string planetResources;
    private string planetHistory;

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object is another planet
        if (collision.gameObject.CompareTag("Planet"))
        {
            DestroyPlanet();
        }
    }

    private void DestroyPlanet()
    {
        // Optionally, play an explosion effect or sound here
        Destroy(gameObject); // Destroy the planet
    }

    public void Initialize(string name, string resource, string history)
    {
        planetName = name;
        planetResources = resource;
        planetHistory = history;
        gameObject.name = planetName;

        // Add a Collider component if not already present
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<SphereCollider>();
        }
    }

    public string GetResource() => planetResources;
    public string GetName() => planetName;
    public string GetHistory() => planetHistory;
}
