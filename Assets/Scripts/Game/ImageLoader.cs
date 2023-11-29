using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class ImageLoader : MonoBehaviour
{
    public static ImageLoader Instance { get; private set; }

    public string directory;
    public string subDirectory;
    public string textureChannel = "_MainTex";

    // public SpriteRenderer title;
    // public SpriteRenderer bg;
    // public SpriteRenderer[] item;
    // public SpriteRenderer[] bonus;
    // public SpriteRenderer[] platform;
    // public SpriteRenderer[] boundary;

    public float maxStyles;
    public string styleNum;
    int chosenStyle;
    public int setStyle;
    public bool useSetStyle;

    private List<int> availableStyles = new List<int>();

    private void Awake()
    {
        // Check if instance already exists and if it's not this instance, destroy it
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // This is the first instance - make it the Singleton
            Instance = this;
            // DontDestroyOnLoad(gameObject); // This makes the object not be destroyed when reloading the scene
        }
    }

    private void Start()
    {
        ResetStyles();
    }

    // private void ResetStyles()
    // {
    //     for (int i = 1; i <= maxStyles; i++)
    //     {
    //         availableStyles.Add(i);
    //     }
    // }
    private void ResetStyles()
    {
        // Assume GetFilenames is a method that retrieves all the filenames from a specific directory
        string[] allFilenames = GetFilenames(directory);
        // Regex pattern to match any four-digit style index that is in between two underscores
        string pattern = @"_(\d{4})_";

        // Clear the previous list of available styles
        availableStyles.Clear();

        // HashSet to keep track of unique style numbers
        HashSet<int> uniqueStyleNumbers = new HashSet<int>();

        foreach (var filename in allFilenames)
        {
            MatchCollection matches = Regex.Matches(filename, pattern);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    // The style number is in the first capturing group
                    if (int.TryParse(match.Groups[1].Value, out int styleNum))
                    {
                        uniqueStyleNumbers.Add(styleNum);
                    }
                }
            }
        }

        // Convert the unique style numbers to the list
        availableStyles = uniqueStyleNumbers.ToList();

        // Optionally sort the list if order matters
        availableStyles.Sort();
    }


    public void SetRandomStyle()
    {
        if (availableStyles.Count == 0)
        {
            ResetStyles();
        }
        if (availableStyles.Count != 0)
        {
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

            styleNum = chosenStyle.ToString("D4");
            // print("styleNum: " + styleNum + " Chosen Style: " + chosenStyle);
        }
        else
        {
            styleNum = 0.ToString("D4");
            Debug.LogWarning("No styles available.");
        }

    }

    // public Texture2D GetImageWithIndex(string imageType, int index = -1){

    //     string resourcePath = directory + styleNum + "/" + imageType + "/";
    //     Debug.Log("Loading from resource path: " + resourcePath);
    //     Texture2D[] textures = Resources.LoadAll<Texture2D>(resourcePath);
    //     if(index==-1){
    //         index = Random.Range(0,textures.Length);
    //     }
    //     return textures[index];
    // }

    public int GetRandomStyleNum(string imageType)
    {
        // Assuming `GetFilenames` retrieves all filenames from the specified directory
        string[] allFilenames = GetFilenames(directory);
        // Adjust the pattern to match "Abstract_Background_0007_42" format
        string pattern = $"{imageType}_([0-9]{{4}})_";

        List<int> foundStyleNums = new List<int>();

        foreach (var filename in allFilenames)
        {
            Match match = Regex.Match(filename, pattern);
            if (match.Success)
            {
                if (int.TryParse(match.Groups[1].Value, out int styleNum))
                {
                    foundStyleNums.Add(styleNum);
                }
            }
        }

        if (foundStyleNums.Count == 0)
        {
            Debug.LogError("No matching style numbers found.");
            return -1; // Or handle this case as you see fit
        }

        int randomIndex = UnityEngine.Random.Range(0, foundStyleNums.Count);
        return foundStyleNums[randomIndex];
    }

    private string[] GetFilenames(string resourceFolder)
    {
        // Load all objects in the specified Resources folder
        Object[] loadedObjects = Resources.LoadAll(resourceFolder);

        // Filter the objects to only include Texture2D objects (images)
        Texture2D[] textures = loadedObjects.OfType<Texture2D>().ToArray();

        // Select the names of the textures
        string[] filenames = textures.Select(texture => texture.name).ToArray();
        return filenames;
    }

    public Sprite GetSpriteWithIndex(string imageType, int index = -1)
    {
        Texture2D tex = GetImageWithIndex(imageType, index);
        // Check if tex is null
        if (tex == null)
        {
            // Create a 1x1 white texture
            tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
        }
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * .5f);
    }



    public Texture2D GetImageWithIndex(string imageType, int index = -1)
    {
        // Format styleNum with padding, assuming it needs to be 4 digits long
        string styleNumFormatted = styleNum;

        // Construct the partial filename with styleNum
        string partialFileName = $"_{styleNumFormatted}_";

        // print(directory + " " + " " + styleNum + " " + partialFileName + " " + imageType);

        // Get all files in the directory
        Texture2D[] textures = Resources.LoadAll<Texture2D>(directory)
            .Where(t => t.name.Contains(styleNum) && t.name.Contains(imageType)).ToArray();

        // Log how many textures were found
        // Debug.Log("Number of textures found: " + textures.Length + " index: " + index);

        // If there are no textures, return null
        if (textures.Length == 0)
        {
            Debug.LogWarning("No textures found with the given criteria.");
            return null;
        }

        // Choose a random texture if index is -1 or if it is out of bounds
        if (index == -1 || index >= textures.Length)
        {
            index = Random.Range(0, textures.Length);
        }

        // Return the selected texture
        // Check if tex is null
        if (textures[index] == null)
        {
            // Create a 1x1 white texture
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            return tex;
        }
        else
            return textures[index];
    }

    public Sprite GetSpriteWithIndexSeed(string imageType, int index = -1, int globalSeed = -1)
    {
        Texture2D tex = GetImageWithIndexSeed(imageType, index, globalSeed);
        // Check if tex is null
        if (tex == null)
        {
            // Create a 1x1 white texture
            tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
        }
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * .5f);
    }

    public Texture2D GetImageWithIndexSeed(string imageType, int imageSeed = -1, int globalSeed = -1)
    {
        // Format styleNum with padding, assuming it needs to be 4 digits long
        // string styleNumFormatted = styleNum;

        // Construct the partial filename with styleNum
        // string partialFileName = $"_{styleNumFormatted}_";

        // print(directory + " " + " " + styleNum + " " + partialFileName + " " + imageType);

        // Get all files in the directory
        Texture2D[] textures = Resources.LoadAll<Texture2D>(directory)
            .Where(t => t.name.Contains(styleNum) && t.name.Contains(imageType) && t.name.EndsWith("_" + globalSeed.ToString())).ToArray();

        // Log how many textures were found
        // Debug.Log("Number of textures found: " + textures.Length + " index: " + imageSeed + " globalSeed: " + globalSeed);

        // If there are no textures, return null
        if (textures.Length == 0)
        {
            Debug.LogWarning("No textures found with the given criteria.");
            return null;
        }

        // Choose a random texture if index is -1 or if it is out of bounds
        if (imageSeed == -1 || imageSeed >= textures.Length)
        {
            imageSeed = Random.Range(0, textures.Length);
        }

        // Return the selected texture
        return textures[imageSeed];
    }

    public void AssignRandomImage(Renderer renderer, string imageType, bool setMaterial = false)
    {

        string resourcePath = directory + styleNum + "/" + imageType + "/";
        Debug.Log("Loading from resource path: " + resourcePath);
        Texture2D[] textures = Resources.LoadAll<Texture2D>(resourcePath);
        print(textures.Length);

        if (textures.Length > 0)
        {
            Texture2D chosenTexture = textures[Random.Range(0, textures.Length)];

            if (renderer is SpriteRenderer spriteRenderer)
            {
                if (!setMaterial)
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

    public void AssignImage(Renderer renderer, string imageType, int index, bool setMaterial = false)
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
            Texture2D chosenTexture = textures[index];//Random.Range(0, textures.Length)];

            if (renderer is SpriteRenderer spriteRenderer)
            {
                if (!setMaterial)
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

    public void SetSprite(SpriteRenderer spriteRenderer, string imageType, int index = -1)
    {
        Texture2D chosenTexture = ImageLoader.Instance.GetImageWithIndex(imageType, index);
        spriteRenderer.sprite = Sprite.Create(chosenTexture, new Rect(0, 0, chosenTexture.width, chosenTexture.height), new Vector2(0.5f, 0.5f));
        spriteRenderer.material.SetTexture("_MainTex", chosenTexture);
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
