using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro; // Include for TextMeshPro

public class SolarSystemGenerator : MonoBehaviour
{
    public int numberOfPlanets = 8;
    public int orbitCount = 5;
    public GameObject sunPrefab;
    public GameObject planetPrefab;
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

    private List<string> planetResources = new List<string>
    {
        "Crystalline minerals that enhance energy absorption.",
        "Toxic gases that can be processed into powerful explosives.",
        "Rare flora that can be used for advanced medicinal compounds.",
        "Iron-rich dust that can be refined into super alloys.",
        "Gaseous compounds ideal for fuel and energy production.",
        "Ice deposits containing water that can sustain life.",
        "Liquid crystals that enhance technology and create energy shields.",
        "Metals with unique properties used in advanced technology.",
        "Silicate-rich soil that can be transformed into nutrient sources.",
        "Magnetic ore used for crafting advanced navigation devices.",
        "Luminous minerals that power energy-based weapons.",
        "Gems with the ability to store and release energy.",
        "Fossilized organisms that yield potent biofuel.",
        "Nanoparticles found in dust storms that can be harvested for tech.",
        "Aetheric particles that enhance magical abilities.",
        "Hyperconductive fibers used in advanced communication systems.",
        "Volcanic glass used to create cutting-edge weaponry.",
        "Liquid metal that can be molded into any shape.",
        "Bio-luminescent algae used for lighting and energy.",
        // Add more resources as needed...
    };

    private List<string> planetHistories = new List<string>
    {
        "Once a thriving world, it faced devastation from a massive asteroid impact, reshaping its landscape.",
        "This planet was the site of an ancient civilization known for its advanced technology and intricate art.",
        "A long-standing conflict between its two major factions led to a century of war, now a peaceful land.",
        "Discovered by explorers centuries ago, it became a hub for trade due to its rich resources.",
        "A mysterious event in its past caused its oceans to evaporate, leaving vast deserts behind.",
        "Once inhabited by giant creatures, it now stands as a monument to their extinction.",
        "A great scientific discovery was made here, leading to advancements in space travel.",
        "This planet is known for its legendary storm, which has raged for millennia, influencing its weather patterns.",
        "A significant political alliance formed here has shaped interstellar relations for generations.",
        "In its early history, the planet was a barren wasteland, transformed into a lush paradise through terraforming efforts.",
        "The remnants of a colossal space station still orbit this planet, a relic of its once-dominant spacefaring civilization.",
        "It was once thought to be uninhabitable, but underground ecosystems were discovered, thriving in isolation.",
        "The planet hosted a galactic summit that led to a groundbreaking peace treaty between rival factions.",
        "An ancient prophecy foretold the rise of a great leader born on this planet, changing its fate forever.",
        "The discovery of a rare mineral led to a gold rush, sparking an economic boom that lasted decades.",
        "This planet was the first to establish a university of interstellar knowledge, attracting scholars from across the galaxy.",
        "Its unique biosphere has made it a key location for scientific research on alien life forms.",
        "The planet's rich folklore and myths have shaped its culture, celebrating heroes and legendary figures.",
        "A catastrophic event, known as the Great Collapse, drastically altered its terrain and climate.",
        "The last known dragon-like creatures were sighted here, inspiring countless tales of adventure.",
        "Once a paradise, it was ravaged by climate change, leading to a struggle for survival among its inhabitants.",
        "A forgotten war left this planet with ancient ruins that tell tales of heroism and tragedy.",
        "Home to a mysterious artifact that grants incredible powers, attracting adventurers from across the galaxy.",
        "This planet was the birthplace of a legendary explorer who charted the unknown regions of space.",
        "Its deep canyons and vast chasms hide secrets of ancient civilizations waiting to be uncovered.",
        "A thriving trade hub, it became the center of an intergalactic marketplace that spanned light-years.",
        "The planet's flora has evolved to produce unique compounds sought after by medicinal researchers.",
        "Once considered a barren rock, it became a vital mining colony, transforming its economy.",
        "The lush forests here are said to harbor spirits that protect the land from invaders.",
        "A significant scientific accident changed the course of its history, leading to unforeseen consequences.",
        "It was a popular vacation destination for galactic elites, known for its breathtaking landscapes.",
        "The rise of an influential political figure on this planet shifted the balance of power in the galaxy.",
        "Its inhabitants have a rich oral tradition, passing down stories of bravery and wisdom through generations.",
        "A deep-space anomaly near the planet revealed unexpected phenomena that changed scientific understanding.",
        // Add more histories as needed...
    };

    private List<Vector3> planetPositions = new List<Vector3>();
    public TextMeshProUGUI planetBioText; // Reference to the UI Text for bios

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

            // Initialize the Planet component
            Planet planetComponent = planet.AddComponent<Planet>();
            planetComponent.Initialize(planetNames[i % planetNames.Count], planetResources[i % planetResources.Count], planetHistories[i % planetHistories.Count]);

            Orbit orbit = planet.AddComponent<Orbit>();
            orbit.sun = GameObject.FindWithTag("Sun").transform;
            orbit.orbitSpeed = orbitSpeed;
            orbit.distanceFromSun = orbitRadius;

            RotateAroundSelf rotateSelf = planet.AddComponent<RotateAroundSelf>();
            rotateSelf.rotationSpeed = Random.Range(minSelfRotationSpeed, maxSelfRotationSpeed);

            CreateOrbitPath(orbitRadius);
        }
    }

    void CreateMoon(GameObject planet, float planetScale)
    {
        float moonScale = planetScale * Random.Range(0.03f, 0.05f); // Scale the moon relative to the planet
        float moonOrbitRadius = Random.Range(0.5f, 1.5f) * planetScale; // Orbit radius relative to planet size
        float moonOrbitSpeed = Random.Range(1f, 3f); // Moon orbit speed

        // Calculate a starting position for the moon
        Vector3 moonStartPosition = planet.transform.position + new Vector3(moonOrbitRadius, moonOrbitRadius, 0);

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
