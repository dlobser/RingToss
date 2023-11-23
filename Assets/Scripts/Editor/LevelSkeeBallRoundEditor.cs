using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelGenerator_SkeeBall_Round))]
public class LevelGenerator_SkeeBall_RoundEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelGenerator_SkeeBall_Round generator = (LevelGenerator_SkeeBall_Round)target;
        if (GUILayout.Button("Generate Platforms"))
        {
            generator.GeneratePlatforms();
        }
        
        if (GUILayout.Button("Generate Platform Positions"))
        {
            generator.GeneratePlatformPositions();
        }
    }
}
