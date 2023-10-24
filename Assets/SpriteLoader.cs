using UnityEngine;

public class SpriteLoader : MonoBehaviour
{
    public string resourcePath = "MySprite"; // Path relative to Resources folder without extension.
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on this GameObject.");
            return;
        }

        LoadAndSetSprite(resourcePath);
    }

    private void LoadAndSetSprite(string path)
    {
        Texture2D texture = Resources.Load<Texture2D>(path);

        if (texture == null)
        {
            Debug.LogError("Failed to load texture from Resources at: " + path);
            return;
        }

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        if (sprite == null)
        {
            Debug.LogError("Failed to create sprite from texture.");
            return;
        }

        spriteRenderer.sprite = sprite;
    }
}
