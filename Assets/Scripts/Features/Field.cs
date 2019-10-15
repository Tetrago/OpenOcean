using UnityEngine;

public class Field : MonoBehaviour
{
    public static Field instance_;

    public GameObject prefab_;

    public const float PLANES_PER_CHUNK_AXIS = 8;

    private void Awake()
    {
        instance_ = this;
    }

    public Mesh FlatPlane(Vector3 size)
    {
        Mesh mesh = new Mesh();

        mesh.vertices = new Vector3[]
        {
            new Vector3(0, 0, 0), new Vector3(size.x, 0, 0),
            new Vector3(size.x, 0, size.z), new Vector3(0, 0, size.z)
        };

        mesh.triangles = new int[]
        {
            0, 2, 1,
            0, 3, 2
        };

        mesh.RecalculateNormals();
        return mesh;
    }

    public void Spawn(Chunk chunk)
    {
        if(chunk.Position.y != 0) return;
        Vector3 chunkSize = World.instance_.size_;

        for(uint x = 0u; x < PLANES_PER_CHUNK_AXIS; ++x)
        {
            for(uint z = 0u; z < PLANES_PER_CHUNK_AXIS; ++z)
            {
                Vector3 pos = chunk.Position + Vector3.up * World.instance_.grassHeight_ + new Vector3(x / PLANES_PER_CHUNK_AXIS * chunkSize.x, 0, z / PLANES_PER_CHUNK_AXIS * chunkSize.z);
                Vector3 size = chunkSize / PLANES_PER_CHUNK_AXIS;

                GameObject go = Instantiate(prefab_, pos, Quaternion.identity, World.GetParent(chunk));
                go.GetComponent<MeshFilter>().mesh = FlatPlane(size);
            }
        }
    }
}
