using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Networking;
using Unity.VisualScripting;

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

    public bool useMatte;
    public string matteLocation;

    public bool debug = false;
    string sdImage;

    public override void RunUnityFunction(string image)
    {
        StartCoroutine(RunUnityFunctionRoutine(image));
    }

    IEnumerator RunUnityFunctionRoutine(string image)
    {
        sdImage = image;
        // Extract the directory path from the location parameter
        string directory = Path.GetDirectoryName(location);
        if(debug) print(this.gameObject.name + " Save Dir: " + location);
        // Check if the directory exists, and create it if it doesn't
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        yield return null;

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
            yield return null;
        }
        else if(useMatte){
            LoadTexture(matteLocation, SetMatteOnTexture);
            yield return null;
        }
        else
            SDRenderUtils.SaveImage(image, location, width, height);
        yield return null;

        foreach (SDRenderChainLink l in link)
        {
            if(debug) print(l.gameObject.name + " Run Unity Function " );
            l.RunUnityFunction(image);
        }
    }

    void SetMatteOnTexture(Texture2D texture){
        Texture2D tex = SDRenderUtils.StringToTexture(sdImage);
        tex = SDRenderUtils.SetAlpha(tex, texture);
        SDRenderUtils.SaveImage(tex, location, width, height);
    }

    // Define a callback delegate to return the texture
    public delegate void TextureCallback(Texture2D texture);

    // Coroutine to load a texture
    public void LoadTexture(string filePath, TextureCallback callback)
    {
        StartCoroutine(CoroutineLoadTexture(filePath, callback));
    }

    private IEnumerator CoroutineLoadTexture(string filePath, TextureCallback callback)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("file://" + filePath))
        {
            // Send the request
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                // Get the downloaded texture and invoke the callback
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                callback?.Invoke(texture);
            }
            else
            {
                Debug.LogError("Failed to load texture: " + uwr.error);
                callback?.Invoke(null);
            }
        }
    }


}
