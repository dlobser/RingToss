using UnityEngine;

public class CreatePlatformForSD : MonoBehaviour
{
    public GameObject root;
    public string layerName = "BG";
    public LevelGenerator_SkeeBall_Round levelGenerator;

    void Start()
    {
        // Initialize the 'root' GameObject
        if (root == null)
        {
            root = new GameObject("Root");
        }

        // Call this to generate the initial platform
        // GenerateAndManagePlatform();
    }

    public void GenerateAndManagePlatform()
    {
        // Clear existing children
        foreach (Transform child in root.transform)
        {
            Destroy(child.gameObject);
        }

        // Generate new platform
        GameObject newPlatform = levelGenerator.GeneratePlatformPositions();
        print("Made Platform" + newPlatform.name);
        if (newPlatform != null)
        {
            newPlatform.transform.SetParent(root.transform, false);

            // Set the layer of the new platform and its children
            SetLayerRecursively(newPlatform, LayerMask.NameToLayer("BG"));
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
