using UnityEngine;
using TMPro;

public class ApplyFontsGlobally : MonoBehaviour
{
    // public GameObject rootObject; // The root object to start the search from
    public string resourcePath; // The resource path where fonts are located
    public int fontIndex; // The index of the font to apply

    // Call this method to start the font application process
    // public void ApplyFontToAllTMP()
    // {
    //     if (rootObject == null)
    //     {
    //         Debug.LogError("Root object is null.");
    //         return;
    //     }

    //     ApplyFontRecursively(rootObject);
    // }

    public void ApplyFontRecursively(GameObject obj)
    {
        TextMeshProUGUI textMesh = obj.GetComponent<TextMeshProUGUI>();
        if (textMesh != null)
        {
             ImageLoader.Instance.ApplySelectedFont(textMesh, resourcePath, fontIndex);
        }

        // Iterate through all children, even if they are inactive
        foreach (Transform child in obj.transform)
        {
            ApplyFontRecursively(child.gameObject);
        }
    }

    // public void ApplySelectedFont(TextMeshProUGUI textMeshPro, string resourcePath, int fontIndex)
    // {
    //     TMP_FontAsset[] fonts = Resources.LoadAll<TMP_FontAsset>(resourcePath);
    //     if (fonts.Length > fontIndex && fontIndex >= 0)
    //     {
    //         textMeshPro.font = fonts[fontIndex];
    //     }
    //     else
    //     {
    //         Debug.LogError("Invalid font index or TMP font not found.");
    //     }
    // }
}
