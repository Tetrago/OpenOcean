using UnityEngine;

[System.Serializable]
public struct NoiseProfile
{
    public Vector3 scale_;
    public int octaves_;
    public float persistance_;
    public float lacunarity_;
    public Vector3 sampleOffset_;
    public float floor_;
    public float roof_;
    public float noiseWeight_;
    public float floorWeight_;
    public float weightMultiplier_;
    public float blendDist_;
}