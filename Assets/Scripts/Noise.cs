using UnityEngine;

public class Noise : MonoBehaviour
{
    public ComputeShader shader_;

    private int kernel_;

    private void Awake()
    {
        kernel_ = shader_.FindKernel("CSMain");
    }

    public float[] Generate(Vector3Int size)
    {
        ComputeBuffer buffer = new ComputeBuffer(size.x * size.y * size.z, sizeof(float));

        shader_.SetBuffer(kernel_, "points_", buffer);
        shader_.SetInts("size_", size.x, size.y, size.z);
        shader_.Dispatch(kernel_, size.x / World.THREADS, size.y / World.THREADS, size.x / World.THREADS);

        float[] points = new float[size.x * size.y * size.z];
        buffer.GetData(points);

        buffer.Release();

        return points;
    }
}
