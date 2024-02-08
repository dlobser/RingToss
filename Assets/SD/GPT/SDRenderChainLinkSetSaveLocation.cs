using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDRenderChainLinkSetSaveLocation : SDRenderChainLink
{
    public string saveLocation;
    public string imageName;
    public int startFrame;
    int frame;
    public SDRenderChainLinkSave save;
    public bool debug;

    public override void RunUnityFunction(string image)
    {
        save.location = saveLocation + "" + imageName + "" + frame + ".png";
        frame++;
        if (debug)
        {
            Debug.Log("Save location set to: " + save.location);
        }
        foreach (SDRenderChainLink l in link)
        {
            l.RunUnityFunction("");
        }

    }
}
