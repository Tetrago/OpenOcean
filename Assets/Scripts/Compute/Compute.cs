using UnityEngine;

public static class Compute
{
    public static ComputeShader Marcher { get { return (ComputeShader)Resources.Load("Marcher"); } }
    public static ComputeShader Noise { get { return (ComputeShader)Resources.Load("Noise"); } }
    public static ComputeShader Feature { get { return (ComputeShader)Resources.Load("Feature"); } }
}
