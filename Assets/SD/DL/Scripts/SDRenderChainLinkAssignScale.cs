using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.IO;

public class SDRenderChainLinkAssignScale : SDRenderChainLink
{
    // First run functions in Unity, probably a screen render
    // Pass these renders along to stable diffusion with a link to arguments
    public Material projectionMaterial;
    public GameObject quad;
    public float speed;
    GameObject scalar;
    GameObject noScale;
    List<GameObject> quads;
    List<string> images;
    List<Texture2D> textures;
    Texture2D[] texturePool;
    Material[] materialPool;
    int texturePoolIndex = 0;
    int materialPoolIndex = 0;
    List<int> IDs;
    bool ready = false;
    int id = 0;

    float thisTime;
    float prevTime;
    float average;

    public float scalePower = 2;

    Coroutine coroutine;
    float speedFade = 1;

    GameObject quadParent;
    // List<Material> usedMaterials;
    // List<Material> unusedMaterials;
    List<GameObject> unusedQuads;

    public float maxScale = 32;

    public SDRenderChainLinkSave saver;
    public bool saveImage;
    public string location;
    int ID = 0;
    Texture2D imageTex;
    byte[] imageBytes;


    public override async void RunUnityFunction(string image)
    {

        if (quads == null)
        {
            quads = new List<GameObject>();
            unusedQuads = new List<GameObject>();
        }
        if (images == null)
        {
            images = new List<string>();
            IDs = new List<int>();
            textures = new List<Texture2D>();
            texturePool = new Texture2D[20];
            materialPool = new Material[20];
            for (int i = 0; i < texturePool.Length; i++)
            {
                texturePool[i] = new Texture2D(512, 512);
                materialPool[i] = new Material(Shader.Find("Unlit/FaderCircle"));
            }
        }
        if (scalar == null)
        {
            scalar = new GameObject("Scalar");
            noScale = new GameObject("NoScale");
            quadParent = new GameObject("unusedQuads");

            // usedMaterials = new List<Material>();
            // unusedMaterials = new List<Material>();
            for (int i = 0; i < 10; i++)
            {
                GameObject thisQuad = Instantiate(quad);
                thisQuad.name = "Quad_" + i;

                thisQuad.transform.parent = quadParent.transform;
                unusedQuads.Add(thisQuad);

                Material mat = GetMaterial();// new Material(Shader.Find("Unlit/FaderCircle"));// Instantiate(projectionMaterial) as Material;
                // mat.name = "material: " + id;
                mat.SetFloat("_Fade", 0);
                // unusedMaterials.Add(mat);
                // mat.SetTexture("_MainTex", tex);
                thisQuad.GetComponent<MeshRenderer>().material = mat;

            }
            quadParent.SetActive(false);
        }
        // images.Add(image);
        IDs.Add(id);
        ready = true;
        print("got new image");
        foreach (SDRenderChainLink l in link)
        {
            l.RunUnityFunction("");
        }

        await UseGetImageAsBase64Async(image);
        if (coroutine == null)
        {
            coroutine = StartCoroutine(ScaleUp());
        }
        if (saveImage)
        {
            saver.location = Application.dataPath + "/Images/" + location + "/Image_" + ID.ToString("D4") + ".png";
            ID++;
            print("Image Saved: " + Application.dataPath + "/Images/" + location + "/Image_" + ID.ToString("D4") + ".png");
        }
    }

    private async Task UseGetImageAsBase64Async(string image)
    {
        textures.Add(await GetImageAsBase64Async(image));
        // print(textures.Count);
        // Do something with the image string
    }

    Texture2D GetTexture()
    {
        texturePoolIndex++;
        if (texturePoolIndex >= texturePool.Length)
            texturePoolIndex = 0;
        return texturePool[texturePoolIndex];
    }

    Material GetMaterial()
    {
        materialPoolIndex++;
        if (materialPoolIndex >= materialPool.Length)
            materialPoolIndex = 0;
        return materialPool[materialPoolIndex];
    }

    private async Task<Texture2D> GetImageAsBase64Async(string image) // return Task<string>
    {

        imageBytes = await Converter(image);
        // Texture2D tex = new Texture2D(512, 512);
        Texture2D tex = GetTexture();
        tex.LoadImage(imageBytes);
        return tex;

    }

    async Task<byte[]> Converter(string image)
    {
        return await Task.Run(() => Convert.FromBase64String(image));
    }


    // IEnumerator Wait(string image){
    //     while()
    // }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    // IEnumerator SetupRenderer(){

    // }

    public void SaveImages()
    {
        print(textures.Count);
        DateTime dateTime = DateTime.Now; // Replace with your desired DateTime value
        string formattedDate = dateTime.ToString("yyyy_MM_dd_H-mm-ss");
        if (!Directory.Exists(Application.dataPath + "/Images"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Images");
        }
        string location = Application.dataPath + "/Images/Image_" + "_" + ID.ToString("D4") + "_" + formattedDate + ".png";
        ScreenCapture.CaptureScreenshot(location);
    }

    IEnumerator ScaleUp()
    {
        thisTime = Time.time;
        average = thisTime - prevTime;
        float count = 0;
        if (prevTime == 0)
            prevTime = thisTime - 7;


        GameObject thisQuad = unusedQuads[0];
        unusedQuads.RemoveAt(0);
        quads.Add(thisQuad);
        // thisQuad.name = "Quad_" + id;
        // id++;
        // thisQuad.GetComponent<SetSortingLayerAndOrder>().sortingOrder = id;
        // thisQuad.GetComponent<SetSortingLayerAndOrder>().OnValidate();
        // byte[] imageBytes = Convert.FromBase64String(images[0]);
        // print("Speed: " + average + " , " + images.Count);
        average = average / (float)textures.Count;//map((float)images.Count, 0, 10, 0, 20);
        // speed *= f;

        // images.RemoveAt(0);
        while (textures.Count == 0)
        {
            yield return null;
        }
        Texture2D tex = textures[0];
        textures.RemoveAt(0);
        // new Texture2D(512, 512);
        // tex.LoadImage(imageBytes);
        Material mat = thisQuad.GetComponent<MeshRenderer>().material;

        mat.SetTexture("_MainTex", tex);

        scalar.transform.localScale = Vector3.one;
        id = 0;
        if (quads != null)
        {
            foreach (GameObject g in quads)
            {
                g.transform.SetParent(scalar.transform, true);
                g.GetComponent<SetSortingLayerAndOrder>().sortingOrder = id;
                g.GetComponent<SetSortingLayerAndOrder>().OnValidate();
                id++;
            }
        }
        // print("Speed: " + speed);
        count = 1;
        while (count < 2)
        {
            speed = Mathf.Lerp(speed, average, Time.deltaTime * .1f);
            mat.SetFloat("_Fade", Mathf.Min(1, (count - 1) * 3));
            count += (Time.deltaTime / speed) * speedFade;
            // print(images.Count + " , " + "," + speed + "," + (images.Count > 0 ? 1 : speedFade));
            if (textures.Count == 0)
                speedFade = Mathf.Lerp(speedFade, 0, Time.deltaTime * .4f);
            else if (textures.Count == 1 && speedFade < 1)
                speedFade = Mathf.Lerp(speedFade, 1, Time.deltaTime * .5f);
            else if (textures.Count > 1 && speedFade < 2f)
                speedFade = Mathf.Lerp(speedFade, 1.5f, Time.deltaTime * .5f);
            scalar.transform.localScale = Vector3.one * map((Mathf.Pow(count, scalePower)), 1, 4, 1, 2);//* (Mathf.Pow(count, 2) - 1);
            // print((Vector3.one + Vector3.one * Mathf.Pow(count, 2)));
            yield return null;
        }
        bool remove = false;
        if (quads != null)
        {
            foreach (GameObject g in quads)
            {
                // print(scalar.transform.localScale.x + " vs this scale before " + g.transform.localScale.x);
                g.transform.SetParent(noScale.transform, true);
                // print(scalar.transform.localScale.x + " vs this scale after " + g.transform.localScale.x);

                if (g.transform.localScale.x > maxScale)
                {
                    remove = true;


                }
                // print("LocalScale: " + g.transform.localScale.x);
            }
        }
        if (remove)
        {
            GameObject g = quads[0];
            unusedQuads.Add(g);
            quads.RemoveAt(0);
            g.transform.parent = quadParent.transform;
            g.transform.localScale = Vector3.one;
            remove = false;
        }
        scalar.transform.localScale = Vector3.one;
        // foreach (SDRenderChainLink l in link)
        // {
        //     l.RunUnityFunction(image);
        // }
        prevTime = thisTime;
        while (textures.Count == 0)
        {
            yield return null;
            print("not ready");
        }

        StartCoroutine(ScaleUp());
    }




}
