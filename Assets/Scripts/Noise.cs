using UnityEngine;

public class Noise : MonoBehaviour
{
    public ComputeShader shader_;

    private static Vector2 GetLerp(NoiseProfile profile)
    {
        float amplitude = 1;
        for(uint i = 0u; i < profile.octaves_; ++i)
            amplitude *= profile.persistance_;

        return new Vector2(-amplitude, amplitude);
    }

    public float[] Generate(Vector3Int size, Vector3 pos, NoiseProfile profile, GenerationProfile.Level level)
    {
        switch(level.type_)
        {
            case GenerationProfile.Type.SimplexCaverns:
                return SimplexCaverns(size, pos, profile);
            case GenerationProfile.Type.SimplexPlane:
                return SimplexPlane(size, pos, profile, level.height_);
            case GenerationProfile.Type.Full:
                return Full(size);
        }

        return new float[size.x * size.y * size.z];
    }

    private float[] SimplexCaverns(Vector3Int size, Vector3 pos, NoiseProfile profile)
    {
        int kernel = shader_.FindKernel("SimplexCaverns");

        ComputeBuffer buffer = new ComputeBuffer(size.x * size.y * size.z, sizeof(float));

        shader_.SetBuffer(kernel, "points_", buffer);
        SetUniforms(size, pos, profile);

        Vector2 lerp = GetLerp(profile);
        shader_.SetFloats("lerp_", lerp.x, lerp.y);

        shader_.Dispatch(kernel, size.x / World.THREADS, size.y / World.THREADS, size.z / World.THREADS);

        float[] points = new float[size.x * size.y * size.z];
        buffer.GetData(points);

        buffer.Release();

        return points;
    }

    private float[] SimplexPlane(Vector3Int size, Vector3 pos, NoiseProfile profile, float height)
    {
        int kernel = shader_.FindKernel("SimplexPlane");
        
        ComputeBuffer buffer = new ComputeBuffer(size.x * size.y * size.z, sizeof(float));

        shader_.SetBuffer(kernel, "points_", buffer);
        SetUniforms(size, pos, profile);
        shader_.SetFloat("floor_", height);

        Vector2 lerp = GetLerp(profile);
        shader_.SetFloats("lerp_", lerp.x, lerp.y);

        shader_.Dispatch(kernel, size.x / World.THREADS, size.y / World.THREADS, size.z / World.THREADS);

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

    private void SetUniforms(Vector3Int size, Vector3 pos, NoiseProfile profile)
    {
        shader_.SetInts("size_", size.x, size.y, size.z);
        shader_.SetFloats("offset_", pos.x, pos.y, pos.z);
        shader_.SetFloats("scale_", profile.scale_.x, profile.scale_.y, profile.scale_.z);
        shader_.SetInt("octaves_", profile.octaves_);
        shader_.SetFloat("persistance_", profile.persistance_);
        shader_.SetFloat("lacunarity_", profile.lacunarity_);
        shader_.SetFloats("sampleOffset_", profile.sampleOffset_.x, profile.sampleOffset_.y, profile.sampleOffset_.z);
        shader_.SetFloats("params_", profile.params_.a_, profile.params_.b_, profile.params_.c_, profile.params_.d_, profile.params_.e_);
    }
}
