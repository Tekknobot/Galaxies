using UnityEngine;

public class AsteroidFragment : MonoBehaviour
{
    public float lifetime = 5f; // Time before fragment is destroyed

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
