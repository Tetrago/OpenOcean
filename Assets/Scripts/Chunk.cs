using UnityEngine;

public class Chunk
{
    public float[] Points { get; private set; }

    private Vector3 pos;

    public Chunk(Vector3 pos)
    {
        this.pos = pos;
    }

    public void Generate(Noise noise, Vector3Int size)
    {
        Points = noise.Generate(size);
    }
}