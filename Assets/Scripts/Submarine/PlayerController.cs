using UnityEngine;

namespace Submarine
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Propeller))]
    public class PlayerController : MonoBehaviour
    {
        public float propellerFactor_;
        public Vector2 maxAcceleration_;
        public Vector2 acceleration_;

        private Propeller prop_;
        private Rigidbody rb_;

        private void Awake()
        {
            prop_ = GetComponent<Propeller>();
            rb_ = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            float forward = Input.GetAxisRaw("Vertical");

            if(forward > 0)
                rb_.velocity += Vector3.forward * maxAcceleration_.x * Time.deltaTime;
            else if(forward < 0)
                rb_.velocity -= Vector3.forward * maxAcceleration_.y * Time.deltaTime;

            float currentForward = rb_.velocity.z;

            if(currentForward > maxAcceleration_.x)
                rb_.velocity = new Vector3(rb_.velocity.x, rb_.velocity.y, maxAcceleration_.x);
            else if(currentForward < -maxAcceleration_.y)
                rb_.velocity = new Vector3(rb_.velocity.x, rb_.velocity.y, -maxAcceleration_.y);

            if(currentForward != 0)
                prop_.SetTargetSpeed(currentForward * propellerFactor_);
        }
    }
}