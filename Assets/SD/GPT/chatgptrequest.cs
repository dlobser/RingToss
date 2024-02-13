using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json; // If using Newtonsoft.Json for serialization

[System.Serializable]
public class Message
{
    public string role;
    public string content;
}

public class chatgptrequest : MonoBehaviour
{
    public string YOUR_API_KEY = "don't push your api key to github";
    private readonly string url = "https://api.openai.com/v1/chat/completions"; // API URL
    private string apiKey;
    private List<Message> conversationHistory = new List<Message>();

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
        // Add user's prompt to conversation history
        conversationHistory.Add(new Message { role = "user", content = prompt });

        // Serialize the conversation history to JSON
        string jsonPayload = JsonConvert.SerializeObject(new { model = "gpt-3.5-turbo", messages = conversationHistory });

        Debug.Log(jsonPayload);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
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
                // Add ChatGPT's response to conversation history
                conversationHistory.Add(new Message { role = "assistant", content = content });
                callback?.Invoke(content);
            }
        }
    }

    string ExtractContentFromResponse(string jsonResponse)
    {
        // Assuming jsonResponse is correctly formatted and contains the desired data
        // Extract the content from the ChatGPT response
        try
        {
            var responseObj = JsonUtility.FromJson<Response>(jsonResponse);
            if (responseObj != null && responseObj.choices != null && responseObj.choices.Length > 0)
            {
                return responseObj.choices[0].message.content;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing response: " + ex.Message);
        }
        return "Content not found";
    }

    [Serializable]
    private class Response
    {
        public Choice[] choices;
    }

    [Serializable]
    private class Choice
    {
        public Message message;
    }
}

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Networking;

// public class chatgptrequest : MonoBehaviour
// {
//     public string YOUR_API_KEY = "don't push your api key to github";
//     private string requestText = "Hello, world!"; // Initial example prompt
//     private readonly string url = "https://api.openai.com/v1/chat/completions"; // API URL
//     private string apiKey;
//     private List<string> conversationHistory = new List<string>();


//     void Start()
//     {
//         apiKey = YOUR_API_KEY; // Initialize the API key
//     }

//     public void SendRequest(string prompt, Action<string> callback)
//     {
//         StartCoroutine(SendRequestToChatGPT(prompt, callback));
//     }

//     IEnumerator SendRequestToChatGPT(string prompt, Action<string> callback)
//     {
//         // Replace newline characters with a space
//         string sanitizedPrompt = prompt.Replace("\n", " ").Replace("\r", " ");

//         conversationHistory.Add(sanitizedPrompt);//"{\"role\": \"user\", \"content\": \"" + sanitizedPrompt + "\"}");

//         string conversation = string.Join(",", conversationHistory);
//         Debug.Log("Conversation: " + conversation);
//         // Constructing the JSON payload with the sanitized conversation history
//         var json = "{\"model\": \"gpt-4\", \"messages\": [{\"role\": \"user\", \"content\": \"" + conversation + "\"}]}";
//         var postData = System.Text.Encoding.UTF8.GetBytes(json);

//         Debug.Log(json);

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
//                 callback?.Invoke("");
//             }
//             else
//             {
//                 string content = ExtractContentFromResponse(request.downloadHandler.text);
//                 Debug.Log("Extracted Content: " + content);
//                 callback?.Invoke(content);
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
