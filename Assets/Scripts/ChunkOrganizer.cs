using UnityEngine;

public abstract class ChunkOrganizer
{
    public enum Type
    {
        Static,
        Dynamic
    }

    public abstract void Generate(ref Chunk[,,] chunks, Noise noise, Vector3 origin);
    public abstract void Build(ref Chunk[,,] chunks, Marcher marcher, Feature feature);
}
