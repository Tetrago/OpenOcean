using UnityEngine;

[System.Serializable]
public struct NoiseProfile
{
    [System.Serializable]
    public struct Params
    {
        public const uint NUM_PARAMS = 5u;
        public float a_, b_, c_, d_, e_;
    }

    public Vector3 scale_;
    public int octaves_;
    public float persistance_;
    public float lacunarity_;
    public Vector3 sampleOffset_;
    public Params params_;
}