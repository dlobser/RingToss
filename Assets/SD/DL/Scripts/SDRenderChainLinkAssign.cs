using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SDRenderChainLinkAssign : SDRenderChainLink
{
    // First run functions in Unity, probably a screen render
    // Pass these renders along to stable diffusion with a link to arguments
    public Material projectionMaterial;
    public string[] projectionMaterialLayerMask;

    public int width = 512;
    public int height = 512;
    public string textureChannel = "_MainTex";
    byte[] imageBytes;
    Texture2D tex;

    void Start()
    {
        tex = new Texture2D(width, height);
    }

    public override void RunUnityFunction(string image)
    {
        SDRenderUtils.AssignMaterialToGameObjects(
            SDRenderUtils.FindGameObjectsWithLayer(
                projectionMaterialLayerMask),
                projectionMaterial);

        imageBytes = Convert.FromBase64String(image);

        tex.LoadImage(imageBytes);
        projectionMaterial.SetTexture(textureChannel, tex);

        foreach (SDRenderChainLink l in link)
        {
            l.RunUnityFunction(image);
        }

    }


}
