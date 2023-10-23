using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomLevelGenerator))]
public class RandomLevelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RandomLevelGenerator generator = (RandomLevelGenerator)target;
        if (GUILayout.Button("Generate Level"))
        {
            generator.GenerateLevel();
        }
    }
}
