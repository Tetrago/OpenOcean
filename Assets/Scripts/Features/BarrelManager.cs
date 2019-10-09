using System.Collections.Generic;
using UnityEngine;

public class BarrelManager : MonoBehaviour
{
    public static BarrelManager instance_;
    public float floor_ = -20;

    private List<Barrel> barrels_;

    private void Awake()
    {
        instance_ = this;
        barrels_ = new List<Barrel>();
    }

    private void Update()
    {
        barrels_.RemoveAll(barrel =>
        {
            if(barrel.transform.position.y < floor_)
            {
                Destroy(barrel.gameObject);
                return true;
            }
            else
            {
                return false;
            }
        });
    }

    public void Register(Barrel barrel)
    {
        barrels_.Add(barrel);
    }
}
