using UnityEngine;

[System.Serializable]
public struct NoiseProfile
{
    public Vector3 scale_;
    public int octaves_;
    public float persistance_;
    public float lacunarity_;
}