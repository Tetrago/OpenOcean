using UnityEngine;

public class Chunk
{
    private float[] points_;
    private Vector3 pos_;
    private Mesh mesh_;

    public Chunk(Vector3 pos)
    {
        pos_ = pos;

        mesh_ = new Mesh();
        mesh_.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
    }

    public void Generate(Noise noise, Vector3Int size, NoiseProfile profile)
    {
        points_ = noise.Generate(size, profile);
    }

    public void Build(Marcher marcher, Vector3Int size, float threshold, float step)
    {
        MeshGenerator.Build(ref mesh_, marcher.Triangulate(size, points_, threshold, step));
        mesh_.RecalculateNormals();
    }

    public Mesh Mesh => mesh_;
}
