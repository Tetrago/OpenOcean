using UnityEngine;

public class World : MonoBehaviour
{
    public static World instance_;

    public const int THREADS = 8;

    [Header("Chunk Generation")]
    public NoiseProfile noiseProfile_;

    [Header("World Generation")]
    [Range(-1, 1)] public float threshold_;
    public float step_;
    public Material material_;
    public Vector3Int size_;
    public int seed_;

    [Header("Chunk Organization")]
    public ChunkOrganizer.Type type_;
    public StaticChunkOrganizer.Settings staticSettings_;
    public DynamicChunkOrganizer.Settings dynamicSettings_;

    private ChunkOrganizer organizer_;

    private void Awake()
    {
        instance_ = this;

        if(GetComponent<TerrainShaderAPI>())
            GetComponent<TerrainShaderAPI>().Configure(size_);

        switch(type_)
        {
            case ChunkOrganizer.Type.Static:
                organizer_ = new StaticChunkOrganizer(staticSettings_);
                break;
            case ChunkOrganizer.Type.Dynamic:
                organizer_ = new DynamicChunkOrganizer(dynamicSettings_);
                break;
        }
    }

    private void Start()
    {
        ColliderManager.Init();
        organizer_.Allocate();

        Generate();
        Build();
    }

    public void Generate()
    {
        organizer_.Generate(transform.position);
    }

    public void Build()
    {
        organizer_.Build();
    }

    private void Update()
    {
        organizer_.Draw(material_);
    }

    private void OnDrawGizmos()
    {
        if(organizer_ != null)
            organizer_.Gizmos();
    }

    private void OnValidate()
    {
        size_.x -= size_.x % THREADS;
        size_.y -= size_.y % THREADS;
        size_.z -= size_.z % THREADS;
    }
}
