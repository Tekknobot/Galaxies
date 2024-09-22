using UnityEngine;
using TMPro;

public class SolarSystemUIManager : MonoBehaviour
{
    public TMP_Text planetNameText; // Reference to the TMP Text component to display planet names

    private void Start()
    {
        if (planetNameText != null)
        {
            planetNameText.text = ""; // Initialize as empty
        }
    }

    // Show the planet name in the UI
    public void ShowPlanetName(string name)
    {
        if (planetNameText != null)
        {
            planetNameText.text = name;
        }
    }

    // Hide the planet name in the UI
    public void HidePlanetName()
    {
        if (planetNameText != null)
        {
            planetNameText.text = ""; // Clear the name
        }
    }
}
