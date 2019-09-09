using UnityEngine;

[CreateAssetMenu(menuName = "OpenOcean/NoiseProfile")]
public class NoiseProfile : ScriptableObject
{
    public Vector3 scale_;
    public int octaves_;
    public float persistance_;
    public float lacunarity_;
}