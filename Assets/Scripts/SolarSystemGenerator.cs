using UnityEngine;
using System.Collections.Generic;

public class SolarSystemGenerator : MonoBehaviour
{
    public int numberOfPlanets = 8;        // Number of planets to generate
    public int orbitCount = 5;             // Number of unique orbits
    public GameObject sunPrefab;           // Prefab or primitive shape for the Sun
    public GameObject planetPrefab;        // Prefab or primitive shape for the Planets
    public Material orbitMaterial;         // Material for the orbital path LineRenderer
    public Material[] planetMaterials;     // Array of materials for the planets
    public Material sunShader;               // Shader for the Sun (can be null)
    public GameObject fragmentPrefab;      // Prefab for the fragments used in shattering

    public float sunScale = 2.0f;          // Scale of the Sun
    public float planetMinScale = 0.2f;    // Minimum scale of a planet
    public float planetMaxScale = 1.0f;    // Maximum scale of a planet
    public float minDistance = 5.0f;       // Minimum distance from the Sun
    public float maxDistance = 20.0f;      // Maximum distance from the Sun
    public float minOrbitSpeed = 5.0f;     // Minimum orbit speed for planets
    public float maxOrbitSpeed = 15.0f;    // Maximum orbit speed for planets

    private List<string> planetNames = new List<string>
    {
        "Astra", "Zephira", "Orion", "Nova", "Aurora", "Helios", "Lunaris", "Vega", "Nebula", "Stellaris",
        "Celestia", "Cosmos", "Solis", "Equinox", "Zodiac", "Draco", "Galaxa", "Phaeton", "Hyperion", "Triton",
        "Cronus", "Icarus", "Phoebe", "Thanatos", "Arcturus", "Sirius", "Titan", "Mira", "Andromeda", "Rigel",
        "Altair", "Castor", "Pollux", "Lyra", "Sphinx", "Electra", "Gaia", "Janus", "Nyx", "Cygnus",
        "Hercules", "Pegasus", "Callisto", "Europa", "Ganymede", "Io", "Perseus", "Cepheus", "Hydra", "Scorpius"
    };

    void Start()
    {
        // Create the Sun with the provided shader (or default shader if none is assigned)
        GameObject sun = CreateSphere("Sun", Vector3.zero, sunScale, sunShader);

        // Shuffle the planet names to ensure unique assignment
        planetNames.Shuffle();

        // Generate unique orbits
        List<float> orbitRadii = GenerateUniqueOrbits();

        // Generate planets
        for (int i = 0; i < numberOfPlanets; i++)
        {
            // Randomly select an orbit radius from the available orbits
            float orbitRadius = orbitRadii[Random.Range(0, orbitRadii.Count)];
            float planetScale = Random.Range(planetMinScale, planetMaxScale);
            float orbitSpeed = GetOrbitSpeedForRadius(orbitRadius);
            float startingAngle = Random.Range(0f, 360f); // Random angle in degrees

            // Calculate the planet's initial position using polar coordinates
            Vector3 startPosition = CalculateOrbitPosition(orbitRadius, startingAngle);

            // Create planet at the calculated position with a random material
            // Create planet at the calculated position with a random material
            GameObject planet = CreateSphere($"Planet_{i + 1}", startPosition, planetScale, GetRandomPlanetMaterial());
            planet.tag = "Planet"; // Ensure the planet has the "Planet" tag

            // Add orbit behavior (rotation around the sun)
            Orbit orbit = planet.AddComponent<Orbit>();
            orbit.sun = sun.transform;
            orbit.orbitSpeed = orbitSpeed;
            orbit.distanceFromSun = orbitRadius;

            // Add shatter behavior
            Shatter shatter = planet.AddComponent<Shatter>();
            shatter.fragmentPrefab = fragmentPrefab; // Set the fragment prefab

            // Add collision handling behavior
            PlanetCollisionHandler collisionHandler = planet.AddComponent<PlanetCollisionHandler>();
            collisionHandler.shatter = shatter; // Reference to the Shatter component

            // Create orbital path
            CreateOrbitPath(orbitRadius);

            // Assign a random unique name from the list
            string planetName = planetNames[i % planetNames.Count]; // Ensure name uniqueness
            planet.name = planetName;  // Set the name of the planet
        }
    }

    // Function to create a sphere (either for Sun or Planets)
    GameObject CreateSphere(string name, Vector3 position, float scale, Material material)
    {
        // Create a new GameObject and add a sphere
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = name;
        sphere.transform.position = position;
        sphere.transform.localScale = new Vector3(scale, scale, scale);

        // Ensure a collider is present
        Collider collider = sphere.GetComponent<Collider>();
        if (collider == null)
        {
            collider = sphere.AddComponent<SphereCollider>();
        }

        // Add Rigidbody to enable collision
        Rigidbody rb = sphere.AddComponent<Rigidbody>();
        rb.useGravity = false; // Assuming gravity should not affect planets
        rb.isKinematic = false; // Set to true if you want to manually control movement

        // Apply the material
        Renderer renderer = sphere.GetComponent<Renderer>();
        renderer.material = material;

        return sphere;
    }

    // Function to calculate the position of a planet in its orbit based on the radius and angle
    Vector3 CalculateOrbitPosition(float radius, float angleDegrees)
    {
        float angleRadians = Mathf.Deg2Rad * angleDegrees;
        float x = Mathf.Sin(angleRadians) * radius;
        float z = Mathf.Cos(angleRadians) * radius;
        return new Vector3(x, 0, z); // y = 0 for a flat orbit on the XZ plane
    }

    // Function to create the orbit path using LineRenderer
    void CreateOrbitPath(float radius)
    {
        GameObject orbit = new GameObject($"Orbit_{radius}");
        LineRenderer lineRenderer = orbit.AddComponent<LineRenderer>();

        // Set the material and appearance of the line
        lineRenderer.material = orbitMaterial;
        lineRenderer.widthMultiplier = 0.05f;

        // Define the number of segments and points to draw a circle
        int segments = 100;
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = false;

        float angle = 0f;
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, 0, z));
            angle += 360f / segments;
        }
    }

    // Function to get a random material from the available planet materials
    Material GetRandomPlanetMaterial()
    {
        if (planetMaterials.Length == 0)
        {
            Debug.LogWarning("No planet materials assigned. Using default material.");
            return new Material(Shader.Find("Standard")); // Fallback to a default material
        }

        return planetMaterials[Random.Range(0, planetMaterials.Length)];
    }

    // Function to generate unique orbit radii
    List<float> GenerateUniqueOrbits()
    {
        List<float> orbitRadii = new List<float>();

        float orbitStep = (maxDistance - minDistance) / orbitCount;
        for (int i = 0; i < orbitCount; i++)
        {
            float radius = minDistance + i * orbitStep;
            orbitRadii.Add(radius);
        }

        return orbitRadii;
    }

    // Function to get the orbit speed for a given orbit radius
    float GetOrbitSpeedForRadius(float radius)
    {
        float normalizedRadius = Mathf.InverseLerp(minDistance, maxDistance, radius);
        return Mathf.Lerp(minOrbitSpeed, maxOrbitSpeed, normalizedRadius);
    }

    // Nested class for handling planet collisions
    public class PlanetCollisionHandler : MonoBehaviour
    {
        public Shatter shatter;

        void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"Collision detected with: {collision.gameObject.name}");

            if (collision.gameObject.CompareTag("Planet"))
            {
                Debug.Log("Collided with another planet.");
                if (shatter != null)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}

// Orbit behavior for each planet
public class Orbit : MonoBehaviour
{
    public Transform sun;
    public float orbitSpeed;
    public float distanceFromSun;

    void Update()
    {
        transform.RotateAround(sun.position, Vector3.up, orbitSpeed * Time.deltaTime);
    }
}

// Extension method to shuffle a list
public static class ListExtensions
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
