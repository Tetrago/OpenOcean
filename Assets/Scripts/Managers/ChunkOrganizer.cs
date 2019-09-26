using UnityEngine;

public abstract class ChunkOrganizer
{
    public enum Type
    {
        Static,
        Dynamic
    }

    public abstract void Allocate();
    public abstract void Generate(Vector3 origin);
    public abstract void Build();
    public abstract void Draw(Material material);
}
