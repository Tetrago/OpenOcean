using UnityEngine;

namespace Submarine
{
    public class Propeller : MonoBehaviour
    {
        public Transform propeller_;
        public float targetSpeed_ = 0.2f;
        public float smoothSpeed_ = 0.2f;

        private float currentSpeed_;
        private float velocity_;

        private void Awake()
        {
            currentSpeed_ = targetSpeed_;
        }

        private void Update()
        {
            if(currentSpeed_ != targetSpeed_)
                currentSpeed_ = Mathf.SmoothDamp(currentSpeed_, targetSpeed_, ref velocity_, smoothSpeed_, targetSpeed_, Time.deltaTime);

            Vector3 rot = propeller_.rotation.eulerAngles;
            propeller_.rotation = Quaternion.RotateTowards(propeller_.rotation, Quaternion.Euler(rot + Vector3.forward * currentSpeed_), currentSpeed_ * Time.deltaTime);
        }

        public void SetTargetSpeed(float speed)
        {
            targetSpeed_ = speed;
        }
    }
}
