using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    private int newSeed; // Temporary variable to store the new seed

    private void OnEnable()
    {
        // Initialize the newSeed with the current value of GlobalSettings.randomSeed
        newSeed = GlobalSettings.randomSeed;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draws the default inspector

        GameManager gameManager = (GameManager)target;

        // Display an input field for the random seed
        newSeed = EditorGUILayout.IntField("Random Seed", newSeed);

        // Add a button that will only update the seed when clicked
        if (GUILayout.Button("Update Seed"))
        {
            Undo.RecordObject(gameManager, "Change Random Seed");
            GlobalSettings.randomSeed = newSeed;
            EditorUtility.SetDirty(gameManager); // Mark the GameManager as dirty to ensure the change is saved

            // If GlobalSettings is a non-MonoBehaviour script, you might need to mark the scene dirty
            // EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}
