using UnityEngine;

namespace Submarine
{
    public class BubbleParticles : MonoBehaviour
    {
        public float propellerInfluence_;
        public ParticleSystem particles_;

        private Rigidbody rb_;

        private void Awake()
        {
            rb_ = GetComponent<Rigidbody>();
        }

        private void LateUpdate()
        {
            particles_.startSpeed = rb_.velocity.z < propellerInfluence_ ? propellerInfluence_ : rb_.velocity.z;
        }
    }
}