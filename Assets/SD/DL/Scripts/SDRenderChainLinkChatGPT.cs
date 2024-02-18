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
        string[] responses;
        int responsesIndex = 0;
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
            string firstQuestion = ExtractQuestionByIndex(questions,0);
            requestText = firstQuestion + " " + requestText;
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
                    if(responses==null){
                        chatGPTRequest.SendRequest(!initialized?requestText:followUpRequest, HandleResponse);
                    }
                    else if(responsesIndex<responses.Length){
                        string outputString = prePrompt + " " + responses[responsesIndex] + " " + postPrompt;
                        responsesIndex++;
                        SetPrompts(outputString);
                    }
                    else{
                        responsesIndex = 0;
                        questionReplacementIndex++;
                        questionIndex++;
                        if(questionIndex >= questions.Split('|').Length)
                        {
                            questionIndex = 0;
                        }
                        string question = questionReplacementPrePrompt + " " + ExtractQuestionByIndex(questions, questionIndex);
                        chatGPTRequest.SendRequest(question, HandleResponse);
                        // chatGPTRequest.SendRequest(!initialized?requestText:followUpRequest, HandleResponse);
                    }
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
                responses = response.Split('|');
                string outputString = prePrompt + " " + responses[responsesIndex] + " " + postPrompt;
                responsesIndex++;
                SetPrompts(outputString);
            }
        }

        void SetPrompts(string outputString){
            saveLocation.imageName = imageName + "Q-" + questionIndex.ToString("0000") + "_V-" + responsesIndex.ToString("0000")+"_F-";
            Debug.Log("Setting Prompt: " + outputString);
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
