using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RenderDepthAndProjection : SDRenderChainLink
{
    public Transform container;
    public Material depthMaterial;
    public Material projectionMaterial;
    MeshRenderer[] meshRenderers;
    SkinnedMeshRenderer[] skinnedRenderers;
    List<Renderer> renderers;
    List<Material> materials;
    public int resolution;
    public Texture2D texture;
    // Request request;
    public bool img2img;
    public string[] displayLayers;
    LayerMask cullingMaskBackup;
    public Camera projectionCam;
    public string saveDepthImageLocation;
    public string saveRenderImageLocation;

    void Start()
    {

    }

    public void SwitchToDepth()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].material = depthMaterial;
        }
    }

    public void SwitchToProjection(Texture2D tex)
    {
        projectionMaterial.SetTexture("_MainTex", tex);
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].material = projectionMaterial;
        }
    }

    public void SwitchToInitialMaterial()
    {
        SetupRenderers();
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].material = materials[i];
        }
    }


    // Update is called once per frame
    void Update()
    {

    }

    void SetupRenderers()
    {
        if (renderers == null)
        {
            meshRenderers = container.GetComponentsInChildren<MeshRenderer>();
        }
        if (skinnedRenderers == null)
        {
            skinnedRenderers = container.GetComponentsInChildren<SkinnedMeshRenderer>();
        }

        if (renderers == null) // renderers = container.GetComponentsInChildren<MeshRenderer>();
        {
            renderers = new List<Renderer>();
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                renderers.Add(meshRenderers[i]);
            }
            for (int i = 0; i < skinnedRenderers.Length; i++)
            {
                renderers.Add(skinnedRenderers[i]);
            }

            materials = new List<Material>();
            for (int i = 0; i < renderers.Count; i++)
            {
                materials.Add(renderers[i].sharedMaterial);
            }
        }
    }

    public void SDRender()
    {
        if (request == null)
        {
            request = FindObjectOfType<Request>();
        }

        SetupRenderers();
        SwitchToInitialMaterial();
        Texture2D tex = Capture();
        string color = Convert.ToBase64String(tex.EncodeToPNG());
        SwitchToDepth();
        Texture2D texD = Capture();
        string depth = Convert.ToBase64String(texD.EncodeToPNG());

        if (img2img)
            request.sendImg2Img((ExtraValuesForImg2Img)extraValues, color, depth, AssignReturnedTexture);
        else
            request.SendTxt2Img((ExtraValuesForTxt2Image)extraValues, depth, AssignReturnedTexture);

    }

    public override void RunUnityFunction(string image)
    {
        SetupRenderers();
        byte[] imageBytes = Convert.FromBase64String(image);
        Texture2D tex = new Texture2D(resolution, resolution);
        tex.LoadImage(imageBytes);
        SwitchToProjection(tex);
        print("Complete");
    }

    void AssignReturnedTexture(string texture)
    {
        SwitchToProjection(SDRenderUtils.StringToTexture(texture));
    }

    public Texture2D Capture()
    {
        int width = resolution;
        int height = resolution;
        // Create a new texture with the desired size
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Set the render target of the camera to the texture
        RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 24);
        projectionCam.targetTexture = renderTexture;

        // Render the camera to the texture
        projectionCam.Render();

        // Read the pixels from the render texture into the texture
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();

        // Clean up
        RenderTexture.ReleaseTemporary(renderTexture);
        projectionCam.targetTexture = null;
        RenderTexture.active = null;

        // Return the texture
        return texture;
    }


    public void SaveCullingMask()
    {
        cullingMaskBackup = projectionCam.cullingMask;
    }

    public void RestoreCullingMask()
    {
        projectionCam.cullingMask = cullingMaskBackup;
    }

    public static void SetLayerMask(Camera cam, string[] layerNames)
    {
        // Create a layer mask for the specified layers.
        LayerMask layerMask = 0;
        foreach (string layerName in layerNames)
        {
            layerMask |= 1 << LayerMask.NameToLayer(layerName);
        }

        // Set the camera's layer mask to the specified layer mask.
        cam.cullingMask = layerMask;
    }
}


#if UNITY_EDITOR


[CustomEditor(typeof(RenderDepthAndProjection))]
public class RenderDepthAndProjectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RenderDepthAndProjection myScript = (RenderDepthAndProjection)target;
        if (GUILayout.Button("SD Render"))
        {
            myScript.SDRender();
        }

        RenderDepthAndProjection myScript2 = (RenderDepthAndProjection)target;
        if (GUILayout.Button("Revert Material"))
        {
            myScript2.SwitchToInitialMaterial();
        }
    }
}
#endif