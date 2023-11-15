using UnityEngine;
using TMPro;

[ExecuteAlways]
[RequireComponent(typeof(TMP_Text))]
public class SetTextWidth : MonoBehaviour
{
    public float desiredWidth = 5f;       // Width you want to maintain
    public int maxCharsBeforeLineBreak = 20;  // Max characters before inserting a line break

    private TMP_Text tmpText;
    public string text;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    // void Update(){
    //     tmpText.text = InsertLineBreaks(text);
    //     FitTextToWidth();
    // }

    public void SetText(string text)
    {
        tmpText.text = InsertLineBreaks(text);
        FitTextToWidth();
    }

    private void FitTextToWidth()
    {
        tmpText.ForceMeshUpdate();  // Ensure TMP updates its internal state

        // Define a max font size so the text doesn't become excessively large
        float maxFontSize = 100f;  // Adjust this value based on your needs

        // While text width is greater than desired width, reduce font size
        while (tmpText.textBounds.size.x > desiredWidth && tmpText.fontSize > 0.1f)
        {
            tmpText.fontSize -= 0.1f;
            tmpText.ForceMeshUpdate();
        }

        // While text width is smaller than desired width, increase font size
        while (tmpText.textBounds.size.x < desiredWidth && tmpText.fontSize < maxFontSize)
        {
            tmpText.fontSize += 0.1f;
            tmpText.ForceMeshUpdate();
        }
    }

    private string InsertLineBreaks(string text)
    {
        string[] words = text.Split(' ');
        string result = "";
        string line = "";

        foreach (string word in words)
        {
            if ((line + word).Length > maxCharsBeforeLineBreak)
            {
                result += line.TrimEnd() + '\n'; // Add the current line to the result and start a new line
                line = "";
            }
            line += word + " ";
        }

        // Add any remaining words
        result += line.TrimEnd();

        return result;
    }

}
