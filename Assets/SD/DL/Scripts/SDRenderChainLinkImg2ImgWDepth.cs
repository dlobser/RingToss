using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SDRenderChainLinkTestImg2ImgWDepth : SDRenderChainLink
{
    // First run functions in Unity, probably a screen render
    // Pass these renders along to stable diffusion with a link to arguments
    public string depthstring;
    string[] args;
    public Texture2D startingDepthImage;

    void Start()
    {
        if (depthstring.Length > 0)
        {
            RunUnityFunction(depthstring);
        }
        else if (startingDepthImage != null)
        {
            string depth = Convert.ToBase64String(startingDepthImage.EncodeToPNG());
            RunUnityFunction(depth);
        }
    }

    public override void RunUnityFunction(string image)
    {
        args = new string[1];
        args[0] = image;
        if (extraValues is ExtraValuesForTxt2Image)
            GetImageFromSD(extraValues, args);
    }

    public override string GetImageFromSD(ExtraValues values, string[] arguments)
    {
        if (request == null)
        {
            request = FindObjectOfType<Request>();
        }

        // request.SendTxt2Img(arguments[0], NullOperator);

        return "";
    }

    void NullOperator(string blank)
    {
        print(blank);
    }
}
