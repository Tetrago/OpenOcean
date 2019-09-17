﻿using UnityEngine;

[RequireComponent(typeof(Noise))]
[RequireComponent(typeof(Marcher))]
[RequireComponent(typeof(Feature))]
public class World : MonoBehaviour
{
    public static World instance_;

    public const int THREADS = 8;

    [Header("Chunk Generation")]
    public NoiseProfile noiseProfile_;
    [HideInInspector] public FeatureProfile featureProfile_;

    [Header("World Generation")]
    [Range(-1, 1)] public float threshold_;
    public float step_;
    public Material material_;
    public Vector3Int size_;

    [Header("Chunk Organization")]
    public ChunkOrganizer.Type type_;
    public StaticChunkOrganizer.Settings staticSettings_;

    private Noise noise_;
    private Marcher marcher_;
    private Feature feature_;

    private Chunk[,,] chunks_;
    private ChunkOrganizer organizer_;

    private void Awake()
    {
        instance_ = this;

        noise_ = GetComponent<Noise>();
        marcher_ = GetComponent<Marcher>();
        feature_ = GetComponent<Feature>();

        if(GetComponent<TerrainShaderAPI>())
            GetComponent<TerrainShaderAPI>().Configure(size_);

        switch(type_)
        {
            case ChunkOrganizer.Type.Static:
                organizer_ = new StaticChunkOrganizer();
                break;
            case ChunkOrganizer.Type.Dynamic:
                break;
        }
    }

    private void Start()
    {
        ColliderManager.Init();
        chunks_ = new Chunk[staticSettings_.worldSize_.x, staticSettings_.worldSize_.y, staticSettings_.worldSize_.z];

        Generate();
        Build();
    }

    public void Generate()
    {
        organizer_.Generate(ref chunks_, noise_, transform.position);
    }

    public void Build()
    {
        organizer_.Build(ref chunks_, marcher_, feature_);
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
