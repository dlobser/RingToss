using UnityEngine;
using UnityEngine.UI;

public class PaintingTool : MonoBehaviour
{
    public RawImage drawingSurface;
    private Texture2D drawingTexture;
    private Color brushColor = Color.black; // Default brush color
    private int brushSize = 5; // Default brush size
    public GameObject quad;

    void Start()
    {
        InitializeTexture();
    }

    void InitializeTexture()
    {
        drawingTexture = new Texture2D((int)drawingSurface.rectTransform.rect.width, (int)drawingSurface.rectTransform.rect.height);
        drawingTexture.filterMode = FilterMode.Point;
        ClearTexture();
        drawingSurface.texture = drawingTexture;
        quad.GetComponent<MeshRenderer>().material.SetTexture("_MainTex",drawingTexture);
    }

    void ClearTexture()
    {
        Color[] clearColors = new Color[drawingTexture.width * drawingTexture.height];
        for (int i = 0; i < clearColors.Length; i++)
        {
            clearColors[i] = Color.white; // or any background color
        }
        drawingTexture.SetPixels(clearColors);
        drawingTexture.Apply();
    }

    void Update()
    {
        Vector2 paintPosition = Vector2.zero;
        bool shouldPaint = false;

        // Handle touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            paintPosition = touch.position;
            shouldPaint = touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Began;
        }
        // Handle mouse input
        else if (Input.GetMouseButton(0)) // 0 is the left mouse button
        {
            paintPosition = Input.mousePosition;
            shouldPaint = true;
        }

        if (shouldPaint)
        {
            // Convert screen position to texture coordinates
            Vector2 textureCoord = ConvertScreenCoordsToTextureCoords(paintPosition);
            Paint(textureCoord);
        }
    }


    Vector2 ConvertScreenCoordsToTextureCoords(Vector2 screenCoords)
    {
        Vector2 textureCoords = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingSurface.rectTransform, screenCoords, null, out Vector2 localPoint);
        textureCoords.x = Mathf.Clamp((localPoint.x + drawingSurface.rectTransform.rect.width * 0.5f) / drawingSurface.rectTransform.rect.width, 0, 1) * drawingTexture.width;
        textureCoords.y = Mathf.Clamp((localPoint.y + drawingSurface.rectTransform.rect.height * 0.5f) / drawingSurface.rectTransform.rect.height, 0, 1) * drawingTexture.height;
        return textureCoords;
    }

    void Paint(Vector2 position)
    {
        for (int x = (int)position.x - brushSize; x < (int)position.x + brushSize; x++)
        {
            for (int y = (int)position.y - brushSize; y < (int)position.y + brushSize; y++)
            {
                drawingTexture.SetPixel(x, y, brushColor);
            }
        }
        drawingTexture.Apply();
    }

    // Add method to change brush size and color based on UI interaction
    public void ChangeBrushSettings(Color newColor, int newSize)
    {
        brushColor = newColor;
        brushSize = newSize;
    }

    // Add a method here to save the texture to a file if needed
}
