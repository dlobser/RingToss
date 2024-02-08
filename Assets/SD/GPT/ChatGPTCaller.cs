using UnityEngine;

public class ChatGPTCaller : MonoBehaviour
{
    public chatgptrequest chatGPTRequest;
    [TextArea(3, 50)]
    public string requestText = "Hello, world!"; // Initial example prompt
    public TextMesh text;
    public bool sendRequest = false;

    void Start()
    {
        // if (chatGPTRequest != null)
        // {
        //     chatGPTRequest.SendRequest("Hello, ChatGPT!", HandleResponse);
        // }
    }

    void Update()
    {
        if (sendRequest)
        {
            sendRequest = false;
            if (chatGPTRequest != null)
            {
                chatGPTRequest.SendRequest(requestText, HandleResponse);
            }
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
            text.text = response;
            Debug.Log("Received response from ChatGPT: " + response);
        }
    }
}
