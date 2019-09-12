using UnityEngine;

[System.Serializable]
public struct GenerationProfile
{
    public enum Type
    {
        SimplexCaverns,
        Full
    }

    [System.Serializable]
    public struct Level
    {
        public Type type_;
        public uint height_;
        public uint blendDist_;

        public NoiseProfile noiseProfile_;
    }

    public Level[] levels_;
}
