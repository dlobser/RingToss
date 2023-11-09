using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Networking;
using System.Threading.Tasks;

public static class SDRenderUtils
{

    public static string BaseDirectory;

    public static Texture2D Capture(Camera cam, int width, int height)
    {
        // Create a new texture with the desired size
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Set the render target of the camera to the texture
        RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 24);
        cam.targetTexture = renderTexture;

        // Render the camera to the texture
        cam.Render();

        // Read the pixels from the render texture into the texture
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();

        // Clean up
        RenderTexture.ReleaseTemporary(renderTexture);
        cam.targetTexture = null;
        RenderTexture.active = null;

        // Return the texture
        return texture;
    }

    public static string TextureToString(Texture2D tex)
    {
        string color = Convert.ToBase64String(tex.EncodeToPNG());
        return color;
    }

    public static string BytesToString(Byte[] bytes)
    {
        string color = Convert.ToBase64String(bytes);
        return color;
    }

    public static Texture2D StringToTexture(string base64, int width = 512, int height = 512)
    {
        byte[] imageBytes = Convert.FromBase64String(base64);
        Texture2D tex = new Texture2D(width, height);
        tex.LoadImage(imageBytes);
        return tex;
    }

    public static void SaveStringToPNG(string base64, string location, int width = 512, int height = 512)
    {
        byte[] bytes = StringToTexture(base64, width, height).EncodeToPNG();
        if (location.Length != 0)
        {
            File.WriteAllBytes(location, bytes);
            Debug.Log("File saved at: " + location);
        }
    }

    public static string ConcatDirectory(string filename, string directory = "")
    {
        if (directory.Length > 0)
        {
            BaseDirectory = directory;
        }
        return BaseDirectory + filename;
    }

    public static GameObject[] FindGameObjectsWithLayer(string[] layer)
    {

        GameObject[] goArray = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        // Debug.Log(goArray.Length);
        var goList = new List<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {

            for (int j = 0; j < layer.Length; j++)
            {
                if (goArray[i].layer == LayerMask.NameToLayer(layer[j]))
                {
                    goList.Add(goArray[i]);
                }
            }

        }
        if (goList.Count == 0)
        {
            return null;
        }
        return goList.ToArray();
    }

    public static void AssignMaterialToGameObjects(GameObject[] gameObjectArray, Material material)
    {
        for (int i = 0; i < gameObjectArray.Length; i++)
        {
            if (gameObjectArray[i].GetComponent<Renderer>() != null)
                gameObjectArray[i].GetComponent<Renderer>().material = material;
        }
    }

    public static void SaveImage(string base64Image, string whereToSave, int width = 512, int height = 512)
    {
        string filename = whereToSave;
        if (!whereToSave.Contains("C:"))
            filename = Application.dataPath + whereToSave;
        byte[] imageBytes = Convert.FromBase64String(base64Image);
        Texture2D tex = new Texture2D(width, height);
        tex.LoadImage(imageBytes);
        byte[] bytes = tex.EncodeToPNG();

        if (whereToSave.Length != 0)
        {
            File.WriteAllBytes(filename, bytes);
            Debug.Log("File saved at: " + filename);
        }

    }

    // public static async Task SaveImageAsync(string base64Image, string whereToSave, int width = 512, int height = 512)
    // {
    //     string filename = whereToSave;
    //     if (!whereToSave.Contains("C:"))
    //         filename = Application.dataPath + whereToSave;

    //     byte[] imageBytes = Convert.FromBase64String(base64Image);
    //     Texture2D tex = new Texture2D(width, height);
    //     tex = await LoadTextureAsync(imageBytes);
    //     byte[] bytes = tex.EncodeToPNG();

    //     if (whereToSave.Length != 0)
    //     {
    //         await Task.Run(() => File.WriteAllBytes(filename, bytes));
    //         Debug.Log("File saved at: " + filename);
    //     }
    // }
    // public static async Task<Texture2D> LoadTextureAsync(byte[] imageBytes)
    // {
    //     using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imageBytes))
    //     {
    //         await uwr.SendWebRequest();

    //         if (uwr.result != UnityWebRequest.Result.Success)
    //         {
    //             Debug.LogError(uwr.error);
    //             return null;
    //         }

    //         DownloadHandlerTexture downloadHandler = (DownloadHandlerTexture)uwr.downloadHandler;
    //         return downloadHandler.texture;
    //     }
    // }

    public static async Task<Texture2D> LoadTextureAsync(byte[] imageBytes, Texture2D tex)
    {
        // Create a new Texture2D to load the image into
        // Texture2D tex = new Texture2D(2, 2);

        // Load the image data asynchronously
        await Task.Run(() => tex.LoadImage(imageBytes));

        // Return the loaded texture
        return tex;
    }

    public static void SaveImage(Texture2D tex, string whereToSave, int width = 512, int height = 512)
    {
        string filename = whereToSave;
        if (!whereToSave.Contains("C:"))
            filename = Application.dataPath + whereToSave;
        byte[] bytes = tex.EncodeToPNG();

        if (whereToSave.Length != 0)
        {
            File.WriteAllBytes(filename, bytes);
            Debug.Log("File saved at: " + filename);
        }

    }


    public static Texture2D LumaMatte(Texture2D texture, float thresshold)
    {
        Color[] pixels = texture.GetPixels();
        Texture2D newTex = new Texture2D(texture.width, texture.height);

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].r > thresshold || pixels[i].g > thresshold || pixels[i].b > thresshold)
            {
                pixels[i].a = 1;
            }
            else
            {
                pixels[i].a = 0;
            }
        }

        newTex.SetPixels(pixels);
        newTex.Apply();

        return newTex;
    }

    public static Texture2D PixelMath(Texture2D texture, Func<Color, Color> mathFunc)
    {
        Color[] pixels = texture.GetPixels();

        Texture2D newTex = new Texture2D(texture.width, texture.height);

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = mathFunc(pixels[i]);
        }

        newTex.SetPixels(pixels);
        newTex.Apply();

        return newTex;
    }

    public static Texture2D SetAlpha(Texture2D color, Texture2D matte)
    {
        // Make sure the textures have the same dimensions
        if (color.width != matte.width || color.height != matte.height)
        {
            Debug.LogError("SetAlpha: Textures must have the same dimensions.");
            return null;
        }

        // Create a new texture to store the result
        Texture2D result = new Texture2D(color.width, color.height, TextureFormat.RGBA32, false);

        // Loop over each pixel and set the alpha channel
        for (int y = 0; y < color.height; y++)
        {
            for (int x = 0; x < color.width; x++)
            {
                Color c = color.GetPixel(x, y);
                Color m = matte.GetPixel(x, y);
                c.a = m.r;
                result.SetPixel(x, y, c);
            }
        }

        // Apply changes and return the result
        result.Apply();
        return result;
    }

    public static Texture2D AlphaToColor(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();
        Texture2D newTex = new Texture2D(texture.width, texture.height);

        for (int i = 0; i < pixels.Length; i++)
        {
            Color pixel = pixels[i];
            pixel.r = pixel.a;
            pixel.g = pixel.a;
            pixel.b = pixel.a;
            pixels[i] = pixel;
        }

        newTex.SetPixels(pixels);
        newTex.Apply();

        return newTex;
    }

    public static Texture2D BlurTexture(Texture2D textureToBlur, float distanceMultiplier = 5, int sampleCount = 5, Texture2D distanceMap = null)
    {
        Texture2D blurredTexture = new Texture2D(textureToBlur.width, textureToBlur.height);

        for (int x = 0; x < textureToBlur.width; x++)
        {
            for (int y = 0; y < textureToBlur.height; y++)
            {
                //get the distance value at this pixel
                Color distance = distanceMap == null ? Color.white : distanceMap.GetPixel(x, y);
                //use the distance value to determine the sample radius
                float sampleRadius = (int)(distance.r * distanceMultiplier);
                //initialize a color variable to store the average color of the samples
                Color averageColor = new Color(0, 0, 0);
                //sample the neighboring pixels
                for (int i = 0; i < sampleCount; i++)
                {
                    Vector2 rando = UnityEngine.Random.insideUnitCircle;
                    int randomX = x + (int)(rando.x * sampleRadius);//UnityEngine.Random.Range(x - sampleRadius, x + sampleRadius);
                    int randomY = y + (int)(rando.y * sampleRadius); UnityEngine.Random.Range(y - sampleRadius, y + sampleRadius);
                    //make sure the sample location is within the bounds of the texture
                    // if (randomX >= 0 && randomX < textureToBlur.width && randomY >= 0 && randomY < textureToBlur.height)
                    // {
                    //add the color of the sample to the average color
                    averageColor += textureToBlur.GetPixel(randomX % textureToBlur.width, randomY % textureToBlur.height);
                    // }
                }
                //divide the average color by the number of samples to get the final color
                averageColor /= sampleCount;
                //set the pixel color of the blurred texture to the final color
                blurredTexture.SetPixel(x, y, averageColor);
            }
        }

        blurredTexture.Apply();

        return blurredTexture;

    }

    public static Texture2D ApplyAntiAliasing(Texture2D input, int kernelSize)
    {
        // Make sure kernel size is odd
        if (kernelSize % 2 == 0)
        {
            Debug.LogWarning("ApplyAntiAliasing: Kernel size must be odd, adding 1.");
            kernelSize++;
        }

        // Create a new texture to store the result
        Texture2D result = new Texture2D(input.width, input.height, TextureFormat.RGBA32, false);

        // Loop over each pixel and apply the filter
        for (int y = 0; y < input.height; y++)
        {
            for (int x = 0; x < input.width; x++)
            {
                // Compute the average color of the neighboring pixels
                Color sum = Color.black;
                int count = 0;
                for (int j = -kernelSize / 2; j <= kernelSize / 2; j++)
                {
                    for (int i = -kernelSize / 2; i <= kernelSize / 2; i++)
                    {
                        int nx = Mathf.Clamp(x + i, 0, input.width - 1);
                        int ny = Mathf.Clamp(y + j, 0, input.height - 1);
                        sum += input.GetPixel(nx, ny);
                        count++;
                    }
                }
                Color avg = sum / count;

                // Set the result pixel color
                result.SetPixel(x, y, avg);
            }
        }

        // Apply changes and return the result
        result.Apply();
        return result;
    }

    // public static Texture2D FillBlackPixels(Texture2D texture)
    // {
    //     // Create a copy of the texture to work with
    //     Texture2D copy = new Texture2D(texture.width, texture.height);
    //     copy.SetPixels(texture.GetPixels());
    //     copy.Apply();

    //     // Loop through all pixels in the texture
    //     for (int x = 0; x < texture.width; x++)
    //     {
    //         for (int y = 0; y < texture.height; y++)
    //         {
    //             // If the current pixel is black, find the nearest colored pixel and set the current pixel to that color
    //             if (copy.GetPixel(x, y) == Color.black)
    //             {
    //                 float closestDistance = Mathf.Infinity;
    //                 Color closestColor = Color.black;

    //                 // Loop through all pixels in the texture again to find the closest colored pixel
    //                 for (int i = 0; i < texture.width; i++)
    //                 {
    //                     for (int j = 0; j < texture.height; j++)
    //                     {
    //                         Color pixelColor = copy.GetPixel(i, j);

    //                         if (pixelColor != Color.black)
    //                         {
    //                             float distance = Vector2.Distance(new Vector2(i, j), new Vector2(x, y));

    //                             if (distance < closestDistance)
    //                             {
    //                                 closestDistance = distance;
    //                                 closestColor = pixelColor;
    //                             }
    //                         }
    //                     }
    //                 }
    //                 closestColor.a = 0;
    //                 // Set the current pixel to the closest colored pixel
    //                 copy.SetPixel(x, y, closestColor);
    //             }
    //         }
    //     }

    //     copy.Apply();
    //     return copy;
    // }

    // public static float ColorDistance(Color c1, Color c2)
    // {
    //     float rDiff = c1.r - c2.r;
    //     float gDiff = c1.g - c2.g;
    //     float bDiff = c1.b - c2.b;
    //     return Mathf.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);
    // }

    // public static Texture2D FillBlackPixels(Texture2D texture)
    // {
    //     Color[] pixels = texture.GetPixels();

    //     // Iterate through each pixel in the texture
    //     for (int i = 0; i < pixels.Length; i++)
    //     {
    //         Color pixelColor = pixels[i];

    //         // Check if the alpha value is greater than 0 and less than 1
    //         if (pixelColor.a > 0.02f && pixelColor.a < .98f)
    //         {
    //             float minDistance = float.MaxValue;
    //             Color nearestColor = Color.clear;

    //             // Find the nearest pixel with an alpha value of 1
    //             for (int j = 0; j < pixels.Length; j++)
    //             {
    //                 Color otherColor = pixels[j];

    //                 if (otherColor.a == 1f)
    //                 {
    //                     float distance = ColorDistance(pixelColor, otherColor);

    //                     if (distance < minDistance)
    //                     {
    //                         minDistance = distance;
    //                         nearestColor = otherColor;
    //                     }
    //                 }
    //             }

    //             // Set the current pixel's color to the nearest color with alpha 1
    //             pixels[i] = nearestColor;
    //         }
    //     }

    //     // Apply the modified pixels to the texture and return it
    //     texture.SetPixels(pixels);
    //     texture.Apply();
    //     return texture;
    // }

    public static float ColorDistance(Color c1, Color c2)
    {
        float rDiff = c1.r - c2.r;
        float gDiff = c1.g - c2.g;
        float bDiff = c1.b - c2.b;
        return Mathf.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);
    }

    public static Texture2D FillTransparentPixels(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;

        Texture2D result = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

        // Create a copy of the texture's pixels
        Color[] pixels = texture.GetPixels();
        Color[] oPixels = texture.GetPixels();

        // Loop through each pixel in the texture
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;

                // Check if the pixel is transparent
                if (pixels[index].a < .99f && pixels[index].a > .01f)
                {
                    Color nearestColor = Color.clear;
                    float nearestDistance = float.MaxValue;

                    // Search within a 10-pixel radius for the nearest non-transparent color
                    for (int j = Mathf.Max(0, y - 10); j <= Mathf.Min(height - 1, y + 10); j++)
                    {
                        for (int i = Mathf.Max(0, x - 10); i <= Mathf.Min(width - 1, x + 10); i++)
                        {
                            int neighborIndex = j * width + i;

                            // Check if the neighbor pixel is non-transparent
                            if (pixels[neighborIndex].a >= 1)
                            {
                                // Calculate the distance between the transparent pixel and the non-transparent neighbor pixel
                                float distance = ColorDistance(pixels[index], oPixels[neighborIndex]);

                                // Update the nearest color if the neighbor pixel is closer
                                if (distance < nearestDistance)
                                {
                                    nearestColor = oPixels[neighborIndex];
                                    nearestDistance = distance;
                                }
                            }
                        }
                    }

                    // // Set the transparent pixel to the nearest non-transparent color
                    nearestColor.a = texture.GetPixel(x, y).a;
                    pixels[index] = nearestColor;
                }
                // pixels[index] = Color.blue
            }
        }

        // Set the texture's pixels to the modified copy
        result.SetPixels(pixels);
        result.Apply();
        return result;
    }

    public static IEnumerator DownloadTextureCoroutine(string url, Action<Texture2D> onTextureDownloaded)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(uwr.error);
            }
            else
            {
                // Get downloaded texture
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);

                // Call the callback with the downloaded texture
                onTextureDownloaded?.Invoke(texture);
            }
        }
    }



    //create a new texture with the same dimensions as the original texture





}
