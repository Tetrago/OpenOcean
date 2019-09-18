using UnityEngine;

namespace Submarine
{
    public class BubbleParticles : MonoBehaviour
    {
        public float propellerInfluence_;
        public float forceUp_;
        public ParticleSystem particles_;

        private Rigidbody rb_;

        private void Awake()
        {
            rb_ = GetComponent<Rigidbody>();
        }

        private void LateUpdate()
        {
            particles_.startSpeed = rb_.velocity.z < propellerInfluence_ ? propellerInfluence_ : rb_.velocity.z;

            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particles_.particleCount + 1u];
            int l = particles_.GetParticles(particles);

            for(int i = 0; i < l; ++i)
            {
                particles[i].velocity = particles[i].velocity + Vector3.up * forceUp_ * particles[i].lifetime;
            }

            particles_.SetParticles(particles);
        }
    }
}