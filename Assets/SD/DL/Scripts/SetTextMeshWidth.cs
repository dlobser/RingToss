using UnityEngine;
using TMPro;

[ExecuteAlways]
public class SetTextMeshWidth : MonoBehaviour
{
    public float desiredWidth = 5f;
    public int maxCharsBeforeLineBreak = 20;

    private TextMesh textMesh;
    private TextMeshProUGUI textMeshPro;
    private MeshRenderer meshRenderer;
    public string text;

    private void Awake()
    {
        textMesh = GetComponent<TextMesh>(); 
        textMeshPro = GetComponent<TextMeshProUGUI>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        
        // Check if any component is null and search for it
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMesh>();
        }

        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }

        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        SetText();
    }

    public void SetText()
    {
        string processedText = InsertLineBreaks(text);
        // print(textMeshPro);
        if (textMesh != null)
        {
            textMesh.text = processedText;
            FitTextToWidthTextMesh();
        }
        if (textMeshPro != null)
        {
            textMeshPro.text = processedText;
            FitTextToWidthTMP();
        }
    }

    public void SetText(string thisText)
    {
        text = thisText;
        FitTextToWidth();
    }

    private void FitTextToWidthTextMesh()
    {
        float maxFontSize = 100f;
        while (GetTextWidthTextMesh() > desiredWidth && textMesh.characterSize > 0.1f)
        {
            textMesh.characterSize -= 0.01f;
        }
        while (GetTextWidthTextMesh() < desiredWidth && textMesh.characterSize < maxFontSize)
        {
            textMesh.characterSize += 0.01f;
        }
    }

    private void FitTextToWidthTMP()
    {
        float maxFontSize = 100f;
        while (GetTextWidthTMP() > desiredWidth && textMeshPro.fontSize > 0.1f)
        {
            textMeshPro.fontSize -= 0.1f;
        }
        while (GetTextWidthTMP() < desiredWidth && textMeshPro.fontSize < maxFontSize)
        {
            textMeshPro.fontSize += 0.1f;
        }
        textMeshPro.gameObject.SetActive(false);
        textMeshPro.gameObject.SetActive(true);
    }

    public void FitTextToWidth(){
        if (textMesh != null)
        {
            FitTextToWidthTextMesh();
        }
        else if (textMeshPro != null)
        {
            FitTextToWidthTMP();
        }
    }

    private float GetTextWidthTextMesh()
    {
        meshRenderer.transform.localScale = new Vector3(textMesh.characterSize, textMesh.characterSize, 1);
        meshRenderer.transform.localPosition = Vector3.zero;
        return meshRenderer.bounds.size.x;
    }

    private float GetTextWidthTMP()
    {
        // This is a simplified estimation. For accurate results, consider text layout complexities.
        return textMeshPro.preferredWidth;
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
                result += line.TrimEnd() + '\n';
                line = "";
            }
            line += word + " ";
        }

        result += line.TrimEnd();
        return result;
    }
}

// using UnityEngine;

// [ExecuteAlways]
// [RequireComponent(typeof(TextMesh))]
// public class SetTextMeshWidth : MonoBehaviour
// {
//     public float desiredWidth = 5f;       // Width you want to maintain
//     public int maxCharsBeforeLineBreak = 20;  // Max characters before inserting a line break

//     private TextMesh textMesh;
//     private MeshRenderer meshRenderer;
//     public string text;

//     private void Awake()
//     {
//         textMesh = GetComponent<TextMesh>();
//         meshRenderer = GetComponent<MeshRenderer>();
//     }
    
//     void Update(){
//         SetText();
//         // print("THE MATERIAL: " + this.GetComponent<MeshRenderer>().sharedMaterial);
//     }

//     public void SetText()
//     {
//         textMesh.text = InsertLineBreaks(text);
//         FitTextToWidth();
//     }

//     public void SetText(string thisText)
//     {
//         text = thisText;
//         SetText();
//     }

//     public void FitTextToWidth()
//     {
//         // Define a max font size so the text doesn't become excessively large
//         float maxFontSize = 100f;  // Adjust this value based on your needs

//         // While text width is greater than desired width, reduce font size
//         while (GetTextWidth() > desiredWidth && textMesh.characterSize > 0.1f)
//         {
//             textMesh.characterSize -= 0.01f;
//         }

//         // While text width is smaller than desired width, increase font size
//         while (GetTextWidth() < desiredWidth && textMesh.characterSize < maxFontSize)
//         {
//             textMesh.characterSize += 0.01f;
//         }
//     }

//     private float GetTextWidth()
//     {
//         // Update mesh and get the bounds
//         // meshRenderer.sharedMaterial = textMesh.font.material; // Ensure the material is updated
//         meshRenderer.transform.localScale = new Vector3(textMesh.characterSize, textMesh.characterSize, 1);
//         meshRenderer.transform.localPosition = new Vector3(0, 0, 0);

//         return meshRenderer.bounds.size.x;
//     }

//     private string InsertLineBreaks(string text)
//     {
//         string[] words = text.Split(' ');
//         string result = "";
//         string line = "";

//         foreach (string word in words)
//         {
//             if ((line + word).Length > maxCharsBeforeLineBreak)
//             {
//                 result += line.TrimEnd() + '\n'; // Add the current line to the result and start a new line
//                 line = "";
//             }
//             line += word + " ";
//         }

//         // Add any remaining words
//         result += line.TrimEnd();

//         return result;
//     }
// }
