using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDRenderChainLinkChatGPT : SDRenderChainLink
{
    public chatgptrequest chatGPTRequest;
    [TextArea(3, 50)]
    public string requestText = "Hello, world!"; // Initial example prompt
    [TextArea(3, 10)]
    public string followUpRequest = "Do it again, but make all the details very different, be specific and very descriptive";
    [TextArea(3, 10)]
    public string questions;
    string questionReplacementPrePrompt = "Do the same thing, but use this as the very special question: ";
    int questionReplacementIndex = 0;
    public int questionIterations = 5;
    int questionIndex = 0;
    public TextMesh text;
    public bool sendRequest = false;
    public ExtraValuesForImg2Img extraValuesImg2Img;
    public ExtraValuesForTxt2Image extraValuesTxt2Img;
    public string prePrompt;
    public string postPrompt;
    bool initialized = false;
    int seed;

    public SDRenderChainLinkSetSaveLocation saveLocation;
    public string imageName;

    public bool startOnAwake = false;

    void Start()
    {
        if (startOnAwake)
        {
            RunUnityFunction("");
        }
    }

    string ExtractQuestionByIndex(string questions, int index)
    {
        string[] individualQuestions = questions.Split('|');
        // Modulus operator to loop back if index goes out of range
        string specificQuestion = individualQuestions[index % individualQuestions.Length].Trim(); // Trim to remove any leading or trailing whitespace
        return specificQuestion;
    }


    public override void RunUnityFunction(string image)
    {
        seed++;
        string replacedText = requestText.Replace("{seed}", seed.ToString());
        questionReplacementIndex++;
        saveLocation.imageName = imageName + "Q-" + questionIndex + "_";
        if (chatGPTRequest != null)
        {
            if(questionReplacementIndex >= questionIterations)
            {
                questionReplacementIndex = 0;
                questionIndex++;
                if(questionIndex >= questions.Split('|').Length)
                {
                    questionIndex = 0;
                }
                string question = questionReplacementPrePrompt + " " + ExtractQuestionByIndex(questions, questionIndex);
                chatGPTRequest.SendRequest(question, HandleResponse);
            }   
            else{
                chatGPTRequest.SendRequest(!initialized?requestText:followUpRequest, HandleResponse);
            }
        }
        if(!initialized)
        {
            initialized = true;
            return;
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
