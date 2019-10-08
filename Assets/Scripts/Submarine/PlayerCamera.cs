using UnityEngine;

class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance_;

    public Vector2 orbitClamp_ = new Vector2(0, 90);
    public Vector2 scrollClamp_ = new Vector2(1.5f, 100);
    public float sensitivity_ = 4.0f;
    public float scrollSensitivty_ = 2.0f;
    public float dampening_ = 10.0f;
    public float scrollDampening_ = 6.0f;
    public float dist_ = 10.0f;
    public Camera camera_;
    public GameObject submarine_;

    private GameObject pivot_;
    private Vector3 lastMousePos_;
    private Vector3 rot_;
    private Vector3 lastHitPoint_;
    private bool moveCamera_;

    private void Awake()
    {
        instance_ = this;

        pivot_ = new GameObject("Pivot");
        transform.parent = pivot_.transform;

        moveCamera_ = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        pivot_.transform.position = submarine_.transform.position;
    }

    private void LateUpdate()
    {
        if(moveCamera_)
        {
            rot_.x += Input.GetAxis("Mouse X") * sensitivity_;
            rot_.y += Input.GetAxis("Mouse Y") * -sensitivity_;

            rot_.y = Mathf.Clamp(rot_.y, orbitClamp_.x, orbitClamp_.y);
        }

        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            Scroll();
        }

        Quaternion quat = Quaternion.Euler(rot_.y, rot_.x, 0);
        pivot_.transform.rotation = Quaternion.Lerp(pivot_.transform.rotation, quat, Time.deltaTime * dampening_);

        if(transform.localPosition.z != dist_ * -1.0f)
        {
            transform.localPosition = new Vector3(0, 0, Mathf.Lerp(transform.localPosition.z, dist_ * -1.0f, Time.deltaTime * scrollDampening_));
        }
    }

    private void Scroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * scrollSensitivty_;
        scroll *= dist_ * 0.3f;

        dist_ += scroll * -1.0f;
        dist_ = Mathf.Clamp(dist_, scrollClamp_.x, scrollClamp_.y);
    }
}
