using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Scalar : MonoBehaviour
{
    public Material projectionMaterial;
    public GameObject quad;
    public float speed;
    GameObject scalar;
    GameObject noScale;
    List<GameObject> quads;
    // List<string> images;
    // List<Texture2D> textures;
    public int which = 0;
    public int total = 850;
    string[] filePaths;

    public bool useResources;
    private Object[] textures;

    public string extension = ".png";


    // List<int> IDs;
    // bool ready = false;
    int id = 0;

    // float thisTime;
    // float prevTime;
    // float average;

    public float scalePower = 2;

    // Coroutine coroutine;
    // float speedFade = 1;

    GameObject quadParent;
    // // List<Material> usedMaterials;
    // // List<Material> unusedMaterials;
    List<GameObject> unusedQuads;

    public float maxScale = 32;

    // public SDRenderChainLinkSave saver;
    // public bool saveImage;
    public string location;

    public Shader unlitShader;
    // string[] filePaths;
    // int ID = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (useResources)
        {
            // textures = Resources.LoadAll(location, typeof(Texture2D));
            // print(textures.Length);
        }
        else
        {
            filePaths = Directory.GetFiles(location, "*" + extension);
        }
        // foreach (string filePath in filePaths)
        // {
        //     // Debug.Log(filePath);
        // }

        if (quads == null)
        {
            quads = new List<GameObject>();
            unusedQuads = new List<GameObject>();
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
                thisQuad.transform.localPosition = Vector3.zero;
                unusedQuads.Add(thisQuad);

                Material mat = new Material(unlitShader);// Shader.Find("Unlit/FaderCircle"));// Instantiate(projectionMaterial) as Material;
                mat.name = "material: " + id;
                mat.SetFloat("_Fade", 0);
                // unusedMaterials.Add(mat);
                // mat.SetTexture("_MainTex", tex);
                thisQuad.GetComponent<MeshRenderer>().material = mat;

            }
            quadParent.SetActive(false);
        }
        StartCoroutine(ScaleUp());
    }

    // Update is called once per frame
    void Update()
    {

    }

    Texture2D GetTexture(string img)
    {
        byte[] imageData = File.ReadAllBytes(img);
        Texture2D imageTexture = new Texture2D(2, 2); // Set initial size to 2x2 to avoid warnings
        imageTexture.LoadImage(imageData);
        return imageTexture;
    }

    IEnumerator ScaleUp()
    {
        // thisTime = Time.time;
        // average = thisTime - prevTime;
        float count = 0;
        // if (prevTime == 0)
        //     prevTime = thisTime - 7;


        GameObject thisQuad = unusedQuads[0];
        unusedQuads.RemoveAt(0);
        quads.Add(thisQuad);
        // thisQuad.name = "Quad_" + id;
        // id++;
        // thisQuad.GetComponent<SetSortingLayerAndOrder>().sortingOrder = id;
        // thisQuad.GetComponent<SetSortingLayerAndOrder>().OnValidate();
        // byte[] imageBytes = Convert.FromBase64String(images[0]);
        // print("Speed: " + average + " , " + images.Count);
        // average = average / (float)textures.Count;//map((float)images.Count, 0, 10, 0, 20);
        // speed *= f;

        // images.RemoveAt(0);
        // while (textures.Count == 0)
        // {
        //     yield return null;
        // }Assets/Resources/depth/Image_0246.png

        // print(filePaths[which]);
        Texture2D tex;
        if (!useResources)
            tex = GetTexture(filePaths[which]);
        else
            tex = (Texture2D)Resources.Load(location + "Image_" + which.ToString("D4"));//(Texture2D)textures[which];

        // print(location + "Image_" + which.ToString("D4") + ".png");
        print(tex.width);
        which++;
        if (!useResources)
        {
            if (which > filePaths.Length - 1)
            {
                which = 0;
            }
        }
        else
        {
            if (which > total)
            {
                which = 0;
            }
        }
        // textures[0];
        // textures.RemoveAt(0);
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
            // speed = Mathf.Lerp(speed, average, Time.deltaTime * .1f);
            mat.SetFloat("_Fade", Mathf.Min(1, (count - 1) * 3));
            count += (Time.deltaTime / speed);// * speedFade;
            // print(images.Count + " , " + "," + speed + "," + (images.Count > 0 ? 1 : speedFade));
            // if (textures.Count == 0)
            //     speedFade = Mathf.Lerp(speedFade, 0, Time.deltaTime * .4f);
            // else if (textures.Count == 1 && speedFade < 1)
            //     speedFade = Mathf.Lerp(speedFade, 1, Time.deltaTime * .5f);
            // else if (textures.Count > 1 && speedFade < 2f)
            //     speedFade = Mathf.Lerp(speedFade, 1.5f, Time.deltaTime * .5f);
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
        // prevTime = thisTime;
        // while (textures.Count == 0)
        // {
        //     yield return null;
        //     print("not ready");
        // }

        StartCoroutine(ScaleUp());
    }
    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
