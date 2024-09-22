using UnityEngine;

public class Planet : MonoBehaviour
{
    private string planetName;
    private string planetResources;
    private string planetHistory; // New field for history

    public void Initialize(string name, string resource, string history)
    {
        planetName = name;
        planetResources = resource;
        planetHistory = history; // Initialize history
        gameObject.name = planetName;
    }

    public string GetResource()
    {
        return planetResources;
    }

    public string GetName()
    {
        return planetName;
    }

    public string GetHistory() // New method to get history
    {
        return planetHistory;
    }
}
