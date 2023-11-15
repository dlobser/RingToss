using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class SDRenderChainLinkRenderDepth : SDRenderChainLink
{
    // First run functions in Unity, probably a screen render
    // Pass these renders along to stable diffusion with a link to arguments
    // public string depthstring;
    string[] args;
    public Texture2D startingDepthImage;
    public Vector2 resolution;
    // public Material mat;
    public bool render360;
    public Cubemap cubemap;

    public Transform container;
    public Material depthMaterial;
    public Material projectionMaterial;
    MeshRenderer[] meshRenderers;
    SkinnedMeshRenderer[] skinnedRenderers;
    SpriteRenderer[] spriteRenderers;
    List<Renderer> renderers;
    List<Material> materials;

    public string[] renderLayerMask;
    public string[] projectionMaterialLayerMask;
    LayerMask cullingMaskBackup;
    public Camera projectionCam;
    bool setupRenderer = true;
    public bool useInputImageForUnityFunc = true;
    public bool switchToInitialMaterial = true;
    public bool switchToProjectionMaterial = false;

    public string saveDepthImageLocation;
    public string saveRenderImageLocation;

    // public Cubemap cubemap;
    public bool debug = false;

    void Start()
    {
        if (projectionCam == null)
        {
            projectionCam = Camera.main;
        }
        if (setupRenderer)
        {
            renderers = null;
            materials = null;
        }
        if (debug)
            print("Start: " + this.gameObject.name);

        SetupRenderers();
    }

    void Update()
    {
        if (debug)
            print("RENDERERS???: " + (renderers == null));
    }

    public override void RunUnityFunction(string image)
    {
        if (projectionCam == null)
        {
            projectionCam = Camera.main;
        }
        if (request == null)
        {
            request = FindObjectOfType<Request>();
        }
        if (debug)
            print(" RunUnityFunction RENDERERS???: " + (renderers == null) + " , " + setupRenderer);

        SetupRenderers();
        if (switchToInitialMaterial)
            SwitchToInitialMaterial();
        if (switchToProjectionMaterial && projectionMaterial!=null)
            SwitchToProjection();

        SaveCullingMask();
        SetLayerMask();

        if (debug)
            print(" RunUnityFunction RENDERERS??? 2: " + (renderers == null));

        Texture2D tex;
        Texture2D texD;

         // Find all TextMesh objects in the scene
        TextMesh[] textMeshes = FindObjectsOfType<TextMesh>();
        foreach (TextMesh textMesh in textMeshes)
        {
            if (textMesh.font != null)
            {
                // Get the MeshRenderer component
                MeshRenderer meshRenderer = textMesh.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    // Update the material to the font's material
                    meshRenderer.sharedMaterial = textMesh.font.material;
                }
            }
        }


        tex = render360 ? Render360((int)resolution.x, (int)resolution.y) : SDRenderUtils.Capture(projectionCam, (int)resolution.x, (int)resolution.y);
        string render = Convert.ToBase64String(tex.EncodeToPNG());

        if (!string.IsNullOrEmpty(image) && useInputImageForUnityFunc)
            render = image;
        if (saveRenderImageLocation.Length > 0)
        {
            SaveImage(render, saveRenderImageLocation);
        }

        SetLayerMask();

        if(depthMaterial!=null)
            SwitchToDepth();

        texD = render360 ? Render360((int)resolution.x, (int)resolution.y) : SDRenderUtils.Capture(projectionCam, (int)resolution.x, (int)resolution.y);
        string depth = Convert.ToBase64String(texD.EncodeToPNG());
        if (saveDepthImageLocation.Length > 0)
        {
            SaveImage(depth, saveDepthImageLocation);
        }
        if (startingDepthImage != null)
        {
            depth = SDRenderUtils.TextureToString(startingDepthImage);
        }
        if(switchToInitialMaterial||switchToProjectionMaterial)
            SwitchToInitialMaterial();
        if (extraValues is ExtraValuesForTxt2Image)
        {
            args = new string[1];
            args[0] = depth;
            GetImageFromSD(extraValues, args);
        }
        else if (extraValues is ExtraValuesForImg2Img)
        {
            args = new string[2];
            args[0] = render;
            args[1] = depth;
            GetImageFromSD(extraValues, args);
        }

        if (debug)
            print(" RunUnityFunction RENDERERS???  3 : " + (renderers == null));

        RestoreCullingMask();
    }

    public override string GetImageFromSD(ExtraValues values, string[] arguments)
    {
        if (request == null)
        {
            request = FindObjectOfType<Request>();
        }
        if (extraValues is ExtraValuesForTxt2Image)
            request.SendTxt2Img((ExtraValuesForTxt2Image)extraValues, arguments[0], OnImageReturnedFromSD);
        else if (extraValues is ExtraValuesForImg2Img)
            request.sendImg2Img((ExtraValuesForImg2Img)extraValues, arguments[0], arguments[1], OnImageReturnedFromSD);

        return "";
    }

    public void SaveImage(string image, string location)
    {
        // Extract the directory path from the location parameter
        string directory = Path.GetDirectoryName(location);

        // Check if the directory exists, and create it if it doesn't
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        SDRenderUtils.SaveImage(image, location, (int)resolution.x, (int)resolution.y);
    }

    void NullOperator(string blank)
    {
        // SwitchToProjection(SDRenderUtils.StringToTexture(blank));
    }

    void OnImageReturnedFromSD(string image)
    {
        foreach (SDRenderChainLink l in link)
        {
            l.RunUnityFunction(image);
        }
        if (link.Length == 0)
        {
            NullOperator(image);
        }
    }

    Texture2D Render360(int width, int height)
    {
        byte[] bytes;
        bytes = I360Render.Capture((int)resolution.x, true, projectionCam, true);
        Texture2D tex = new Texture2D(width, height);
        tex.LoadImage(bytes);
        return tex;
    }

    public void SwitchToDepth()
    {
        for (int i = 0; i < renderers.Count; i++)
        {

            // print("Set depth: " + renderers[i].gameObject.name);
            if (!renderers[i].gameObject.name.Contains("matte"))
                renderers[i].material = depthMaterial;
        }
    }

    public void SwitchToProjection(Texture2D tex)
    {
        projectionMaterial.SetTexture("_MainTex", tex);

        SDRenderUtils.AssignMaterialToGameObjects(
            SDRenderUtils.FindGameObjectsWithLayer(
                projectionMaterialLayerMask),
                projectionMaterial);
    }

    public void SwitchToProjection()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            if (!renderers[i].gameObject.name.Contains("matte"))
                renderers[i].material = projectionMaterial;
        }
    }

    public void SwitchToInitialMaterial()
    {
        SetupRenderers();
        if (renderers == null)
        {
            print("No Renderers! " + this.gameObject.name);
        }
        else
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                // print(materials[i]);
                if (!renderers[i].gameObject.name.Contains("matte"))
                    renderers[i].material = materials[i];
            }
        }
    }

    public void SetRenderersAndMaterials(List<Renderer> ren, List<Material> mat)
    {
        renderers = ren;
        materials = mat;
        setupRenderer = false;
    }

    void SetupRenderers()
    {
        if (debug)
        {
            print("Setup renderer: " + setupRenderer + ", " + renderers);
        }
        if (setupRenderer)
        {
            setupRenderer = false;
            if (renderers == null)
            {
                meshRenderers = container.GetComponentsInChildren<MeshRenderer>();
            }
            if (skinnedRenderers == null)
            {
                skinnedRenderers = container.GetComponentsInChildren<SkinnedMeshRenderer>();
            }
            if (spriteRenderers == null)
            {
                spriteRenderers = container.GetComponentsInChildren<SpriteRenderer>();
            }

            if (renderers == null) // renderers = container.GetComponentsInChildren<MeshRenderer>();
            {
                print("Setup Renderers: " + this.gameObject.name);
                renderers = new List<Renderer>();
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    renderers.Add(meshRenderers[i]);
                }
                for (int i = 0; i < skinnedRenderers.Length; i++)
                {
                    renderers.Add(skinnedRenderers[i]);
                }
                for (int i = 0; i < spriteRenderers.Length; i++)
                {
                    renderers.Add(spriteRenderers[i]);
                }
                if (debug)
                {
                    print("enderer: " + (renderers == null) + ", " + renderers.Count);
                }

                materials = new List<Material>();
                for (int i = 0; i < renderers.Count; i++)
                {
                    materials.Add(renderers[i].sharedMaterial);
                }
            }

            for (int i = 0; i < link.Length; i++)
            {
                // if (link[i] is SDRenderChainLinkRenderDepth)
                // {
                //     ((SDRenderChainLinkRenderDepth)link[i]).SetRenderersAndMaterials(renderers, materials);
                // }
            }
            if (debug)
                print("BRENDERERER: " + (renderers == null));

        }
    }

    public void SaveCullingMask()
    {
        cullingMaskBackup = projectionCam.cullingMask;
    }

    public void RestoreCullingMask()
    {
        projectionCam.cullingMask = cullingMaskBackup;
    }

    public void SetLayerMask()
    {
        if (renderLayerMask.Length > 0)
            projectionCam.cullingMask = MergeLayerMask(renderLayerMask);
    }

    public LayerMask MergeLayerMask(string[] layerNames)
    {

        LayerMask layerMask = 0;

        foreach (string layerName in layerNames)
        {
            layerMask |= 1 << LayerMask.NameToLayer(layerName);
        }

        return layerMask;
    }

}


#if UNITY_EDITOR


[CustomEditor(typeof(SDRenderChainLinkRenderDepth))]
public class SDRenderChainLinkRenderDepthEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SDRenderChainLinkRenderDepth myScript = (SDRenderChainLinkRenderDepth)target;
        if (GUILayout.Button("Render"))
        {
            myScript.RunUnityFunction("");
        }

        SDRenderChainLinkRenderDepth myScript2 = (SDRenderChainLinkRenderDepth)target;
        if (GUILayout.Button("Revert Material"))
        {
            myScript2.SwitchToInitialMaterial();
        }

        SDRenderChainLinkRenderDepth myScript3 = (SDRenderChainLinkRenderDepth)target;
        if (GUILayout.Button("Switch To Depth"))
        {
            myScript2.SwitchToDepth();
        }

        SDRenderChainLinkRenderDepth myScript4 = (SDRenderChainLinkRenderDepth)target;
        if (GUILayout.Button("Switch To Projection"))
        {
            myScript2.SwitchToProjection();
        }
    }
}
#endif