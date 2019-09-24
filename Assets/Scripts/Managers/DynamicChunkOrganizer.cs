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
        Vector3 sub = settings_.submarine_.transform.position;
        sub.x /= Mathf.Ceil(World.instance_.size_.x);
        sub.y /= Mathf.Ceil(World.instance_.size_.y);
        sub.z /= Mathf.Ceil(World.instance_.size_.z);
        sub = Vector3.Scale(sub, World.instance_.size_);

        for(float x = -settings_.viewDist_; x < settings_.viewDist_; ++x)
        {
            for(float y = -settings_.viewDist_; y < settings_.viewDist_; ++y)
            {
                for(float z = -settings_.viewDist_; z < settings_.viewDist_; ++z)
                {
                    Vector3 pos = sub + Vector3.Scale(new Vector3(x, y, z), new Vector3(World.instance_.size_.x - 1, World.instance_.size_.y - 1, World.instance_.size_.z - 1));
                    if(!chunks_.ContainsKey(pos) && Vector3.Distance(settings_.submarine_.transform.position, pos) < settings_.viewDist_ * World.instance_.size_.magnitude)
                        Create(pos);
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

    public override void Generate(Noise noise, Vector3 origin)
    {
        List<Chunk> chunks = new List<Chunk>(ungeneratedChunks_);
        chunks.ForEach((Chunk chunk) =>
        {
            chunk.Generate(noise, World.instance_.noiseProfile_);
            ungeneratedChunks_.Remove(chunk);
        });
    }

    public override void Build(Marcher marcher, Feature feature)
    {
        List<Chunk> chunks = new List<Chunk>(unbuiltChunks_);
        chunks.ForEach((Chunk chunk) =>
        {
            chunk.Build(marcher, feature, World.instance_.size_, World.instance_.threshold_, World.instance_.step_, World.instance_.featureProfile_);
            unbuiltChunks_.Remove(chunk);
        });
    }

    public override void Draw(Material material)
    {
        foreach(Chunk chunk in chunks_.Values)
        {
            if(Vector3.Distance(chunk.Position, settings_.submarine_.transform.position) < settings_.viewDist_ * World.instance_.size_.magnitude)
                Graphics.DrawMesh(chunk.Mesh, chunk.Position, Quaternion.identity, material, 0);
        }
    }

    public override void Update(Noise noise, Marcher marcher, Feature feature)
    {
        base.Update(noise, marcher, feature);

        if(Time.time - lastUpdate_ >= settings_.updateTime_)
        {
            Allocate();
            Clean();

            Generate(noise, Vector3.zero);
            Build(marcher, feature);

            lastUpdate_ = Time.time;
        }
    }

    private void Clean()
    {
        Dictionary<Vector3, Chunk> chunks = new Dictionary<Vector3, Chunk>(chunks_);

        foreach(Chunk chunk in chunks.Values)
        {
            if(Vector3.Distance(chunk.Position, settings_.submarine_.transform.position) >= settings_.viewDist_ * World.instance_.size_.magnitude)
            {
                ColliderManager.Destroy(chunk);
                chunks_.Remove(chunk.Position);
            }
        }
    }
}
