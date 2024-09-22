using UnityEngine;

public class Planet : MonoBehaviour
{
    private string planetName;
    private float planetScale;
    private string planetResources;

    public void Initialize(string name, float scale, string resource)
    {
        planetName = name;
        planetScale = scale;
        planetResources = resource;
        gameObject.name = planetName;
    }

    public string GetResource()
    {
        return planetResources;
    }

    // Additional methods...
}

