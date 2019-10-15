using UnityEngine;

public static class MeshGenerator
{
    public static MeshManager.Definition Build(Marcher.Triangle[] triangles)
    {
        Vector3[] vectors = new Vector3[triangles.Length * 3];
        int[] tris = new int[triangles.Length * 3];

        for(int i = 0, j = 0; j < triangles.Length; ++j, i += 3)
        {
            vectors[i] = triangles[j].a_;
            vectors[i + 1] = triangles[j].b_;
            vectors[i + 2] = triangles[j].c_;

            tris[i] = i;
            tris[i + 1] = i + 1;
            tris[i + 2] = i + 2;
        }

        MeshManager.Definition definition = new MeshManager.Definition
        {
            vertices_ = vectors,
            triangles_ = tris
        };

        return definition;
    }
}
