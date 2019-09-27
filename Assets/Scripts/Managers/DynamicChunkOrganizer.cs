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
    private List<Chunk> ungeneratedChunks_;
    private List<Chunk> unbuiltChunks_;
    private Settings settings_;
    private float lastUpdate_;

    public DynamicChunkOrganizer(Settings settings)
    {
        chunks_ = new Dictionary<Vector3, Chunk>();
        ungeneratedChunks_ = new List<Chunk>();
        unbuiltChunks_ = new List<Chunk>();
        settings_ = settings;
        lastUpdate_ = Time.time;
    }

    public override void Allocate()
    {
        Vector3 sizeMinusOne = new Vector3(World.instance_.size_.x - 1, World.instance_.size_.y - 1, World.instance_.size_.z - 1);

        Vector3 sub = settings_.submarine_.transform.position;
        sub.x = Mathf.Ceil(sub.x / (World.instance_.size_.x - 1));
        sub.y = Mathf.Ceil(sub.y / (World.instance_.size_.y - 1));
        sub.z = Mathf.Ceil(sub.z / (World.instance_.size_.z - 1));
        sub = Vector3.Scale(sub, sizeMinusOne);

        for(float x = -settings_.viewDist_; x < settings_.viewDist_; ++x)
        {
            for(float y = -settings_.viewDist_; y < settings_.viewDist_; ++y)
            {
                for(float z = -settings_.viewDist_; z < settings_.viewDist_; ++z)
                {
                    Vector3 pos = sub + Vector3.Scale(new Vector3(x, y, z), sizeMinusOne);
                    if(!chunks_.ContainsKey(pos) && Vector3.Distance(settings_.submarine_.transform.position, pos) < settings_.viewDist_ * World.instance_.size_.magnitude)
                    {
                        Create(pos);
                    }
                }
            }
        }
    }

    private void Create(Vector3 pos)
    {
        Chunk chunk = new Chunk(pos);

        chunks_.Add(pos, chunk);
        ungeneratedChunks_.Add(chunk);
        unbuiltChunks_.Add(chunk);
    }

    public override void Generate(Vector3 origin)
    {
        ungeneratedChunks_.ForEach((Chunk chunk) =>
        {
            chunk.Generate(World.instance_.noiseProfile_);
        });

        ungeneratedChunks_.Clear();
    }

    public override void Build()
    {
        unbuiltChunks_.ForEach((Chunk chunk) =>
        {
            chunk.Build();
        });

        unbuiltChunks_.Clear();
    }

    public override void Draw(Material material)
    {
        if(Time.time - lastUpdate_ >= settings_.updateTime_)
        {
            Allocate();
            Clean();

            Generate(Vector3.zero);
            Build();

            lastUpdate_ = Time.time;
        }

        foreach(Chunk chunk in chunks_.Values)
        {
            if(Vector3.Distance(chunk.Position, settings_.submarine_.transform.position) < settings_.viewDist_ * World.instance_.size_.magnitude)
                chunk.Draw(material);
        }
    }

    private void Clean()
    {
        List<Chunk> chunks = new List<Chunk>(chunks_.Values);
        chunks.ForEach((Chunk chunk) =>
        {
            if(Vector3.Distance(chunk.Position, settings_.submarine_.transform.position) >= settings_.viewDist_ * World.instance_.size_.magnitude)
            {
                ColliderManager.Destroy(chunk);
                chunks_.Remove(chunk.Position);
            }
        });
    }
}
