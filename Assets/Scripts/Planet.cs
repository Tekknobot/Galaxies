using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    private string planetName;
    private string planetResources;
    private string planetHistory; // New field for history

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

    public void Initialize(string name, string resource, string history)
    {
        planetName = name;
        planetResources = resource;
        planetHistory = planetHistories[Random.Range(0, planetHistories.Count)]; // Randomly select a history
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
