using UnityEngine;

public static class ColorPaletteGenerator 
{
    // public Texture2D texture;
    // public Color averageColor;
    // public Color[] splitComplementaryColors;

    // void Start()
    // {
    //     averageColor = CalculateAverageColor(texture);
    //     splitComplementaryColors = GenerateSplitComplementaryPalette(averageColor);
    // }

    public static Color CalculateAverageColor(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();
        float r = 0, g = 0, b = 0;

        foreach (Color pixel in pixels)
        {
            r += pixel.r;
            g += pixel.g;
            b += pixel.b;
        }

        r /= pixels.Length;
        g /= pixels.Length;
        b /= pixels.Length;

        return new Color(r, g, b);
    }

    // public static Color[] GenerateSplitComplementaryPalette(Color color)
    // {
    //     Color.RGBToHSV(color, out float h, out float s, out float v);

    //     // Adjusting saturation and value
    //     s = (s + 0.33f) % 1;
    //     v = Mathf.Min(1,(v + 0.2f));

    //     Color[] palette = new Color[3];

    //     // Calculate split complementary colors
    //     float h1 = (h + 0.33f) % 1; // Adjust these values for different angles in the color wheel
    //     float h2 = (h - 0.33f + 1) % 1; // Adding 1 before modulo operation to ensure positive value
    //     float h3 = (h - 0.5f + 1) % 1;

    //     palette[0] = Color.HSVToRGB(h3, s, v);
    //     palette[1] = Color.HSVToRGB(h1, s, v);
    //     palette[2] = Color.HSVToRGB(h2, s, v);

    //     return palette;
    // }

    public static Color[] GenerateSplitComplementaryPalette(Color color)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);

        // Find the complementary hue
        float complementaryHue = (h + 0.5f) % 1;
        v = (v + 0.2f) % 1; // Adjusting value (brightness
        s = (s + 0.3f) % 1; // Adjusting saturation

        // Choose smaller offsets for adjacent hues
        float offset = 0.1f;  // Adjust as needed
        float h1 = (complementaryHue + offset) % 1;
        float h2 = (complementaryHue - offset + 1) % 1;

        // Create a palette of three colors: the base color and two split complements
        Color[] palette = new Color[3];
        palette[0] = Color.HSVToRGB(complementaryHue, s, v);;  // Original color
        palette[1] = Color.HSVToRGB(h1, s, v);  // First split complement
        palette[2] = Color.HSVToRGB(h2, s, v);  // Second split complement

        return palette;
    }


    public static Texture2D MakeTextureReadable(Texture2D original)
    {
        // Create a temporary RenderTexture of the same size as the original Texture
        RenderTexture tmp = RenderTexture.GetTemporary(
                            original.width,
                            original.height,
                            0,
                            RenderTextureFormat.Default,
                            RenderTextureReadWrite.Linear);

        // Blit the pixels on texture to the RenderTexture
        Graphics.Blit(original, tmp);

        // Backup the currently active RenderTexture
        RenderTexture previous = RenderTexture.active;

        // Set the current RenderTexture to the temporary one we created
        RenderTexture.active = tmp;

        // Create a new readable Texture2D to copy the pixels to it
        Texture2D readableTexture = new Texture2D(original.width, original.height);
        readableTexture.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        readableTexture.Apply();

        // Reset the active RenderTexture
        RenderTexture.active = previous;

        // Release the temporary RenderTexture
        RenderTexture.ReleaseTemporary(tmp);

        return readableTexture;
    }

}
