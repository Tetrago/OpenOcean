using UnityEngine;

public class StaticChunkOrganizer : ChunkOrganizer
{
    [System.Serializable]
    public struct Settings
    {
        public Vector3Int worldSize_;
    }

    public override void Generate(ref Chunk[,,] chunks, Noise noise, Vector3 origin)
    {
        Vector3Int worldSize = World.instance_.staticSettings_.worldSize_;
        Vector3Int size = World.instance_.size_;
        float step = World.instance_.step_;

        for(int x = 0; x < worldSize.x; ++x)
        {
            for(int y = 0; y < worldSize.y; ++y)
            {
                for(int z = 0; z < worldSize.z; ++z)
                {
                    Chunk chunk = new Chunk(origin + new Vector3(x * (size.x - 1) * step, y * (size.y - 1) * step, z * (size.z - 1) * step));
                    chunk.Generate(noise, World.instance_.noiseProfile_);
                    chunks[x, y, z] = chunk;
                }
            }
        }
    }

    public override void Build(ref Chunk[,,] chunks, Marcher marcher, Feature feature)
    {
        foreach(Chunk chunk in chunks)
            chunk.Build(marcher, feature, World.instance_.size_, World.instance_.threshold_, World.instance_.step_, World.instance_.featureProfile_);
    }
}
