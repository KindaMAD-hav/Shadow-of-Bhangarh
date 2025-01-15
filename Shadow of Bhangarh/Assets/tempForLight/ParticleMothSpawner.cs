using UnityEngine;

public class ParticleMothSpawner : MonoBehaviour
{
    public GameObject mothPrefab; // Assign the moth prefab in the Inspector
    private ParticleSystem particleSystem;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Play();
    }

    void Update()
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
        int numParticlesAlive = particleSystem.GetParticles(particles);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            if (!particles[i].startLifetime.Equals(0))
            {
                Vector3 position = particles[i].position + transform.position;
                Instantiate(mothPrefab, position, Quaternion.identity);
            }
        }
    }
}
