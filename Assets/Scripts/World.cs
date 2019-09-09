using UnityEngine;

[RequireComponent(typeof(Noise))]
public class World : MonoBehaviour
{
    public const int NUM_COMPUTE_THREADS = 8;

    public Vector3Int size;
    [Range(-1, 1)] public float threshold;

    private Noise noise;
    private Chunk chunk;

    private void Awake()
    {
        noise = GetComponent<Noise>();
    }

    private void Start()
    {
        chunk = new Chunk(Vector3.zero);
        chunk.Generate(noise, size);
    }

    private void OnDrawGizmos()
    {
        if(chunk == null) return;

        for(int x = 0; x < size.x; ++x)
        {
            for(int y = 0; y < size.y; ++y)
            {
                for(int z = 0; z < size.z; ++z)
                {
                    float value = Mathf.InverseLerp(-1, 1, chunk.Points[x + size.y * (y + size.x * z)]);
                    Gizmos.color = new Color(value, value, value);
                    if(value >= threshold)
                        Gizmos.DrawSphere(new Vector3(x, y, z), 0.1f);
                }
            }
        }
    }

    private void OnValidate()
    {
        size.x -= size.x % NUM_COMPUTE_THREADS;
        size.y -= size.y % NUM_COMPUTE_THREADS;
        size.z -= size.z % NUM_COMPUTE_THREADS;
    }
}