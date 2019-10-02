using UnityEngine;

public class ParticleGravity : MonoBehaviour
{
    public float forceUp_ = 0.01f;
    public ParticleSystem particles_;

    private void LateUpdate()
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particles_.particleCount + 1u];
        int l = particles_.GetParticles(particles);

        for(int i = 0; i < l; ++i)
        {
            particles[i].velocity = particles[i].velocity + Vector3.up * forceUp_ * particles[i].lifetime;
        }

        particles_.SetParticles(particles);
    }
}
