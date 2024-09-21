using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SolarSystemGenerator : MonoBehaviour
{
    public int numberOfPlanets = 8;
    public int orbitCount = 5;
    public GameObject sunPrefab;
    public GameObject planetPrefab;
    public GameObject starrySystemPrefab;
    public Material orbitMaterial;
    public Material[] planetMaterials;
    public Material moonMaterial; // Material for the moons
    public Material sunShader;

    public float sunScale = 2.0f;
    public float planetMinScale = 0.2f;
    public float planetMaxScale = 1.0f;
    public float minDistance = 5.0f;
    public float maxDistance = 20.0f;
    public float minOrbitSpeed = 5.0f;
    public float maxOrbitSpeed = 15.0f;
    public float minSelfRotationSpeed = 5.0f;
    public float maxSelfRotationSpeed = 20.0f;

    public float orbitThickness = 0.0125f;
    public List<GameObject> Planets { get; private set; } = new List<GameObject>();

    private List<string> planetNames = new List<string>
    {
        "Astra", "Zephira", "Orion", "Nova", "Aurora", "Helios", "Lunaris", "Vega", "Nebula", "Stellaris",
        "Celestia", "Cosmos", "Solis", "Equinox", "Zodiac", "Draco", "Galaxa", "Phaeton", "Hyperion", "Triton",
        "Cronus", "Icarus", "Phoebe", "Thanatos", "Arcturus", "Sirius", "Titan", "Mira", "Andromeda", "Rigel",
        "Altair", "Castor", "Pollux", "Lyra", "Sphinx", "Electra", "Gaia", "Janus", "Nyx", "Cygnus",
        "Hercules", "Pegasus", "Callisto", "Europa", "Ganymede", "Io", "Perseus", "Cepheus", "Hydra", "Scorpius"
    };

    private List<Vector3> planetPositions = new List<Vector3>();

    void Start()
    {
        CreateSun();
        GeneratePlanets();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetScene();
        }
    }

    void CreateSun()
    {
        GameObject sun = CreateSphere("Sun", Vector3.zero, sunScale, sunShader);
        sun.tag = "Sun";

        Light sunLight = sun.AddComponent<Light>();
        sunLight.type = LightType.Point;
        sunLight.range = 100f;
        sunLight.intensity = 1.5f;
        sunLight.color = new Color(1f, 0.95f, 0.8f);
        sunLight.shadows = LightShadows.Soft;

        // Instantiate starry system at the sun's position
        InstantiateStarrySystem(sun.transform.position);
    }

    void GeneratePlanets()
    {
        planetNames.Shuffle();
        List<float> orbitRadii = GenerateUniqueOrbits();
        numberOfPlanets = Random.Range(8, numberOfPlanets);

        for (int i = 0; i < numberOfPlanets; i++)
        {
            float orbitRadius = orbitRadii[Random.Range(0, orbitRadii.Count)];
            float planetScale = Random.Range(planetMinScale, planetMaxScale);
            float orbitSpeed = GetOrbitSpeedForRadius(orbitRadius);
            float startingAngle = Random.Range(0f, 360f);
            Vector3 startPosition = CalculateOrbitPosition(orbitRadius, startingAngle);

            while (IsCollidingWithOtherPlanets(startPosition, planetScale))
            {
                startingAngle += 5f;
                startPosition = CalculateOrbitPosition(orbitRadius, startingAngle);
            }

            GameObject planet = CreateSphere($"Planet_{i + 1}", startPosition, planetScale, GetRandomPlanetMaterial());
            planet.tag = "Planet";

            Orbit orbit = planet.AddComponent<Orbit>();
            orbit.sun = GameObject.FindWithTag("Sun").transform;
            orbit.orbitSpeed = orbitSpeed;
            orbit.distanceFromSun = orbitRadius;

            RotateAroundSelf rotateSelf = planet.AddComponent<RotateAroundSelf>();
            rotateSelf.rotationSpeed = Random.Range(minSelfRotationSpeed, maxSelfRotationSpeed);

            CreateOrbitPath(orbitRadius);
            planet.name = planetNames[i % planetNames.Count];

            Planets.Add(planet);
            planetPositions.Add(startPosition);

            // Randomly assign moons to larger planets
            if (Random.value < 0.5f) // 50% chance to have a moon
            {
                CreateMoon(planet, planetScale);
            }

            InstantiateStarrySystem(planet.transform.position);
        }
    }

    void CreateMoon(GameObject planet, float planetScale)
    {
        float moonScale = planetScale * Random.Range(0.03f, 0.05f); // Scale the moon relative to the planet
        float moonOrbitRadius = Random.Range(0.5f, 1.5f) * planetScale; // Orbit radius relative to planet size
        float moonOrbitSpeed = Random.Range(1f, 3f); // Moon orbit speed

        // Calculate a starting position for the moon
        Vector3 moonStartPosition = planet.transform.position + new Vector3(moonOrbitRadius, 0, 0);

        GameObject moon = CreateSphere($"Moon_{planet.name}", moonStartPosition, moonScale, moonMaterial);
        moon.tag = "Moon";

        // Add orbit behavior for the moon
        Orbit moonOrbit = moon.AddComponent<Orbit>();
        moonOrbit.sun = planet.transform; // The planet is the "sun" for the moon
        moonOrbit.orbitSpeed = moonOrbitSpeed;
        moonOrbit.distanceFromSun = moonOrbitRadius;

        // Add self-rotation behavior for the moon
        RotateAroundSelf moonRotateSelf = moon.AddComponent<RotateAroundSelf>();
        moonRotateSelf.rotationSpeed = Random.Range(minSelfRotationSpeed, maxSelfRotationSpeed);
    }

    void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    GameObject CreateSphere(string name, Vector3 position, float scale, Material material)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = name;
        sphere.transform.position = position;
        sphere.transform.localScale = new Vector3(scale, scale, scale);

        Collider collider = sphere.GetComponent<Collider>();
        if (collider == null)
        {
            collider = sphere.AddComponent<SphereCollider>();
        }

        Rigidbody rb = sphere.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.mass = Mathf.Pow(scale, 3) * 100f;
        rb.drag = 0.1f;
        rb.angularDrag = 0.1f;

        Renderer renderer = sphere.GetComponent<Renderer>();
        renderer.material = material;

        return sphere;
    }

    void InstantiateStarrySystem(Vector3 position)
    {
        if (starrySystemPrefab != null)
        {
            Instantiate(starrySystemPrefab, position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Starry system prefab is not assigned.");
        }
    }

    Vector3 CalculateOrbitPosition(float radius, float angleDegrees)
    {
        float angleRadians = Mathf.Deg2Rad * angleDegrees;
        float x = Mathf.Sin(angleRadians) * radius;
        float z = Mathf.Cos(angleRadians) * radius;
        return new Vector3(x, 0, z);
    }

    bool IsCollidingWithOtherPlanets(Vector3 newPosition, float newScale)
    {
        foreach (Vector3 position in planetPositions)
        {
            float distance = Vector3.Distance(newPosition, position);
            if (distance < newScale + planetMinScale)
            {
                return true;
            }
        }
        return false;
    }

    void CreateOrbitPath(float radius)
    {
        GameObject orbit = new GameObject($"Orbit_{radius}");
        LineRenderer lineRenderer = orbit.AddComponent<LineRenderer>();

        lineRenderer.material = orbitMaterial;
        lineRenderer.widthMultiplier = orbitThickness;

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

    Material GetRandomPlanetMaterial()
    {
        if (planetMaterials.Length == 0)
        {
            Debug.LogWarning("No planet materials assigned. Using default material.");
            return new Material(Shader.Find("Standard"));
        }

        return planetMaterials[Random.Range(0, planetMaterials.Length)];
    }

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

    float GetOrbitSpeedForRadius(float radius)
    {
        float normalizedRadius = Mathf.InverseLerp(minDistance, maxDistance, radius);
        return Mathf.Lerp(minOrbitSpeed, maxOrbitSpeed, normalizedRadius);
    }

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

    public class RotateAroundSelf : MonoBehaviour
    {
        public float rotationSpeed;

        void Update()
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    public class RotateSun : MonoBehaviour
    {
        public float rotationSpeed = 10.0f;

        void Update()
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
}

// Extension method for shuffling lists
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
