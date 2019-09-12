using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public class Comparer : IComparer<GenerationProfile.Level>
    {
        public int Compare(GenerationProfile.Level a, GenerationProfile.Level b)
        {
            if(a.height_ > b.height_)
                return 1;
            else if(a.height_ < b.height_)
                return -1;
            else
                return 0;
        }
    }

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
        GenerationProfile.Level[] levels = generation.levels_;
        Array.Sort(generation.levels_, new Comparer());

        float[] final = new float[size.x * size.y * size.z];

        for(uint i = 0u; i < levels.Length; ++i)
        {
            GenerationProfile.Level level = levels[i];
            float height = level.height_;

            float[] points = noise.Generate(size, pos, level.noiseProfile_, level);

            for(float x = 0; x < size.x; ++x)
            {
                for(float y = 0; y < size.y; ++y)
                {
                    for(float z = 0; z < size.z; ++z)
                    {
                        float delta = pos.y + y - height;
                        if(delta >= 0)
                        {
                            float blendFactor = level.blendDist_ == 0 ? 1.0f : Mathf.InverseLerp(0, level.blendDist_, Mathf.Min(delta, level.blendDist_));
                            final[(int)(x + size.y * (y + size.x * z))] += points[(int)(x + size.y * (y + size.x * z))] * blendFactor - final[(int)(x + size.y * (y + size.x * z))] * blendFactor;
                        }
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
