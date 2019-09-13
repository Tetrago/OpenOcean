using UnityEngine;

[RequireComponent(typeof(Noise))]
[RequireComponent(typeof(Marcher))]
[RequireComponent(typeof(Feature))]
public class World : MonoBehaviour
{
    public static World instance_;

    public const int THREADS = 8;

    [Header("Chunk Generation")]
    public GenerationProfile generationProfile_;
    [HideInInspector] public FeatureProfile featureProfile_;

    [Header("World Generation")]
    public Vector3Int size_;
    [Range(-1, 1)] public float threshold_;
    public float step_;
    public Material material_;
    public Vector3Int worldSize_;

    private Noise noise_;
    private Marcher marcher_;
    private Feature feature_;

    private Chunk[,,] chunks_;

    private void Awake()
    {
        instance_ = this;

        noise_ = GetComponent<Noise>();
        marcher_ = GetComponent<Marcher>();
        feature_ = GetComponent<Feature>();

        if(GetComponent<TerrainShaderAPI>())
            GetComponent<TerrainShaderAPI>().Configure(size_);
    }

    private void Start()
    {
        ColliderManager.Init();
        chunks_ = new Chunk[worldSize_.x, worldSize_.y, worldSize_.z];

        Generate();
        Build();
    }

    public void Generate()
    {
        for(int x = 0; x < worldSize_.x; ++x)
        {
            for(int y = 0; y < worldSize_.y; ++y)
            {
                for(int z = 0; z < worldSize_.z; ++z)
                {
                    Chunk chunk = new Chunk(transform.position + new Vector3(x * (size_.x - 1) * step_, y * (size_.y - 1) * step_, z * (size_.z - 1) * step_));
                    chunk.Generate(noise_, generationProfile_);
                    chunks_[x, y, z] = chunk;
                }
            }
        }
    }

    public void Build()
    {
        foreach(Chunk chunk in chunks_)
            chunk.Build(marcher_, feature_, size_, threshold_, step_, featureProfile_);
    }

    private void Update()
    {
        foreach(Chunk chunk in chunks_)
            Graphics.DrawMesh(chunk.Mesh, chunk.Position, Quaternion.identity, material_, 0);
    }

    private void OnValidate()
    {
        size_.x -= size_.x % THREADS;
        size_.y -= size_.y % THREADS;
        size_.z -= size_.z % THREADS;
    }
}
