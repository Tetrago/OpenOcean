using System;
using System.Linq;
using UnityEngine;

public static class MeshGenerator
{
    public static void Build(ref Mesh mesh, Marcher.Triangle[] triangles)
    {
        Vector3[] vertices = new Vector3[triangles.Length * 3];
        int[] indices = new int[triangles.Length * 3];

        uint v = 0u;
        uint t = 0u;

        foreach(Marcher.Triangle tri in triangles)
        {
            vertices[v] = tri.a_;
            vertices[v + 1u] = tri.b_;
            vertices[v + 2u] = tri.c_;

            indices[t] = (int)v;
            indices[t + 1u] = (int)v + 1;
            indices[t + 2u] = (int)v + 2;

            v += 3;
            t += 3;
        }

        Buffer.BlockCopy(triangles, 0, mesh.vertices, 0, triangles.Length * 3 * 3);
        mesh.triangles = Enumerable.Range(0, triangles.Length * 3).Cast<int>().ToArray();
    }
}
