using UnityEngine;
using System.Collections.Generic;

public class ImageLoader : MonoBehaviour
{
    public string directory;
    public string subDirectory;
    public string textureChannel = "_MainTex";

    public SpriteRenderer title;
    public SpriteRenderer bg;
    public SpriteRenderer[] item;
    public SpriteRenderer[] bonus;
    public SpriteRenderer[] platform;
    public SpriteRenderer[] boundary;

    public float maxStyles;
    private string styleNum;
    public int setStyle;
    public bool useSetStyle;

    private List<int> availableStyles = new List<int>();

    private void Start()
    {
        ResetStyles();
    }

    private void ResetStyles()
    {
        for (int i = 1; i <= maxStyles; i++)
        {
            availableStyles.Add(i);
        }
    }

    public void SetRandomStyle()
    {
        if (availableStyles.Count == 0)
        {
            ResetStyles();
        }

        int index = Random.Range(0, availableStyles.Count);
        int chosenStyle = availableStyles[index];

        if (useSetStyle)
        {
            chosenStyle = setStyle;
            availableStyles.Remove(chosenStyle);
        }
        else
        {
            availableStyles.RemoveAt(index);
        }

        styleNum = chosenStyle.ToString("00");
        print("styleNum: " + styleNum + " Chosen Style: " + chosenStyle);
    }

    public void AssignRandomImage(Renderer renderer, string imageType, bool setMaterial = false)
    {
        Texture testTexture = Resources.Load<Texture>("Textures/Style_01/Background/mntns");
        Debug.Log("Absolute path to Resources: " + System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, "Resources")));
        if (testTexture == null)
        {
            Debug.LogError("Failed to load the test texture.");
        }
        else
        {
            Debug.Log("Successfully loaded the test texture.");
        }

        string resourcePath = directory + styleNum + "/" + imageType + "/";
        Debug.Log("Loading from resource path: " + resourcePath);
        Texture2D[] textures = Resources.LoadAll<Texture2D>(resourcePath);
        print(textures.Length);
        
        if (textures.Length > 0)
        {
            Texture2D chosenTexture = textures[Random.Range(0, textures.Length)];

            if (renderer is SpriteRenderer spriteRenderer)
            {
                if(!setMaterial)
                    spriteRenderer.sprite = Sprite.Create(chosenTexture, new Rect(0, 0, chosenTexture.width, chosenTexture.height), new Vector2(0.5f, 0.5f));
                else
                    spriteRenderer.material.SetTexture(textureChannel, chosenTexture);
                
                print("assigned sprite");
            }
            else if (renderer is MeshRenderer meshRenderer)
            {
                if (meshRenderer.materials.Length > 0)
                {
                    meshRenderer.materials[0].SetTexture(textureChannel, chosenTexture);
                    print("assigned meshrenderer");
                }
                else
                {
                    Debug.LogWarning("MeshRenderer has no materials assigned.");
                }
            }
            else
            {
                Debug.LogWarning("Unsupported renderer type.");
            }
        }
        else
        {
            Debug.LogWarning("No textures found in the specified Resources directory.");
        }
    }

}

// using System.IO;
// using UnityEngine;
// using System.Collections.Generic;

// public class ImageLoader : MonoBehaviour
// {
//     // public SpriteRenderer spriteRenderer;
//     public string directory;
//     public string subDirectory;
//     public string textureChannel = "_MainTex";

//     public SpriteRenderer title;
//     public SpriteRenderer bg;
//     public SpriteRenderer[] item;
//     public SpriteRenderer[] bonus;
//     public SpriteRenderer[] platform;
//     public SpriteRenderer[] boundary;

//     public float maxStyles;
//     private string styleNum;
//     public int setStyle;
//     public bool useSetStyle;

//     private List<int> availableStyles = new List<int>();


//     private void Start()
//     {
//         ResetStyles();
//     }

//     public void AssignImages(){

//     }

//     private void ResetStyles()
//     {
//         for (int i = 1; i <= maxStyles; i++)
//         {
//             availableStyles.Add(i);
//         }
//     }


//     // public void SetRandomStyle(){
//     //     float randInt = Random.value*maxStyles+1;
//     //     int rint = (int)randInt;
//     //     if(useSetStyle) rint = setStyle;
//     //     styleNum = rint.ToString("00");
//     //     print("styleNum: " + styleNum + " randint " + randInt + " " + Random.value*44);
//     // }
    
//     public void SetRandomStyle()
//     {
//         if (availableStyles.Count == 0)
//         {
//             ResetStyles();
//         }

//         int index = Random.Range(0, availableStyles.Count);
//         int chosenStyle = availableStyles[index];

//         if (useSetStyle)
//         {
//             chosenStyle = setStyle;
//             availableStyles.Remove(chosenStyle);
//         }
//         else
//         {
//             availableStyles.RemoveAt(index);
//         }

//         styleNum = chosenStyle.ToString("00");
//         print("styleNum: " + styleNum + " Chosen Style: " + chosenStyle);
//     }

//     public void AssignRandomImage(Renderer renderer, string imageType, bool setMaterial = false)
//     {
        
//         string thisDirectory = Application.dataPath + directory + styleNum + "/" + imageType + "/";
//         print("Checking in: " + thisDirectory);
        
//         if (Directory.Exists(thisDirectory))
//         {
//             string[] pngFiles = Directory.GetFiles(thisDirectory, "*.png");
//             if (pngFiles.Length > 0)
//             {
//                 string randomPngFile = pngFiles[Random.Range(0, pngFiles.Length)];
//                 Texture2D texture = new Texture2D(2, 2);
//                 byte[] fileData = File.ReadAllBytes(randomPngFile);
//                 texture.LoadImage(fileData);

//                 if (renderer is SpriteRenderer spriteRenderer)
//                 {
//                     if(!setMaterial)
//                         spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
//                     else
//                         spriteRenderer.material.SetTexture(textureChannel,texture);
//                     print("assigned sprite");
//                 }
//                 else if (renderer is MeshRenderer meshRenderer)
//                 {
//                     if (meshRenderer.materials.Length > 0)
//                     {
//                         meshRenderer.materials[0].SetTexture(textureChannel,texture);
//                         print("assigned meshrenderer");
//                     }
//                     else
//                     {
//                         Debug.LogWarning("MeshRenderer has no materials assigned.");
//                     }
//                 }
//                 else
//                 {
//                     Debug.LogWarning("Unsupported renderer type.");
//                 }
//             }
//             else
//             {
//                 Debug.LogWarning("No PNG files found in the specified directory.");
//             }
//         }
//         else
//         {
//             Debug.LogError("The specified directory does not exist.");
//         }
//     }

// }
