using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDRenderChainLinkChatGPT : SDRenderChainLink
{
    public chatgptrequest chatGPTRequest;
    [TextArea(3, 50)]
    public string requestText = "Hello, world!"; // Initial example prompt
    public TextMesh text;
    public bool sendRequest = false;
    public ExtraValuesForImg2Img extraValuesImg2Img;
    public ExtraValuesForTxt2Image extraValuesTxt2Img;
    public string prePrompt;
    public string postPrompt;

    int seed;

    public bool startOnAwake = false;

    void Start()
    {
        if (startOnAwake)
        {
            RunUnityFunction("");
        }
    }

    public override void RunUnityFunction(string image)
    {
        seed++;
        string replacedText = requestText.Replace("{seed}", seed.ToString());
        if (chatGPTRequest != null)
        {
            chatGPTRequest.SendRequest(requestText, HandleResponse);
        }
    }

    void HandleResponse(string response)
    {
        if (string.IsNullOrEmpty(response))
        {
            Debug.Log("ChatGPT response was empty or there was an error.");
        }
        else
        {
            Debug.Log("Received response from ChatGPT: " + response);
            string outputString = prePrompt + " " + response + " " + postPrompt;

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
}
