using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SDRenderChainLinkSetMaterialProperty : SDRenderChainLink
{
    // First run functions in Unity, probably a screen render
    // Pass these renders along to stable diffusion with a link to arguments
    public float start;
    float count = 0;
    public float increment;
    public Material mat;
    public string channel;

    void Start()
    {
        count = start;
    }

    public override void RunUnityFunction(string image)
    {
        mat.SetFloat(channel, count);
        count += increment;

    }


}
