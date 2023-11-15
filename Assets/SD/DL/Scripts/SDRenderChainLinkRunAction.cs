using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // Required for UnityAction

public class SDRenderChainLinkRunAction : SDRenderChainLink
{
    // Define a UnityAction that takes a string parameter
    public UnityEvent onImageProcessed;

    public override void RunUnityFunction(string image)
    {
        bool isExtraValuesForImg2Img = extraValues is ExtraValuesForImg2Img;

        string[] args = new string[2];
        args[0] = image;
        args[1] = image;
        if(extraValues!=null)
            print("Run Script: " + extraValues.gameObject.name);
        // GetImageFromSD(extraValues, args);
        OnImageReturnedFromSD("");
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
        // Invoke the UnityAction
        onImageProcessed?.Invoke();

        foreach (SDRenderChainLink l in link)
        {
            l.RunUnityFunction(image);
        }

    }
}