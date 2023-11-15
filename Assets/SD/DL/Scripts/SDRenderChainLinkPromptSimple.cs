using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SDRenderChainLinkPromptSimple : SDRenderChainLink
{
    // First run functions in Unity, probably a screen render
    // Pass these renders along to stable diffusion with a link to arguments
    [TextAreaAttribute]
    public string prompt;
    public ExtraValuesForImg2Img extraValuesImg2Img;
    public ExtraValuesForTxt2Image extraValuesTxt2Img;

    void Start()
    {
        string outputString = ReplaceVariations(prompt);
        Debug.Log(outputString);
    }

    string ReplaceVariations(string input)
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

    public override void RunUnityFunction(string image)
    {
        string outputString = ReplaceVariations(prompt);

        if (extraValuesImg2Img != null)
        {
            extraValuesImg2Img.prompt = outputString;
        }
        if (extraValuesTxt2Img != null)
        {
            extraValuesTxt2Img.prompt = outputString;
        }
        foreach (SDRenderChainLink l in link)
        {
            l.RunUnityFunction("");
        }

    }


}
