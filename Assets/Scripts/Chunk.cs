using System.Collections;
using UnityEngine;

public class Chunk
{
    private float[] points_;
    private Vector3 pos_;
    private Mesh mesh_;
    private Feature.Stack stack_;

    private Marcher marcher_;
    private Noise noise_;
    private Feature feature_;

    public Chunk(Vector3 pos)
    {
        pos_ = pos;

        mesh_ = new Mesh();
        mesh_.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        marcher_ = new Marcher();
        noise_ = new Noise();
        feature_ = new Feature();
    }

    public void Generate(NoiseProfile noiseProfile)
    {
        points_ = noise_.Generate(World.instance_.size_, pos_, noiseProfile);
    }

    public void Build()
    {
        marcher_.Triangulate(points_);
        stack_ = feature_.Features(World.instance_.size_, points_, World.instance_.threshold_, World.instance_.featureProfile_);
        MeshGenerator.Build(ref mesh_, marcher_.Triangles);
        Feature.Populate(this);
        mesh_.RecalculateNormals();
        ColliderManager.Collider(this);
    }

    public void Draw(Material material)
    {
        Graphics.DrawMesh(mesh_, pos_, Quaternion.identity, material, 0);
    }

    public float[] Points => points_;
    public Vector3 Position => pos_;
    public Mesh Mesh => mesh_;
    public Feature.Stack Features => stack_;
}
