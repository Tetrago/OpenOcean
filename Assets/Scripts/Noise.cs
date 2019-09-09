using UnityEngine;

public class Noise : MonoBehaviour
{
    public ComputeShader shader;

    public float[] Generate(Vector3Int size)
    {
        ComputeBuffer buffer = new ComputeBuffer(size.x * size.y * size.z, sizeof(float));

        int kernel = shader.FindKernel("CSMain");
        shader.SetBuffer(kernel, "points", buffer);
        shader.SetInts("size", size.x, size.y, size.z);
        shader.Dispatch(kernel, size.x / World.NUM_COMPUTE_THREADS, size.y / World.NUM_COMPUTE_THREADS, size.x / World.NUM_COMPUTE_THREADS);

        float[] points = new float[size.x * size.y * size.z];
        buffer.GetData(points);

        buffer.Release();

        return points;
    }
}