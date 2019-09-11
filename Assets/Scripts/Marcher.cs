using UnityEngine;

public class Marcher : MonoBehaviour
{
    public ComputeShader shader_;

    private int kernel_;

    public struct Triangle
    {
        public Vector3 a_, b_, c_;
    }

    private void Awake()
    {
        kernel_ = shader_.FindKernel("CSMain");
    }

    public Triangle[] Triangulate(Vector3Int size, float[] points, float iso, float threshold, float step)
    {
        ComputeBuffer pointsBuffer = new ComputeBuffer(points.Length, sizeof(float));
        ComputeBuffer trianglesBuffer = new ComputeBuffer(size.x * size.y * size.z * 5, sizeof(float) * 3 * 3, ComputeBufferType.Append);
        ComputeBuffer countBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);

        shader_.SetInts("size_", size.x, size.y, size.z);
        shader_.SetFloat("iso_", iso);
        shader_.SetFloat("threshold_", threshold);
        shader_.SetFloat("step_", step);

        pointsBuffer.SetData(points);

        shader_.SetBuffer(kernel_, "triangles_", trianglesBuffer);
        shader_.SetBuffer(kernel_, "points_", pointsBuffer);

        trianglesBuffer.SetCounterValue(0u);

        shader_.Dispatch(kernel_, size.x / World.THREADS, size.y / World.THREADS, size.z / World.THREADS);

        ComputeBuffer.CopyCount(trianglesBuffer, countBuffer, 0);
        int[] countArray = { 0 };
        countBuffer.GetData(countArray);

        Triangle[] triangles = new Triangle[countArray[0u]];
        trianglesBuffer.GetData(triangles);

        countBuffer.Release();
        trianglesBuffer.Release();
        pointsBuffer.Release();

        return triangles;
    }
}
