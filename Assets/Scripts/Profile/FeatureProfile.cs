using UnityEngine;

[System.Serializable]
public struct FeatureProfile
{
    [System.Serializable]
    public struct Spawner
    {
        public GameObject go_;
        public uint rarity_;
    }

    public Spawner[] spawners_;
}
