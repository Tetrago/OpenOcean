using UnityEngine;

public class TerrainShaderAPI : MonoBehaviour
{
    public Material material_;

    public void Configure(Vector3Int size)
    {
        material_.SetVector("size_", new Vector3(size.x, size.y, size.z));
    }
}
