using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // Required for UnityAction

public class SDRenderChainLinkBuildPlatform : SDRenderChainLink
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

    public override void RunUnityFunction(string image)
    {
        StartCoroutine(GenerateAndManagePlatformRoutine());
    }

    IEnumerator GenerateAndManagePlatformRoutine()
    {
        // Clear existing children
        foreach (Transform child in root.transform)
        {
            Destroy(child.gameObject);
        }

        // Generate new platform
        GameObject newPlatform = levelGenerator.GeneratePlatformPositions();
        if (newPlatform != null)
        {
            newPlatform.transform.SetParent(root.transform, false);

            // Set the layer of the new platform and its children
            SetLayerRecursively(newPlatform, LayerMask.NameToLayer("BG"));
        }

        yield return null;
        foreach (SDRenderChainLink l in link)
        {
            l.RunUnityFunction("");
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
