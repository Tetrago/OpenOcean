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

    public float[] Generate(Vector3Int size, Vector3 pos, NoiseProfile profile)
    {
        int kernel = shader_.FindKernel("CSMain");

        ComputeBuffer buffer = new ComputeBuffer(size.x * size.y * size.z, sizeof(float));

        shader_.SetBuffer(kernel, "points_", buffer);
        shader_.SetInts("size_", size.x, size.y, size.z);
        shader_.SetFloats("offset_", pos.x, pos.y, pos.z);
        shader_.SetFloats("scale_", profile.scale_.x, profile.scale_.y, profile.scale_.z);
        shader_.SetInt("octaves_", profile.octaves_);
        shader_.SetFloat("persistance_", profile.persistance_);
        shader_.SetFloat("lacunarity_", profile.lacunarity_);
        shader_.SetFloats("sampleOffset_", profile.sampleOffset_.x, profile.sampleOffset_.y, profile.sampleOffset_.z);
        shader_.SetFloat("floor_", profile.floor_);
        shader_.SetFloat("roof_", profile.roof_);
        shader_.SetFloat("noiseWeight_", profile.noiseWeight_);
        shader_.SetFloat("floorWeight_", profile.floorWeight_);
        shader_.SetFloat("weightMultiplier_", profile.weightMultiplier_);
        shader_.SetVector("blend_", profile.blend_);

        Vector2 lerp = GetLerp(profile);
        shader_.SetFloats("lerp_", lerp.x, lerp.y);

        shader_.Dispatch(kernel, size.x / World.THREADS, size.y / World.THREADS, size.z / World.THREADS);

        float[] points = new float[size.x * size.y * size.z];
        buffer.GetData(points);
        buffer.Release();
        return points;
    }
}
