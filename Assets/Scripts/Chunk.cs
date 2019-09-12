using UnityEngine;

public class Chunk
{
    private float[] points_;
    private Vector3 pos_;
    private Mesh mesh_;
    private Feature.Stack stack_;

    public Chunk(Vector3 pos)
    {
        pos_ = pos;

        mesh_ = new Mesh();
        mesh_.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
    }

    public void Generate(Noise noise, GenerationProfile generation)
    {
        points_ = DetermineTerrain(noise, World.instance_.size_, generation, pos_);
    }

    public void Build(Marcher marcher, Feature feature, Vector3Int size, float threshold, float step, FeatureProfile profile)
    {
        stack_ = feature.Features(size, points_, threshold, profile);
        MeshGenerator.Build(ref mesh_, marcher.Triangulate(size, points_, threshold, step));
        mesh_.RecalculateNormals();
        ColliderManager.Collider(this);
    }

    private static float[] DetermineTerrain(Noise noise, Vector3Int size, GenerationProfile generation, Vector3 pos)
    {
        float[] final = new float[size.x * size.y * size.z];

        for(uint i = 0u; i < generation.levels_.Length; ++i)
        {
            GenerationProfile.Level level = generation.levels_[i];
            float height = level.height_;

            float[] points = noise.Generate(size, pos, level.noiseProfile_, generation.levels_[i].type_);

            for(float x = 0; x < size.x; ++x)
            {
                for(float y = 0; y < size.y; ++y)
                {
                    for(float z = 0; z < size.z; ++z)
                    {
                        float blendFactor = y < height ? 0.0f : 1.0f;
                        final[(int)(x + size.y * (y + size.x * z))] = points[(int)(x + size.y * (y + size.x * z))] * blendFactor;
                    }
                }
            }
        }

        return final;
    }

    public Vector3 Position => pos_;
    public Mesh Mesh => mesh_;
    public Feature.Stack Features => stack_;
}
