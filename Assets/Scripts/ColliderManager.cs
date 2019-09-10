using System.Collections.Generic;
using UnityEngine;

public static class ColliderManager
{
    private static Dictionary<Chunk, MeshCollider> colliders_ = new Dictionary<Chunk, MeshCollider>();
    private static GameObject parent_;
    private static uint id_;

    public static void Init()
    {
        id_ = 0u;
        parent_ = new GameObject("Colliders");
    }

    public static void Collider(Chunk chunk)
    {
        if(!colliders_.ContainsKey(chunk))
            colliders_.Add(chunk, Create(chunk.Position));

        colliders_[chunk].sharedMesh = chunk.Mesh;
    }

    private static MeshCollider Create(Vector3 pos)
    {
        GameObject go = new GameObject("" + id_++);
        go.transform.position = pos;
        go.transform.parent = parent_.transform;
        return go.AddComponent<MeshCollider>();
    }
}