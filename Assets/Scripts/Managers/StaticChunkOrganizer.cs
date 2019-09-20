﻿using UnityEngine;

public class StaticChunkOrganizer : ChunkOrganizer
{
    [System.Serializable]
    public struct Settings
    {
        public Vector3Int worldSize_;
    }

    private Chunk[,,] chunks_;
    private Settings settings_;

    public StaticChunkOrganizer(Settings settings)
    {
        settings_ = settings;
    }

    public override void Allocate()
    {
        chunks_ = new Chunk[settings_.worldSize_.x, settings_.worldSize_.y, settings_.worldSize_.z];
    }

    public override void Generate(Noise noise, Vector3 origin)
    {
        Vector3Int worldSize = World.instance_.staticSettings_.worldSize_;
        Vector3Int size = World.instance_.size_;
        float step = World.instance_.step_;

        for(int x = 0; x < worldSize.x; ++x)
        {
            for(int y = 0; y < worldSize.y; ++y)
            {
                for(int z = 0; z < worldSize.z; ++z)
                {
                    Chunk chunk = new Chunk(origin + new Vector3(x * (size.x - 1) * step, y * (size.y - 1) * step, z * (size.z - 1) * step));
                    chunk.Generate(noise, World.instance_.noiseProfile_);
                    chunks_[x, y, z] = chunk;
                }
            }
        }
    }

    public override void Build(Marcher marcher, Feature feature)
    {
        foreach(Chunk chunk in chunks_)
            chunk.Build(marcher, feature, World.instance_.size_, World.instance_.threshold_, World.instance_.step_, World.instance_.featureProfile_);
    }

    public override void Draw(Material material)
    {
        foreach(Chunk chunk in chunks_)
            Graphics.DrawMesh(chunk.Mesh, chunk.Position, Quaternion.identity, material, 0);
    }
}