using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SDRenderChainLinkSave : SDRenderChainLink
{
    // First run functions in Unity, probably a screen render
    // Pass these renders along to stable diffusion with a link to arguments
    public string location;
    public int width = 512;
    public int height = 512;

    public bool makeMatte;
    public float matteThreshold = 0;
    public float blurAmount = 0;
    public float postBlurThreshold = 0;
    public int antiAliasingKernel = 3;

    public override void RunUnityFunction(string image)
    {
        // Extract the directory path from the location parameter
        string directory = Path.GetDirectoryName(location);

        // Check if the directory exists, and create it if it doesn't
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (makeMatte)
        {
            Texture2D tex = SDRenderUtils.StringToTexture(image);
            tex = SDRenderUtils.LumaMatte(tex, matteThreshold);
            if (blurAmount > 0)
            {
                Texture2D matte = SDRenderUtils.AlphaToColor(tex);
                matte = SDRenderUtils.BlurTexture(matte, blurAmount, 60);
                matte = SDRenderUtils.PixelMath(matte, (c) =>
                {
                    float high = 1;
                    float m = Mathf.Lerp(postBlurThreshold, 1, .5f);
                    float low = postBlurThreshold;

                    float range = 1f / (high - low);
                    float mid = (m - low) * range;

                    float col = c.r;

                    // Apply the levels adjustment to each channel
                    col = Mathf.Clamp((col - low) * range, 0f, 1f);

                    // Apply the gamma correction
                    col = Mathf.Pow(col, 1f / mid);

                    return new Color(col, col, col, col);
                });
                matte = SDRenderUtils.BlurTexture(matte, blurAmount * .25f, 60);
                matte = SDRenderUtils.ApplyAntiAliasing(matte, antiAliasingKernel);
                matte = SDRenderUtils.PixelMath(matte, (c) =>
                {
                    return c * 3;
                });
                tex = SDRenderUtils.SetAlpha(tex, matte);
                tex = SDRenderUtils.FillTransparentPixels(tex);

            }
            SDRenderUtils.SaveImage(tex, location, width, height);
        }
        else
            SDRenderUtils.SaveImage(image, location, width, height);

        foreach (SDRenderChainLink l in link)
        {
            l.RunUnityFunction(image);
        }
    }


}
