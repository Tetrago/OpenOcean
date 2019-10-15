using System.Threading;
using System.Collections;
using UnityEngine;

public class MeshManager
{
    public struct Definition
    {
        public Vector3[] vertices_;
        public int[] triangles_;
    }

    private Mesh mesh_;
    private bool ready_;
    private Thread thread_;
    private Definition definition_;

    public MeshManager()
    {
        mesh_ = new Mesh();
        mesh_.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        ready_ = false;
    }

    public void Build(Marcher.Triangle[] triangles)
    {
        ready_ = false;
        thread_ = new Thread(() => ThreadFunc(triangles));
        thread_.Start();
        Coroutine.Instance.StartCoroutine(MeshEnumerator());
    }

    private void ThreadFunc(Marcher.Triangle[] triangles)
    {
        definition_ = MeshGenerator.Build(triangles);
        ready_ = true;
    }

    private IEnumerator MeshEnumerator()
    {
        while(!ready_) yield return null;
        Upload();
    }

    private void Upload()
    {
        ready_ = true;

        mesh_.vertices = definition_.vertices_;
        mesh_.triangles = definition_.triangles_;
        mesh_.RecalculateNormals();
    }

    public void Draw(Vector3 position, Material mat)
    {
        if(ready_)
            Graphics.DrawMesh(mesh_, position, Quaternion.identity, mat, 0);
    }
}
