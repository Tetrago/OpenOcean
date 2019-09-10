using UnityEngine;

[CreateAssetMenu(menuName = "OpenOcean/FeatureStack")]
public class FeatureProfile : ScriptableObject
{
    public float lineCutoff_;
    public Vector2Int lineRandRange_;
    public float randMultiplier_;
}
