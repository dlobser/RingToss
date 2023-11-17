using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class UpdateSpriteSize : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider;
    public float nudge;

    private void Awake()
    {
        if(spriteRenderer==null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if(boxCollider==null)
            boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (spriteRenderer.sprite != null)
        {
            // spriteRenderer.size.width = 1;
            // show documentation for spriterenderer.size 

            Vector2 spriteSize = new Vector2(spriteRenderer.size.x, spriteRenderer.size.y);
            Vector2 scale = transform.localScale;
            Vector2 scaledSize = new Vector2(spriteSize.x / scale.x, spriteSize.y / scale.y) * new Vector2(this.transform.lossyScale.x, this.transform.lossyScale.y);
            boxCollider.size = new Vector2(scaledSize.x + nudge, scaledSize.y + nudge);
        }
    }
}
