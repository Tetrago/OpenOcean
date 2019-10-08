using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public enum GizmoType { Never, SelectedOnly, Always }

    public Boid prefab;
    public float spawnRadius = 10;
    public int spawnCount = 10;
    public bool randomColor;
    public Color colour;
    public GizmoType showSpawnRegion;

    private static GameObject parent_;

    public static Transform Parent
    {
        get
        {
            if(parent_ == null)
                parent_ = new GameObject("Boids");
            return parent_.transform;
        }
    }

    void Awake () {
        for (int i = 0; i < spawnCount; i++) {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            Boid boid = Instantiate (prefab, Parent);
            boid.transform.position = pos;
            boid.transform.forward = Random.insideUnitSphere;

            if(!randomColor)
                boid.SetColour(colour);
            else
                boid.SetColour(Random.ColorHSV(0, 1, 0.75f, 0.8f));
        }
    }

    private void OnDrawGizmos () {
        if (showSpawnRegion == GizmoType.Always) {
            DrawGizmos ();
        }
    }

    void OnDrawGizmosSelected () {
        if (showSpawnRegion == GizmoType.SelectedOnly) {
            DrawGizmos ();
        }
    }

    void DrawGizmos () {

        Gizmos.color = new Color (colour.r, colour.g, colour.b, 0.3f);
        Gizmos.DrawSphere (transform.position, spawnRadius);
    }

}