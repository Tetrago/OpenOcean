using UnityEngine;

public abstract class ChunkOrganizer
{
    public enum Type
    {
        Static,
        Dynamic
    }

    public abstract void Allocate();
    public abstract void Generate(Noise noise, Vector3 origin);
    public abstract void Build(Marcher marcher, Feature feature);
    public abstract void Draw(Material material);
}
