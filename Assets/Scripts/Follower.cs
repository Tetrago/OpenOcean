using UnityEngine;

public class Follower : MonoBehaviour
{
    public Transform target_;

    private void FixedUpdate()
    {
        transform.position = target_.position;
    }
}
