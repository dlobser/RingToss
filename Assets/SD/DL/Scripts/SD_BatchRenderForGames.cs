using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text.RegularExpressions;

[Serializable]
public class ArtDescription
{
    public string Name;
    public int Seed;
    public string ArtistName;
    public string BackgroundPrompt;
    public string PlatformPrompt;
    public string ItemPrompt;
    public string BonusItemPrompt;
    public string WallsPrompt;
    public string TitlePrompt;
    public string EmitterPrompt;
    public string ProjectilePrompt;
}

[Serializable]
public class ArtDescriptionWrapper
{
    public ArtDescription[] artDescriptions;
}


public class SD_BatchRenderForGames : SDRenderChainLink
{
    // TextAsset jsonData = Resources.Load<TextAsset>("artDescriptions");
    // string jsonString = jsonData.text;
    ArtDescription[] artDescriptions;// = JsonUtility.FromJson<List<ArtDescription>>(jsonString);
    public TextMesh textMesh; // Assign this via the Inspector or some other method
    public Dictionary<string, SDRenderChainLinkPrompt> promptLinks;
    public SDRenderChainLinkRenderDepth textRenderer;
    public SDRenderChainLink startingLink;
    public ExtraValuesForImg2Img[] extraValuesImg2Img;
    public ExtraValuesForTxt2Image[] extraValuesTxt2Img;
    public int seedStart;
    public int seedIterate = 1;
    int seedIterationCounter = 0;
    public int seedIterationsPerSet = 1;
    int seed = 0;
    int ID = 0;
    // [TextArea]
    // public string prompt;
    // public string[] promptOptions;

    public SDRenderChainLinkSave[] saveLinks;
    public string[] saveNames;
    // public SDRenderChainLinkPrompt[] promptLinks;
    // public string[] prompts;
    // public SDRenderChainLinkSave writeBG;
    // public SDRenderChainLinkSave writeMG;
    // public SDRenderChainLinkSave writeFG;

    public string rootDirectory;
    public string familyName;
    public bool render;

    // public TMPro.TMP_FontAsset[] fonts;
    public Font[] fonts;

    int artDescriptionIndex = 0;


    void Start()
    {
        string path = Application.dataPath + "/Resources/LevelsJSON.json";
        string jsonString = File.ReadAllText(path);

        // ... during deserialization:
        ArtDescriptionWrapper wrapper = JsonUtility.FromJson<ArtDescriptionWrapper>("{\"artDescriptions\":" + jsonString + "}");
        artDescriptions = wrapper.artDescriptions;

        // List<ArtDescription> artDescriptions = JsonUtility.FromJson<List<ArtDescription>>(jsonString);


        // TextAsset jsonData = Resources.Load<TextAsset>("LevelsJSON");
        // string jsonString = jsonData.text;
        // artDescriptions = JsonUtility.FromJson<List<ArtDescription>>(jsonString);

        seed = seedStart;
        RunUnityFunction("");

    }

    void Update()
    {

    }

    public Transform FindChildRecursive(Transform parent, string childName)
    {
        // Check immediate children first
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }
        }

        // If not found, dig deeper
        foreach (Transform child in parent)
        {
            Transform found = FindChildRecursive(child, childName);
            if (found != null)
            {
                return found;
            }
        }

        // If child wasn't found in any branch of the tree, return null
        return null;
    }


    void SetPromptValue(string childName, string value)
    {
        print("Child Name: " + childName);
        Transform childTransform = FindChildRecursive(this.transform, childName);
        
        if (childTransform != null)
        {
            SDRenderChainLinkPrompt extraValueComponent = childTransform.GetComponent<SDRenderChainLinkPrompt>();
            if (extraValueComponent != null)
            {
                extraValueComponent.prompt = value; 
                print("Found Child: " + childTransform.name + ", " + value);
                // if(extraValueComponent.extraValuesTxt2Img!=null){
                //     extraValueComponent.extraValuesTxt2Img.prompt = value;
                // }
                // if(extraValueComponent.extraValuesImg2Img!=null){
                //     extraValueComponent.extraValuesImg2Img.prompt = value;
                // }
            }
        }
    }

    public void UpdatePrompts()
    {
        print("art descriptions length: " + artDescriptions.Length);
        if (artDescriptionIndex < artDescriptions.Length)
        {
            var art = artDescriptions[artDescriptionIndex];

            if(textMesh.GetComponent<SetTextMeshWidth>()!=null)
                textMesh.GetComponent<SetTextMeshWidth>().SetText(art.Name);
            else
                textMesh.text = art.Name;
            
            if(fonts.Length>0){
                textMesh.font = fonts[UnityEngine.Random.Range(0, fonts.Length)];
                textMesh.GetComponent<MeshRenderer>().sharedMaterial = textMesh.font.material;
            }

            SetPromptValue("BackgroundPrompt", art.BackgroundPrompt + " by " + art.ArtistName);
            SetPromptValue("PlatformPrompt", art.PlatformPrompt + " by " + art.ArtistName);
            SetPromptValue("ItemPrompt", art.ItemPrompt + " by " + art.ArtistName);
            SetPromptValue("BonusItemPrompt", art.BonusItemPrompt + " by " + art.ArtistName);
            SetPromptValue("WallsPrompt", art.WallsPrompt + " by " + art.ArtistName);
            SetPromptValue("TitlePrompt", art.TitlePrompt + " by " + art.ArtistName);
            SetPromptValue("EmitterPrompt", art.EmitterPrompt + " by " + art.ArtistName);
            SetPromptValue("ProjectilePrompt", art.ProjectilePrompt + " by " + art.ArtistName);

            // The rest of your function's code...

            // artDescriptionIndex++;
        }
        else
        {
            Debug.Log("All art descriptions have been used.");
        }
    }

    public override void RunUnityFunction(string image)
    {
        if (render && artDescriptionIndex < artDescriptions.Length)
        {
            // string thisPrompt = GeneratePrompt(prompt);
            // print(thisPrompt);

            // print("Seed: " + seed);

            // for (int i = 0; i < promptLinks.Length; i++)
            // {
            //     promptLinks[i].prompt = ReplaceStrings(prompts[i], promptOptions);
            // }
            UpdatePrompts();
            CreateSDRenderChainLinkSaveForChildren();


            if (extraValuesImg2Img != null )
            {
                foreach (ExtraValuesForImg2Img v in extraValuesImg2Img)
                    v.seed =  artDescriptions[artDescriptionIndex].Seed + seedIterationCounter;
            }
            if (extraValuesTxt2Img != null)
            {
                foreach (ExtraValuesForTxt2Image v in extraValuesTxt2Img)
                    v.seed = artDescriptions[artDescriptionIndex].Seed + seedIterationCounter;
            }

            // for (int i = 0; i < link.Length; i++)
            // {
            //     ExtraValues extraValues = link[i].extraValues;
            //     if (extraValues is ExtraValuesForImg2Img)
            //     {
            //         // ExtraValuesForImg2Img extraValuesImg2Img = (ExtraValuesForImg2Img)extraValues;
            //         // Modify members of ExtraValuesForImg2Img using extraValuesImg2Img

            //     }

            // }
            if(textRenderer!=null)
                textRenderer.saveDepthImageLocation = rootDirectory + "/" + "TextDepth" + "_" + artDescriptionIndex.ToString("0000") + "_" + seed.ToString()+ ".png";
            // for (int i = 0; i < saveLinks.Length; i++)
            // {
            //     saveLinks[i].location = rootDirectory + "/" + familyName + "_" + ID.ToString("D4") + "_" + seed.ToString() + "_" + saveNames[i] + ".png";
            // }
            // writeBG.location = rootDirectory + "/" + ID.ToString("D4") + "_" + familyName + "_BG_" + seed.ToString("D4") + ".png";
            // writeMG.location = rootDirectory + "/" + ID.ToString("D4") + "_" + familyName + "_MG_" + seed.ToString("D4") + ".png";
            // writeFG.location = rootDirectory + "/" + ID.ToString("D4") + "_" + familyName + "_FG_" + seed.ToString("D4") + ".png";
            startingLink.RunUnityFunction("");
            seed += seedIterate;
            
            GlobalSettings.randomSeed = seed;
            UnityEngine.Random.InitState(seed);

            ID++;
            if(seedIterationCounter < seedIterationsPerSet){
                seedIterationCounter ++;
            }
            else{
                seedIterationCounter=0;
                artDescriptionIndex++;
            }
            
        }
    }

    public void CreateSDRenderChainLinkSaveForChildren()
    {
        Transform root = this.transform;
        // Get the immediate children of the root
        // int childCount = root.childCount;
        SDRenderChainLinkSave[] sdRenderChainLinkSaves = GameObject.FindObjectsOfType<SDRenderChainLinkSave>();

        foreach(SDRenderChainLinkSave save in sdRenderChainLinkSaves){
            save.location = rootDirectory + "" + artDescriptions[artDescriptionIndex].Name.Split(' ')[0] + "_" + save.transform.parent.parent.name.Replace("Prompt", "") + "_" + artDescriptionIndex.ToString("0000") + "_" + seed.ToString()+ ".png";
        }
    }


}
