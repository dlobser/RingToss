using UnityEngine;
using TMPro; // Namespace for TextMeshPro

public class UISeedDisplay : MonoBehaviour
{
    public TextMeshPro textMesh; // Reference to your TextMeshPro UI object

    void Update()
    {
        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro component is not assigned!");
            return;
        }

        // Assuming GlobalSettings is a class you have that contains a static randomSeed variable
        textMesh.text = "#" + GlobalSettings.randomSeed.ToString();
    }
}
