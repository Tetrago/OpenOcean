using System.Collections.Generic;
using UnityEngine;

public class DynamicChunkOrganizer : ChunkOrganizer
{
    [System.Serializable]
    public struct Settings
    {
        public GameObject submarine_;
        public float viewDist_;
        public float updateTime_;
    }

    private Dictionary<Vector3, Chunk> chunks_;
    private Settings settings_;
    private float lastUpdate_;

    public DynamicChunkOrganizer(Settings settings)
    {
        settings_ = settings;
        chunks_ = new Dictionary<Vector3, Chunk>();
        lastUpdate_ = Time.time;
    }

    public override void Allocate()
    {
        Vector3 sub = settings_.submarine_.transform.position;
        sub.x -= sub.x % World.instance_.size_.x;
        sub.y -= sub.y % World.instance_.size_.y;
        sub.z -= sub.z % World.instance_.size_.z;

        for(float x = -settings_.viewDist_; x < settings_.viewDist_; ++x)
        {
            for(float y = -settings_.viewDist_; y < settings_.viewDist_; ++y)
            {
                for(float z = -settings_.viewDist_; z < settings_.viewDist_; ++z)
                {
                    Vector3 pos = sub + Vector3.Scale(new Vector3(x, y, z), new Vector3(World.instance_.size_.x - 1, World.instance_.size_.y - 1, World.instance_.size_.z - 1));
                    if(Vector3.Distance(settings_.submarine_.transform.position, pos) < settings_.viewDist_ * World.instance_.size_.magnitude && !chunks_.ContainsKey(pos))
                    {
                        chunks_.Add(pos, new Chunk(pos));
                    }
                }
            }
        }
    }

    public override void Generate(Noise noise, Vector3 origin)
    {
        List<Chunk> chunks = GetChunks(settings_.submarine_.transform.position);
        foreach(Chunk chunk in chunks)
            chunk.Generate(noise, World.instance_.noiseProfile_);
    }

    public override void Build(Marcher marcher, Feature feature)
    {
        List<Chunk> chunks = GetChunks(settings_.submarine_.transform.position);
        foreach(Chunk chunk in chunks)
            chunk.Build(marcher, feature, World.instance_.size_, World.instance_.threshold_, World.instance_.step_, World.instance_.featureProfile_);
    }

    public override void Draw(Material material)
    {
        List<Chunk> chunks = GetChunks(settings_.submarine_.transform.position);
        foreach(Chunk chunk in chunks)
            Graphics.DrawMesh(chunk.Mesh, chunk.Position, Quaternion.identity, material, 0);
    }

    public override void Update(Noise noise, Marcher marcher, Feature feature)
    {
        base.Update(noise, marcher, feature);

        if(Time.time - lastUpdate_ >= settings_.updateTime_)
        {
            Allocate();
            Clean(settings_.submarine_.transform.position);

            Generate(noise, Vector3.zero);
            Build(marcher, feature);

            lastUpdate_ = Time.time;
        }
    }

    private List<Chunk> GetChunks(Vector3 position)
    {
        List<Chunk> chunks = new List<Chunk>();

        foreach(Chunk chunk in chunks_.Values)
        {
            if(Vector3.Distance(chunk.Position, position) < settings_.viewDist_ * World.instance_.size_.magnitude)
                chunks.Add(chunk);
        }

        return chunks;
    }

    private void Clean(Vector3 position)
    {
        foreach(var item in new Dictionary<Vector3, Chunk>(chunks_))
        {
            if(Vector3.Distance(item.Value.Position, position) >= settings_.viewDist_)
            {
                ColliderManager.Destroy(item.Value);
                chunks_.Remove(item.Key);
            }
        }
    }
}
