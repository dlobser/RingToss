using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelGenerator_SkeeBall))]
public class LevelGenerator_SkeeBallEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelGenerator_SkeeBall generator = (LevelGenerator_SkeeBall)target;
        if (GUILayout.Button("Generate Platforms"))
        {
            generator.GeneratePlatforms();
        }
    }
}
