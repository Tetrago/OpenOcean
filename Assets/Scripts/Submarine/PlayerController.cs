﻿using UnityEngine;

namespace Submarine
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Propeller))]
    public class PlayerController : MonoBehaviour
    {
        public float propellerFactor_;
        public Vector2 acceleration_;
        public float pitchTorque_;
        public float yawTorque_;
        public float rollTorque_;

        private Propeller prop_;
        private Rigidbody rb_;

        private void Awake()
        {
            prop_ = GetComponent<Propeller>();
            rb_ = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            Velocity();
            Torque();
        }

        private void Velocity()
        {
            float forward = Input.GetAxisRaw("Speed");

            if(forward > 0)
                rb_.AddForce(transform.forward * acceleration_.x);
            else if(forward < 0)
                rb_.AddForce(-transform.forward * acceleration_.y);

            Vector3 currentForward = Vector3.Scale(rb_.velocity, transform.forward);
            prop_.SetTargetSpeed(Mathf.Max(Mathf.Max(currentForward.x, currentForward.y), currentForward.z) * propellerFactor_);
        }

        private void Torque()
        {
            float pitch = Input.GetAxisRaw("Vertical");
            float yaw = Input.GetAxisRaw("Horizontal");
            float roll = Input.GetAxisRaw("Rotation");

            rb_.AddTorque(transform.right * pitchTorque_ * pitch + transform.up * yawTorque_ * yaw + transform.forward * rollTorque_ * roll);
        }
    }
}