using UnityEngine;

public class Chunk
{
    private float[] points_;
    private Vector3 pos_;
    private MeshManager mesh_;

    private Marcher marcher_;
    private Noise noise_;

    public Chunk(Vector3 pos)
    {
        pos_ = pos;

        mesh_ = new MeshManager();

        marcher_ = new Marcher();
        noise_ = new Noise();
    }

    ~Chunk()
    {
        World.KillParent(this);
    }

    public void Generate(NoiseProfile noiseProfile)
    {
        points_ = noise_.Generate(World.instance_.size_, pos_, noiseProfile);
        Transform parent = World.GetParent(this);

        System.Random rand = WorldRand.Random;
        foreach(Spawnable spawn in World.instance_.spawnables_)
        {
            for(uint i = 0u; i < spawn.iterations_; ++i)
            {
                if(rand.Next((int)spawn.rarity_) == 1)
                {
                    Vector3 pos = pos_ + Random.insideUnitSphere * World.instance_.size_.magnitude;
                    GameObject.Instantiate(spawn.prefab_, pos, Quaternion.identity, parent);
                }
            }
        }

        Field.instance_.Spawn(this);
    }

    public void Build()
    {
        marcher_.Triangulate(points_);
        mesh_.Build(marcher_.Triangles);
        ColliderManager.Collider(this);
    }

    public void Draw(Material material)
    {
        mesh_.Draw(pos_, material);
    }

    public void Gizmos()
    {
        
    }

    public float[] Points => points_;
    public Vector3 Position => pos_;
}
