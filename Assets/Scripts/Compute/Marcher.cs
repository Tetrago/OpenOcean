using System;
using System.Collections;
using UnityEngine;

public class Marcher
{
    private ComputeShader shader_;
    private int kernel_;

    private Func<bool> process_;
    private Triangle[] triangles_;
    private float[] local_;
    private bool isAlive_;
    private bool ready_;

    public struct Triangle
    {
        public Vector3 a_, b_, c_;
    }

    public Marcher()
    {
        shader_ = Compute.Marcher;
        kernel_ = shader_.FindKernel("CSMain");

        process_ = ShaderProcess;
        isAlive_ = false;
        ready_ = false;
    }

    public void Triangulate(float[] points)
    {
        local_ = points;
        Coroutine.Instance.StartCoroutine(MarcherCo());
    }

    private bool ShaderProcess()
    {
        if(isAlive_) return false;
        isAlive_ = true;

        Vector3Int size = World.instance_.size_;

        ComputeBuffer pointsBuffer = new ComputeBuffer(local_.Length, sizeof(float));
        ComputeBuffer trianglesBuffer = new ComputeBuffer(size.x * size.y * size.z * 5, sizeof(float) * 3 * 3, ComputeBufferType.Append);
        ComputeBuffer countBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);

        shader_.SetInts("size_", size.x, size.y, size.z);
        shader_.SetFloat("threshold_", World.instance_.threshold_);
        shader_.SetFloat("step_", World.instance_.step_);

        pointsBuffer.SetData(local_);

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

        isAlive_ = false;
        triangles_ = triangles;

        return true;
    }

    IEnumerator MarcherCo()
    {
        ready_ = false;
        yield return new WaitUntil(ShaderProcess);
        ready_ = true;
    }

    public Triangle[] Triangles => triangles_;
    public bool Ready => ready_;
}
