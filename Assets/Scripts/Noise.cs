using UnityEngine;

public class Noise : MonoBehaviour
{
    public ComputeShader shader_;

    private int kernel_;

    private void Awake()
    {
        kernel_ = shader_.FindKernel("CSMain");
    }

    private static Vector2 GetLerp(NoiseProfile profile)
    {
        float amplitude = 1;
        for(uint i = 0u; i < profile.octaves_; ++i)
            amplitude *= profile.persistance_;

        return new Vector2(-amplitude, amplitude);
    }

    public float[] Generate(Vector3Int size, Vector3 pos, NoiseProfile profile, GenerationProfile.Type type)
    {
        switch(type)
        {
            case GenerationProfile.Type.SimplexCaverns:
                return SimplexCaverns(size, pos, profile);
            case GenerationProfile.Type.Full:
                return Full(size);
        }

        return new float[size.x * size.y * size.z];
    }

    private float[] SimplexCaverns(Vector3Int size, Vector3 pos, NoiseProfile profile)
    {
        ComputeBuffer buffer = new ComputeBuffer(size.x * size.y * size.z, sizeof(float));

        shader_.SetBuffer(kernel_, "points_", buffer);
        shader_.SetInts("size_", size.x, size.y, size.z);
        shader_.SetFloats("offset_", pos.x, pos.y, pos.z);
        shader_.SetFloats("scale_", profile.scale_.x, profile.scale_.y, profile.scale_.z);
        shader_.SetInt("octaves_", profile.octaves_);
        shader_.SetFloat("persistance_", profile.persistance_);
        shader_.SetFloat("lacunarity_", profile.lacunarity_);

        Vector2 lerp = GetLerp(profile);
        shader_.SetFloats("lerp_", lerp.x, lerp.y);

        shader_.Dispatch(kernel_, size.x / World.THREADS, size.y / World.THREADS, size.x / World.THREADS);

        float[] points = new float[size.x * size.y * size.z];
        buffer.GetData(points);

        buffer.Release();

        return points;
    }

    private float[] Full(Vector3Int size)
    {
        float[] points = new float[size.x * size.y * size.z];
        for(uint i = 0u; i < points.Length; ++i)
            points[i] = 1.0f;
        return points;
    }
}
