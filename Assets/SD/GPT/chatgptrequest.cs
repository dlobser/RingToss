using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class chatgptrequest : MonoBehaviour
{
    public string YOUR_API_KEY = "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";
    private string requestText = "Hello, world!"; // Initial example prompt
    private readonly string url = "https://api.openai.com/v1/chat/completions"; // API URL
    private string apiKey;
    private List<string> conversationHistory = new List<string>();


    void Start()
    {
        apiKey = YOUR_API_KEY; // Initialize the API key
    }

    public void SendRequest(string prompt, Action<string> callback)
    {
        StartCoroutine(SendRequestToChatGPT(prompt, callback));
    }

    IEnumerator SendRequestToChatGPT(string prompt, Action<string> callback)
    {
        // Replace newline characters with a space
        string sanitizedPrompt = prompt.Replace("\n", " ").Replace("\r", " ");

        conversationHistory.Add("{\"role\": \"user\", \"content\": \"" + sanitizedPrompt + "\"}");

        // Constructing the JSON payload with the sanitized conversation history
        var json = "{\"model\": \"gpt-4\", \"messages\": [{\"role\": \"user\", \"content\": \"" + string.Join(",", conversationHistory) + "\"}]}";
        var postData = System.Text.Encoding.UTF8.GetBytes(json);

        Debug.Log(json);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                callback?.Invoke("");
            }
            else
            {
                string content = ExtractContentFromResponse(request.downloadHandler.text);
                Debug.Log("Extracted Content: " + content);
                callback?.Invoke(content);
            }
        }
    }


    string ExtractContentFromResponse(string jsonResponse)
    {
        string searchString = "\"content\": \"";
        int startIndex = jsonResponse.IndexOf(searchString) + searchString.Length;
        if (startIndex != -1 + searchString.Length)
        {
            int endIndex = jsonResponse.IndexOf("\"", startIndex);
            if (endIndex > startIndex)
            {
                return jsonResponse.Substring(startIndex, endIndex - startIndex);
            }
        }
        return "Content not found";
    }
}

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Networking;

// public class chatgptrequest : MonoBehaviour
// {
//     public string YOUR_API_KEY = "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";
//     [TextArea]
//     public string request = "Hello, world!"; // Initial example prompt
//     private readonly string url = "https://api.openai.com/v1/chat/completions"; // API URL
//     private string apiKey;

//     public bool sendRequest = false;
//     private List<string> conversationHistory = new List<string>(); // To store conversation history

//     void Start()
//     {
//         apiKey = YOUR_API_KEY; // Initialize the API key
//         // Optionally start an initial conversation
//         // StartCoroutine(SendRequestToChatGPT(request)); 
//     }

//     void Update()
//     {
//         if (sendRequest)
//         {
//             Debug.Log("Sending request to ChatGPT");
//             sendRequest = false;
//             StartCoroutine(SendRequestToChatGPT(request));
//         }
//     }

//     public string GetResponse(string prompt)
//     {
//         request = prompt;
//         sendRequest = true;
//         return "Request sent";
//     }

//     IEnumerator SendRequestToChatGPT(string prompt)
//     {
//         // Adding the user's prompt to the conversation history
//         conversationHistory.Add("{\"role\": \"user\", \"content\": \"" + prompt + "\"}");

//         // Constructing the JSON payload with the conversation history
//         var json = "{\"model\": \"gpt-3.5-turbo\", \"messages\": [" + string.Join(",", conversationHistory) + "]}";
//         var postData = System.Text.Encoding.UTF8.GetBytes(json);

//         using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
//         {
//             request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
//             request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
//             request.SetRequestHeader("Content-Type", "application/json");
//             request.SetRequestHeader("Authorization", "Bearer " + apiKey);

//             yield return request.SendWebRequest();

//             if (request.result != UnityWebRequest.Result.Success)
//             {
//                 Debug.LogError(request.error);
//             }
//             else
//             {
//                 Debug.Log("Raw Response: " + request.downloadHandler.text);
//                 string content = ExtractContentFromResponse(request.downloadHandler.text);
//                 Debug.Log("Extracted Content: " + content);

//                 // Optionally, add the AI response to the conversation history
//                 // conversationHistory.Add("{\"role\": \"assistant\", \"content\": \"" + content + "\"}");
//             }
//         }
//     }

//     string ExtractContentFromResponse(string jsonResponse)
//     {
//         string searchString = "\"content\": \"";
//         int startIndex = jsonResponse.IndexOf(searchString) + searchString.Length;
//         if (startIndex != -1 + searchString.Length)
//         {
//             int endIndex = jsonResponse.IndexOf("\"", startIndex);
//             if (endIndex > startIndex)
//             {
//                 return jsonResponse.Substring(startIndex, endIndex - startIndex);
//             }
//         }
//         return "Content not found";
//     }
// }


// // using System.Collections;
// // using UnityEngine;
// // using UnityEngine.Networking;

// // public class chatgptrequest : MonoBehaviour
// // {
// //     public string YOUR_API_KEY = "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";
// //     public string request = "Hello, world!"; // Example prompt
// //     private readonly string url = "https://api.openai.com/v1/chat/completions"; // Replace with the actual API URL
// //     private string apiKey;// = YOUR_API_KEY; // Replace with your actual API key

// //     public bool sendRequest = false;

// //     // Start is called before the first frame update
// //     void Start()
// //     {
// //         apiKey = YOUR_API_KEY;
// //         StartCoroutine(SendRequestToChatGPT(request)); // Example prompt
// //     }

// //     void Update(){
// //         if(sendRequest){
// //             sendRequest = false;
// //             StartCoroutine(SendRequestToChatGPT(request));
// //         }
// //     }

// //     IEnumerator SendRequestToChatGPT(string prompt)
// //     {
// //         var json = "{\"model\": \"gpt-3.5-turbo\", \"messages\": [{\"role\": \"user\", \"content\": \"" + prompt + "\"}]}";
// //         var postData = System.Text.Encoding.UTF8.GetBytes(json);

// //         using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
// //         {
// //             request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
// //             request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
// //             request.SetRequestHeader("Content-Type", "application/json");
// //             request.SetRequestHeader("Authorization", "Bearer " + apiKey);

// //             yield return request.SendWebRequest();

// //             if (request.result != UnityWebRequest.Result.Success)
// //             {
// //                 Debug.LogError(request.error);
// //             }
// //             else
// //             {
// //                 Debug.Log("Raw Response: " + request.downloadHandler.text);
// //                 // Extracting the content
// //                 string content = ExtractContentFromResponse(request.downloadHandler.text);
// //                 Debug.Log("Extracted Content: " + content);
// //             }
// //         }
// //     }

// //     string ExtractContentFromResponse(string jsonResponse)
// //     {
// //         // Simple JSON parsing to find the content field
// //         string searchString = "\"content\": \"";
// //         int startIndex = jsonResponse.IndexOf(searchString) + searchString.Length;
// //         if (startIndex != -1 + searchString.Length) // Means the searchString was found
// //         {
// //             int endIndex = jsonResponse.IndexOf("\"", startIndex);
// //             if (endIndex > startIndex)
// //             {
// //                 return jsonResponse.Substring(startIndex, endIndex - startIndex);
// //             }
// //         }
// //         return "Content not found";
// //     }
// // }
