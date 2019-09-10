using UnityEngine;

public class Feature : MonoBehaviour
{
    public ComputeShader shader_;

    private int kernel_;

    public struct Line
    {
        public Vector3Int index_;
        public float height_;
    }

    public struct Stack
    {
        public Line[] lines_;
    }

    private void Awake()
    {
        kernel_ = shader_.FindKernel("CSMain");
    }

    public Stack Features(Vector3Int size, float[] points, float threshold, FeatureProfile profile)
    {
        Stack fs = new Stack();

        ComputeBuffer pointsBuffer = new ComputeBuffer(points.Length, sizeof(float));
        ComputeBuffer lineBuffer = new ComputeBuffer(size.x * size.y * size.z, sizeof(int) * 3 + sizeof(float), ComputeBufferType.Append);
        ComputeBuffer countBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);

        shader_.SetInts("size_", size.x, size.y, size.z);
        shader_.SetFloat("threshold_", threshold);
        shader_.SetFloat("lineCutoff_", profile.lineCutoff_);

        pointsBuffer.SetData(points);

        shader_.SetBuffer(kernel_, "points_", pointsBuffer);
        shader_.SetBuffer(kernel_, "lines_", lineBuffer);

        lineBuffer.SetCounterValue(0u);

        shader_.Dispatch(kernel_, size.x / World.THREADS, size.y / World.THREADS, size.z / World.THREADS);

        ComputeBuffer.CopyCount(lineBuffer, countBuffer, 0);

        int[] countArray = { 0 };
        countBuffer.GetData(countArray);

        if(countArray[0u] > 0)
        {
            Line[] lines = new Line[countArray[0u]];
            lineBuffer.GetData(lines);

            fs.lines_ = lines;
        }

        countBuffer.Release();
        lineBuffer.Release();
        pointsBuffer.Release();

        return fs;
    }
}
