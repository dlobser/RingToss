using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text.RegularExpressions;
using TMPro;

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
 
    ArtDescription[] artDescriptions;
    public TextMeshProUGUI textMesh; 
    public Dictionary<string, SDRenderChainLinkPrompt> promptLinks;
    public SDRenderChainLinkRenderDepth textRenderer;
    public SDRenderChainLink startingLink;
    public ExtraValuesForImg2Img[] extraValuesImg2Img;
    public ExtraValuesForTxt2Image[] extraValuesTxt2Img;
    public int seedStart;
    public int seedIterate = 1;
    int seedIterationCounter = 0;
    int gameRandomSeed = 0;
    public int seedIterationsPerSet = 1;
    public int imageSeed = 0;
    // int ID = 0;

    public SDRenderChainLinkSave[] saveLinks;
    public string[] saveNames;

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

        ArtDescriptionWrapper wrapper = JsonUtility.FromJson<ArtDescriptionWrapper>("{\"artDescriptions\":" + jsonString + "}");
        artDescriptions = wrapper.artDescriptions;

        imageSeed = seedStart;
        UpdatePrompts();
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
        Transform childTransform = FindChildRecursive(this.transform, childName);
        
        if (childTransform != null)
        {
            SDRenderChainLinkPrompt extraValueComponent = childTransform.GetComponent<SDRenderChainLinkPrompt>();
            if (extraValueComponent != null)
            {
                extraValueComponent.prompt = value; 
            }
        }
    }

    public void ApplySelectedFont(TextMeshProUGUI textMeshPro, string resourcePath, int fontIndex)
    {
        TMP_FontAsset[] fonts = Resources.LoadAll<TMP_FontAsset>(resourcePath);
        if (fonts.Length > fontIndex && fontIndex >= 0)
        {
            textMeshPro.font = fonts[fontIndex];
        }
        else
        {
            Debug.LogError("Invalid font index or TMP font not found.");
        }
    }

    public void UpdatePrompts()
    {
        if (artDescriptionIndex < artDescriptions.Length)
        {
            var art = artDescriptions[artDescriptionIndex];

            int fontsAmount = Resources.LoadAll<TMP_FontAsset>("Fonts").Length;
            if(fonts.Length>0){
                ApplySelectedFont(textMesh,"Fonts",gameRandomSeed % fontsAmount);
            }

            if(textMesh.GetComponent<SetTextMeshWidth>()!=null){
                textMesh.GetComponent<SetTextMeshWidth>().SetText(art.Name);
                textMesh.GetComponent<SetTextMeshWidth>().FitTextToWidth();
            }
            else
                textMesh.text = art.Name;

            SetPromptValue("BackgroundPrompt", art.BackgroundPrompt + " by " + art.ArtistName);
            SetPromptValue("PlatformPrompt", art.PlatformPrompt + " by " + art.ArtistName);
            SetPromptValue("ItemPrompt", art.ItemPrompt + " by " + art.ArtistName);
            SetPromptValue("BonusItemPrompt", art.BonusItemPrompt + " by " + art.ArtistName);
            SetPromptValue("WallsPrompt", art.WallsPrompt + " by " + art.ArtistName);
            SetPromptValue("TitlePrompt", art.TitlePrompt + " by " + art.ArtistName);
            SetPromptValue("EmitterPrompt", art.EmitterPrompt + " by " + art.ArtistName);
            SetPromptValue("ProjectilePrompt", art.ProjectilePrompt + " by " + art.ArtistName);
        }
    }

    IEnumerator RunUnityFunctionCoroutine(){
        if (render && seedIterationCounter < seedIterationsPerSet)//artDescriptionIndex < artDescriptions.Length)
        {

            UpdatePrompts();
            print("ImageSeed before yield: " + imageSeed + " - " + gameRandomSeed);
            yield return null;
            CreateSDRenderChainLinkSaveForChildren();

            SDRenderChainLink[] links = GetComponentsInChildren<SDRenderChainLink>();
            foreach(SDRenderChainLink link in links){
                if (link.extraValues is ExtraValuesForImg2Img img2ImgValues)
                {
                    img2ImgValues.seed = imageSeed; // Or any other logic to set the seed
                }
                else if (link.extraValues is ExtraValuesForTxt2Image txt2ImgValues)
                {
                    txt2ImgValues.seed = imageSeed; // Or any other logic to set the seed
                }
            }

            // if (extraValuesImg2Img != null )
            // {
            //     foreach (ExtraValuesForImg2Img v in extraValuesImg2Img){
            //         v.seed =  imageSeed;//artDescriptions[artDescriptionIndex].Seed + seedIterationCounter;
            //     }
            // }
            // if (extraValuesTxt2Img != null)
            // {
            //     foreach (ExtraValuesForTxt2Image v in extraValuesTxt2Img)
            //         v.seed = imageSeed;//artDescriptions[artDescriptionIndex].Seed + seedIterationCounter;
            // }
            
            print("ImageSeed after yield: " + imageSeed + " - " + gameRandomSeed);
            if(textRenderer!=null)
                textRenderer.saveDepthImageLocation = rootDirectory + "/" + artDescriptions[artDescriptionIndex].Name.Split(' ')[0] + "_IntroTextAlpha" + "_" + 
                artDescriptionIndex.ToString("0000") + "_" + imageSeed.ToString() + "_" + gameRandomSeed + ".png";

            imageSeed += seedIterate;

            GlobalSettings.randomSeed = gameRandomSeed;
            UnityEngine.Random.InitState(gameRandomSeed);

            // ID++;

            // if(seedIterationCounter < seedIterationsPerSet){
                seedIterationCounter ++;
            // }
            // else{
            //     seedIterationCounter=0;
            //     artDescriptionIndex++;
            // }
            
            startingLink.RunUnityFunction("");
            
        }
        else
        {
            imageSeed += seedIterate;

            // SDRenderChainLink[] links = GetComponentsInChildren<SDRenderChainLink>();
            // foreach(SDRenderChainLink link in links){
            //     if (link.extraValues is ExtraValuesForImg2Img img2ImgValues)
            //     {
            //         img2ImgValues.seed = imageSeed; // Or any other logic to set the seed
            //     }
            //     else if (link.extraValues is ExtraValuesForTxt2Image txt2ImgValues)
            //     {
            //         txt2ImgValues.seed = imageSeed; // Or any other logic to set the seed
            //     }
            // }

            print("ImageSeed after else: " + imageSeed + " - " + gameRandomSeed);
            seedIterationCounter=0;
            artDescriptionIndex ++;
            if(artDescriptionIndex >= artDescriptions.Length){
                Debug.Log("All art descriptions have been used.");
                artDescriptionIndex=0;
            }
            gameRandomSeed++;
            GlobalSettings.randomSeed = gameRandomSeed;
            UnityEngine.Random.InitState(gameRandomSeed);
            CreateSDRenderChainLinkSaveForChildren();

            SDRenderChainLink[] links = GetComponentsInChildren<SDRenderChainLink>();
            foreach(SDRenderChainLink link in links){
                if (link.extraValues is ExtraValuesForImg2Img img2ImgValues)
                {
                    img2ImgValues.seed = imageSeed; // Or any other logic to set the seed
                }
                else if (link.extraValues is ExtraValuesForTxt2Image txt2ImgValues)
                {
                    txt2ImgValues.seed = imageSeed; // Or any other logic to set the seed
                }
            }

            startingLink.RunUnityFunction("");
        }
    }

    public override void RunUnityFunction(string image)
    {
        StartCoroutine(RunUnityFunctionCoroutine());
        // if (render && seedIterationCounter < seedIterationsPerSet)//artDescriptionIndex < artDescriptions.Length)
        // {

        //     UpdatePrompts();
        //     CreateSDRenderChainLinkSaveForChildren();


        //     if (extraValuesImg2Img != null )
        //     {
        //         foreach (ExtraValuesForImg2Img v in extraValuesImg2Img)
        //             v.seed =  artDescriptions[artDescriptionIndex].Seed + seedIterationCounter;
        //     }
        //     if (extraValuesTxt2Img != null)
        //     {
        //         foreach (ExtraValuesForTxt2Image v in extraValuesTxt2Img)
        //             v.seed = artDescriptions[artDescriptionIndex].Seed + seedIterationCounter;
        //     }

        //     if(textRenderer!=null)
        //         textRenderer.saveDepthImageLocation = rootDirectory + "/" + artDescriptions[artDescriptionIndex].Name.Split(' ')[0] + "_TitleAlpha" + "_" + artDescriptionIndex.ToString("0000") + "_" + seed.ToString()+ ".png";

        //     seed += seedIterate;

        //     GlobalSettings.randomSeed = gameRandomSeed;
        //     UnityEngine.Random.InitState(seed);

        //     ID++;

        //     if(seedIterationCounter < seedIterationsPerSet){
        //         seedIterationCounter ++;
        //     }
        //     else{
        //         seedIterationCounter=0;
        //         artDescriptionIndex++;
        //     }
            
        //     startingLink.RunUnityFunction("");
            
        // }
        // else
        // {
        //     artDescriptionIndex = 0;
        //     gameRandomSeed++;
        //     Debug.Log("All art descriptions have been used.");
        //     GlobalSettings.randomSeed = gameRandomSeed;
        //     UnityEngine.Random.InitState(seed);
        //     startingLink.RunUnityFunction("");
        // }
    }

    public void CreateSDRenderChainLinkSaveForChildren()
    {
        Transform root = this.transform;
        // Get the immediate children of the root
        // int childCount = root.childCount;
        SDRenderChainLinkSave[] sdRenderChainLinkSaves = GameObject.FindObjectsOfType<SDRenderChainLinkSave>();
        MyDebug.Instance.Log(artDescriptionIndex+"");
        foreach(SDRenderChainLinkSave save in sdRenderChainLinkSaves){
            save.location = rootDirectory + "" + artDescriptions[artDescriptionIndex].Name.Split(' ')[0] + "_" + save.transform.name.Replace("Save", "") 
            + "_" + artDescriptionIndex.ToString("0000") + "_" + imageSeed.ToString() + "_" + gameRandomSeed + ".png";
        }
    }


}
