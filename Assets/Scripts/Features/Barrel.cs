using UnityEngine;

public class Barrel : MonoBehaviour
{
    private void Start()
    {
        BarrelManager.instance_.Register(this);
    }
}
