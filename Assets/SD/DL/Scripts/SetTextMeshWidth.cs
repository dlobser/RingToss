using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(TextMesh))]
public class SetTextMeshWidth : MonoBehaviour
{
    public float desiredWidth = 5f;       // Width you want to maintain
    public int maxCharsBeforeLineBreak = 20;  // Max characters before inserting a line break

    private TextMesh textMesh;
    private MeshRenderer meshRenderer;
    public string text;

    private void Awake()
    {
        textMesh = GetComponent<TextMesh>();
        meshRenderer = GetComponent<MeshRenderer>();
    }
    
    void Update(){
        SetText();
        // print("THE MATERIAL: " + this.GetComponent<MeshRenderer>().sharedMaterial);
    }

    public void SetText()
    {
        textMesh.text = InsertLineBreaks(text);
        FitTextToWidth();
    }

    public void SetText(string thisText)
    {
        text = thisText;
        SetText();
    }

    public void FitTextToWidth()
    {
        // Define a max font size so the text doesn't become excessively large
        float maxFontSize = 100f;  // Adjust this value based on your needs

        // While text width is greater than desired width, reduce font size
        while (GetTextWidth() > desiredWidth && textMesh.characterSize > 0.1f)
        {
            textMesh.characterSize -= 0.01f;
        }

        // While text width is smaller than desired width, increase font size
        while (GetTextWidth() < desiredWidth && textMesh.characterSize < maxFontSize)
        {
            textMesh.characterSize += 0.01f;
        }
    }

    private float GetTextWidth()
    {
        // Update mesh and get the bounds
        // meshRenderer.sharedMaterial = textMesh.font.material; // Ensure the material is updated
        meshRenderer.transform.localScale = new Vector3(textMesh.characterSize, textMesh.characterSize, 1);
        meshRenderer.transform.localPosition = new Vector3(0, 0, 0);

        return meshRenderer.bounds.size.x;
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
