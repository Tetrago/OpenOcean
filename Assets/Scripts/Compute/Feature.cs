using UnityEngine;

public class Feature : MonoBehaviour
{
    public ComputeShader shader_;

    private int kernel_;

    public struct Stack
    {
        public Vector3Int[] tops_;
    }

    private void Awake()
    {
        kernel_ = shader_.FindKernel("CSMain");
    }

    public Stack Features(Vector3Int size, float[] points, float threshold, FeatureProfile profile)
    {
        Stack fs = new Stack();

        ComputeBuffer pointsBuffer = new ComputeBuffer(points.Length, sizeof(float));
        ComputeBuffer topBuffer = new ComputeBuffer(size.x * size.y * size.z, sizeof(int) * 3, ComputeBufferType.Append);
        ComputeBuffer countBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);

        shader_.SetInts("size_", size.x, size.y, size.z);
        shader_.SetFloat("threshold_", threshold);

        pointsBuffer.SetData(points);

        shader_.SetBuffer(kernel_, "points_", pointsBuffer);
        shader_.SetBuffer(kernel_, "tops_", topBuffer);

        topBuffer.SetCounterValue(0u);

        shader_.Dispatch(kernel_, size.x / World.THREADS, size.y / World.THREADS, size.z / World.THREADS);

        ComputeBuffer.CopyCount(topBuffer, countBuffer, 0);

        int[] countArray = { 0 };
        countBuffer.GetData(countArray);

        if(countArray[0u] > 0)
        {
            Vector3Int[] tops = new Vector3Int[countArray[0u]];
            topBuffer.GetData(tops);

            fs.tops_ = tops;
        }

        countBuffer.Release();
        topBuffer.Release();
        pointsBuffer.Release();

        return fs;
    }
}
