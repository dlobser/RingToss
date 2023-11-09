using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDRenderChainLinkRunScript : SDRenderChainLink
{
    // First run functions in Unity, probably a screen render
    // Pass these renders along to stable diffusion with a link to arguments


    public override void RunUnityFunction(string image)
    {
        bool isExtraValuesForImg2Img = extraValues is ExtraValuesForImg2Img;

        string[] args = new string[2];
        args[0] = image;
        args[1] = image;
        print("Run Script: " + extraValues.gameObject.name);
        GetImageFromSD(extraValues, args);
    }

    public override string GetImageFromSD(ExtraValues values, string[] arguments)
    {
        string image = arguments[0];

        if (request == null)
        {
            request = FindObjectOfType<Request>();
        }

        request.sendImg2Img((ExtraValuesForImg2Img)extraValues, arguments[0], arguments[1], OnImageReturnedFromSD);

        return "";
    }

    void OnImageReturnedFromSD(string image)
    {
        foreach (SDRenderChainLink l in link)
        {
            l.RunUnityFunction(image);
        }
    }
}
