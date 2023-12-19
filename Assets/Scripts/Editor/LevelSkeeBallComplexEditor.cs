using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelGenerator_SkeeBall_Complex))]
public class LevelGenerator_SkeeBall_ComplexEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelGenerator_SkeeBall_Complex generator = (LevelGenerator_SkeeBall_Complex)target;
        if (GUILayout.Button("Generate Platforms"))
        {
            generator.GeneratePlatforms();
        }
        
        if (GUILayout.Button("Generate Platform Positions"))
        {
            GlobalSettings.randomSeed = (int)(Random.value*10000);
            generator.GeneratePlatformPositions();
        }
    }
}
