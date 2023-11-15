using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SDRenderChainLinkRenderSimple : SDRenderChainLink
{
    // First run functions in Unity, probably a screen render
    // Pass these renders along to stable diffusion with a link to arguments


    public override void RunUnityFunction(string image)
    {
        if (extraValues is ExtraValuesForTxt2Image)
            GetImageFromSD((ExtraValuesForTxt2Image)extraValues, new string[]{""});
        else if (extraValues is ExtraValuesForImg2Img)
            GetImageFromSD((ExtraValuesForImg2Img)extraValues, new string[]{""});

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

    void NullOperator(string blank)
    {
        print(blank);
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
}
