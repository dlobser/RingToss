using UnityEngine;

public class FadeOutAndDestroy : MonoBehaviour
{
    private SpriteRenderer[] spriteRenderers;
    public float fadeDuration = 10f;
    private float timer;

    void Start()
    {
        // Get all SpriteRenderer components in this GameObject and its children
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (spriteRenderers.Length == 0) return;

        // Increment the timer
        timer += Time.deltaTime;

        // Calculate the new alpha value
        float alpha = Mathf.Clamp01(1 - (timer / fadeDuration));

        // Update the alpha of all SpriteRenderers
        foreach (var spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null)
            {
                Color newColor = spriteRenderer.color;
                newColor.a = alpha;
                spriteRenderer.color = newColor;
            }
        }

        // If the fade out is complete, destroy this GameObject
        if (alpha <= 0)
        {
            Destroy(gameObject);
        }
    }
}
