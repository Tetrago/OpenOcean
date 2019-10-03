using UnityEngine;
using Debug = UnityEngine.Gizmos;

public class Chunk
{
    private float[] points_;
    private Vector3 pos_;
    private Mesh mesh_;

    private Marcher marcher_;
    private Noise noise_;

    public Chunk(Vector3 pos)
    {
        pos_ = pos;

        mesh_ = new Mesh();
        mesh_.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        marcher_ = new Marcher();
        noise_ = new Noise();
    }

    public void Generate(NoiseProfile noiseProfile)
    {
        points_ = noise_.Generate(World.instance_.size_, pos_, noiseProfile);
    }

    public void Build()
    {
        marcher_.Triangulate(points_);
        MeshGenerator.Build(ref mesh_, marcher_.Triangles);
        mesh_.RecalculateNormals();
        ColliderManager.Collider(this);
    }

    public void Draw(Material material)
    {
        Graphics.DrawMesh(mesh_, pos_, Quaternion.identity, material, 0);
    }

    public void Gizmos()
    {
        
    }

    public float[] Points => points_;
    public Vector3 Position => pos_;
    public Mesh Mesh => mesh_;
}
