﻿using UnityEngine;

[RequireComponent(typeof(Noise))]
[RequireComponent(typeof(Marcher))]
public class World : MonoBehaviour
{
    public static World instance_;

    public const int THREADS = 8;

    public NoiseProfile profile_;
    public Vector3Int size_;
    [Range(-1, 1)] public float threshold_;
    public float step_;
    public Material material_;
    public Vector3Int worldSize_;

    private Noise noise_;
    private Marcher marcher_;
    private Chunk[] chunks_;

    private void Awake()
    {
        instance_ = this;
        noise_ = GetComponent<Noise>();
        marcher_ = GetComponent<Marcher>();
    }

    private void Start()
    {
        ColliderManager.Init();

        chunks_ = new Chunk[worldSize_.x * worldSize_.y * worldSize_.z];

        for(int x = 0; x < worldSize_.x; ++x)
        {
            for(int y = 0; y < worldSize_.y; ++y)
            {
                for(int z = 0; z < worldSize_.z; ++z)
                {
                    Chunk chunk = new Chunk(new Vector3(x * (size_.x - 1) * step_, y * (size_.y - 1) * step_, z * (size_.z - 1) * step_));

                    chunk.Generate(noise_, size_, profile_);
                    chunk.Build(marcher_, size_, threshold_, step_);

                    chunks_[x + worldSize_.y * (y + worldSize_.x * z)] = chunk;
                }
            }
        }
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
