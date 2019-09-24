using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(World))]
public class WorldEditor : Editor
{
    private World obj_;

    private void OnEnable()
    {
        obj_ = (World)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(Application.isPlaying)
        {
            if(GUILayout.Button("Rebuild"))
            {
                obj_.Generate();
                obj_.Build();
            }
        }
    }
}
