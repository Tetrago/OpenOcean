using UnityEngine;

namespace Submarine
{
    public class BubbleParticles : MonoBehaviour
    {
        public ParticleSystem particles_;

        private Rigidbody rb_;

        private void Awake()
        {
            rb_ = GetComponent<Rigidbody>();
        }

        private void LateUpdate()
        {
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particles_.particleCount + 1u];
            int l = particles_.GetParticles(particles);

            for(int i = 0; i < l; ++i)
            {
                particles[i].velocity = -rb_.velocity;
            }

            particles_.SetParticles(particles);
        }
    }
}