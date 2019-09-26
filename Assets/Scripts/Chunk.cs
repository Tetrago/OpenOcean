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

    private bool ready_;

    public Chunk(Vector3 pos)
    {
        pos_ = pos;

        mesh_ = new Mesh();
        mesh_.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        marcher_ = new Marcher();
        noise_ = new Noise();
        feature_ = new Feature();

        ready_ = false;
    }

    public void Generate(NoiseProfile noiseProfile)
    {
        points_ = noise_.Generate(World.instance_.size_, pos_, noiseProfile);
    }

    private bool IsReady()
    {
        return marcher_.Ready;
    }

    public void Build()
    {
        Coroutine.Instance.StartCoroutine(BuildProcess());
    }

    IEnumerator BuildProcess()
    {
        ready_ = false;

        marcher_.Triangulate(points_);
        stack_ = feature_.Features(World.instance_.size_, points_, World.instance_.threshold_, World.instance_.featureProfile_);

        yield return new WaitUntil(IsReady);

        MeshGenerator.Build(ref mesh_, marcher_.Triangles);
        mesh_.RecalculateNormals();
        ColliderManager.Collider(this);

        ready_ = true;
    }

    public void Draw(Material material)
    {
        if(!ready_) return;
        Graphics.DrawMesh(mesh_, pos_, Quaternion.identity, material, 0);
    }

    public float[] Points => points_;
    public Vector3 Position => pos_;
    public Mesh Mesh => mesh_;
    public Feature.Stack Features => stack_;
}
