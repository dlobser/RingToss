using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDRenderChainLink : MonoBehaviour
{
    // First run functions in Unity, probably a screen render
    // Pass these renders along to stable diffusion with a link to arguments
    public ExtraValues extraValues;
    public SDRenderChainLink[] link;
    public Request request { get; set; }

    public virtual void RunUnityFunction(string image)
    {
        bool isExtraValuesForImg2Img = extraValues is ExtraValuesForImg2Img;
    }

    public virtual string GetImageFromSD(ExtraValues values, string[] arguments)
    {
        return "";
    }
}
