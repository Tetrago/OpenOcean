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

    public void Generate(Noise noise, NoiseProfile noiseProfile)
    {
        points_ = noise.Generate(World.instance_.size_, pos_, noiseProfile);
    }

    public void Build(Marcher marcher, Feature feature, Vector3Int size, float threshold, float step, FeatureProfile profile)
    {
        stack_ = World.instance_.Feature.Features(World.instance_.size_, points_, World.instance_.threshold_, World.instance_.featureProfile_);
        MeshGenerator.Build(ref mesh_, World.instance_.Marcher.Triangulate(World.instance_.size_, points_, World.instance_.threshold_, World.instance_.step_));
        mesh_.RecalculateNormals();
        ColliderManager.Collider(this);
    }

    public float[] Points => points_;
    public Vector3 Position => pos_;
    public Mesh Mesh => mesh_;
    public Feature.Stack Features => stack_;
}
