using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;


public class SD_BatchRender : SDRenderChainLink
{
    public SDRenderChainLink startingLink;
    public ExtraValuesForImg2Img[] extraValuesImg2Img;
    public ExtraValuesForTxt2Image[] extraValuesTxt2Img;
    public int seedStart;
    public int seedIterate = 1;
    int seed = 0;
    int ID = 0;
    // [TextArea]
    // public string prompt;
    public string[] promptOptions;

    public SDRenderChainLinkSave[] saveLinks;
    public string[] saveNames;
    public SDRenderChainLinkPrompt[] promptLinks;
    public string[] prompts;
    // public SDRenderChainLinkSave writeBG;
    // public SDRenderChainLinkSave writeMG;
    // public SDRenderChainLinkSave writeFG;

    public string rootDirectory;
    public string familyName;
    public bool render;


    void Start()
    {
        // seed = seedStart;
        // for (int i = 0; i < 100; i++)
        // {
        //     string s = GeneratePrompt(prompt);
        //     seed += 1;
        //     print(s);
        // }
        seed = seedStart;
        RunUnityFunction("");

    }

    void Update()
    {

    }

    public override void RunUnityFunction(string image)
    {
        if (render)
        {
            // string thisPrompt = GeneratePrompt(prompt);
            // print(thisPrompt);

            print("Seed: " + seed);

            for (int i = 0; i < promptLinks.Length; i++)
            {
                promptLinks[i].prompt = ReplaceStrings(prompts[i], promptOptions);
            }
            if (extraValuesImg2Img != null)
            {
                foreach (ExtraValuesForImg2Img v in extraValuesImg2Img)
                    v.seed = seed;
            }
            if (extraValuesTxt2Img != null)
            {
                foreach (ExtraValuesForTxt2Image v in extraValuesTxt2Img)
                    v.seed = seed;
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

            for (int i = 0; i < saveLinks.Length; i++)
            {
                saveLinks[i].location = rootDirectory + "/" + familyName + "_" + ID.ToString("D4") + "_" + seed.ToString() + "_" + saveNames[i] + ".png";
            }
            // writeBG.location = rootDirectory + "/" + ID.ToString("D4") + "_" + familyName + "_BG_" + seed.ToString("D4") + ".png";
            // writeMG.location = rootDirectory + "/" + ID.ToString("D4") + "_" + familyName + "_MG_" + seed.ToString("D4") + ".png";
            // writeFG.location = rootDirectory + "/" + ID.ToString("D4") + "_" + familyName + "_FG_" + seed.ToString("D4") + ".png";
            startingLink.RunUnityFunction("");
            seed += seedIterate;
            ID++;
        }
    }

    public string ReplaceStrings(string s, string[] arr)
    {

        string r = s;
        print(arr.Length);
        for (int i = 0; i < arr.Length; i++)
        {
            // Random.InitState(seed);
            string[] options = arr[i].Split('|');
            string replacement = options[UnityEngine.Random.Range(0, options.Length)];
            print(replacement);
            r = r.Replace($"*{i + 1}", replacement);
        }
        print("String: " + r);
        return r;
    }

    string GeneratePrompt(string input)
    {
        int startIndex = input.IndexOf("{");
        while (startIndex >= 0)
        {
            int endIndex = input.IndexOf("}", startIndex);
            if (endIndex < 0) break;

            string block = input.Substring(startIndex + 1, endIndex - startIndex - 1);
            string[] options = block.Split('|');
            string replacement = options[UnityEngine.Random.Range(0, options.Length)];

            input = input.Substring(0, startIndex) + replacement + input.Substring(endIndex + 1);
            startIndex = input.IndexOf("{", startIndex + replacement.Length);
        }
        return input;
    }

    string[] GeneratePromptArray(string input)
    {
        string[] output = new string[CountBraces(input)];
        int startIndex = input.IndexOf("{");
        int i = 0;
        while (startIndex >= 0)
        {
            int endIndex = input.IndexOf("}", startIndex);
            if (endIndex < 0) break;

            string block = input.Substring(startIndex + 1, endIndex - startIndex - 1);
            string[] options = block.Split('|');
            string replacement = options[UnityEngine.Random.Range(0, options.Length)];

            input = input.Substring(0, startIndex) + replacement + input.Substring(endIndex + 1);
            output[i] = replacement;
            i++;
            startIndex = input.IndexOf("{", startIndex + replacement.Length);
        }
        return output;
    }

    int CountBraces(string input)
    {
        int count = 0;
        foreach (char c in input)
        {
            if (c == '{') count++;
        }
        return count;
    }
}
