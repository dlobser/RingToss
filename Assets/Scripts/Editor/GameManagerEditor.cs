using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draws the default inspector

        GameManager gameManager = (GameManager)target;

        // Adding an input field for the random seed
        EditorGUI.BeginChangeCheck();
        int newSeed = EditorGUILayout.IntField("Random Seed", GlobalSettings.randomSeed);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(gameManager, "Change Random Seed");
            GlobalSettings.randomSeed = newSeed;
            EditorUtility.SetDirty(gameManager); // Mark the GameManager as dirty to ensure the change is saved
        }
    }
}