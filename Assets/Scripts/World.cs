using UnityEngine;

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

    private Noise noise_;
    private Marcher marcher_;
    private Chunk chunk_;

    private void Awake()
    {
        instance_ = this;
        noise_ = GetComponent<Noise>();
        marcher_ = GetComponent<Marcher>();
    }

    private void Start()
    {
        chunk_ = new Chunk(Vector3.zero);
        chunk_.Generate(noise_, size_, profile_);
        chunk_.Build(marcher_, size_, threshold_, step_);
    }

    private void Update()
    {
        Graphics.DrawMesh(chunk_.Mesh, Vector3.zero, Quaternion.identity, material_, 0);
    }

    private void OnValidate()
    {
        size_.x -= size_.x % THREADS;
        size_.y -= size_.y % THREADS;
        size_.z -= size_.z % THREADS;

        if(chunk_ != null)
        {
            chunk_.Generate(noise_, size_, profile_);
            chunk_.Build(marcher_, size_, threshold_, step_);
        }
    }
}
