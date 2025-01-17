using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float lifetime = 5.0f; // Set this to match the particle's lifetime

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
