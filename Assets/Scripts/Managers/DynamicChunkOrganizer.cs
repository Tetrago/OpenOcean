using System.Collections.Generic;
using UnityEngine;

public class DynamicChunkOrganizer : ChunkOrganizer
{
    [System.Serializable]
    public struct Settings
    {
        public Vector3Int worldSize_;
        public GameObject submarine_;
        public float viewDist_;
    }

    private Dictionary<Vector3, Chunk> chunks_;
    private Settings settings_;

    public DynamicChunkOrganizer(Settings settings)
    {
        settings_ = settings;
    }

    public override void Allocate()
    {
        for(float x = -settings_.viewDist_; x < settings_.viewDist_; ++x)
        {
            for(float y = -settings_.viewDist_; y < settings_.viewDist_; ++y)
            {
                for(float z = -settings_.viewDist_; z < settings_.viewDist_; ++z)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    if(Vector3.Distance(settings_.submarine_.transform.position, pos) < settings_.viewDist_)
                    {
                        chunks_.Add(pos, new Chunk(pos));
                    }
                }
            }
        }
    }

    public override void Build(Marcher marcher, Feature feature)
    {
        
    }

    public override void Generate(Noise noise, Vector3 origin)
    {
        
    }

    public override void Draw(Material material)
    {
        List<Chunk> chunks = GetChunks(settings_.submarine_.transform.position);
        foreach(Chunk chunk in chunks)
            Graphics.DrawMesh(chunk.Mesh, chunk.Position, Quaternion.identity, material, 0);
    }

    private Chunk GetChunk(Vector3 position)
    {
        if(chunks_.ContainsKey(position))
        {
            return chunks_[position];
        }
        else
        {
            Chunk chunk = new Chunk(position);
            chunks_.Add(position, chunk);
            return chunk;
        }
    }

    private List<Chunk> GetChunks(Vector3 position)
    {
        List<Chunk> chunks = new List<Chunk>();

        foreach(Chunk chunk in chunks_.Values)
        {
            if(Vector3.Distance(chunk.Position, position) < settings_.viewDist_)
                chunks.Add(chunk);
        }

        return chunks;
    }

    private void Clean(Vector3 position)
    {
        foreach(var item in new Dictionary<Vector3, Chunk>(chunks_))
        {
            if(Vector3.Distance(item.Value.Position, position) >= settings_.viewDist_)
                chunks_.Remove(item.Key);
        }
    }
}
