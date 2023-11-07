using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSprite : Platform
{
    public Material material;
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider;
    public Transform itemParent;

    void Start(){
        // SetMaterial(material);
        if(spriteRenderer==null && spriteRenderer.GetComponent<SpriteRenderer>()!=null)
            spriteRenderer.GetComponent<SpriteRenderer>();
    }

    public override void SetMaterial(Material mat)
    {
        base.SetMaterial(mat);
    }

    public override void SetSize(Vector2 scale)
    {
        spriteRenderer.size = scale;
        size = new Vector3(scale.x,scale.y,1);
    }

    public override void SetMainTexture(Sprite tex)
    {
        spriteRenderer.material.SetTexture("_TextureTex", tex.texture);
    }

    public override void SetPhysicsBounciness(float bounceValue)
    {
        PhysicsMaterial2D physMat = Instantiate(boxCollider.sharedMaterial);
        boxCollider.sharedMaterial = physMat;
        physMat.bounciness = bounceValue;
    }

    public override void SetAlpha(float a){
        Color c = spriteRenderer.color;
        c.a = a;
        spriteRenderer.color = c;
    }

    public override void PopulatePlatformWithItems(PlatformItemArguments itemArguments){
        if(itemArguments.amount>0){
            GameObject items = new GameObject("Items");
            items.transform.SetParent(itemParent);
            items.transform.localPosition = Vector3.zero;
            items.transform.localScale = Vector3.one;
            items.transform.localEulerAngles = Vector3.zero;
            float spacing = size.x / (itemArguments.amount + 1);

            for (int j = 1; j <= itemArguments.amount; j++)
            {
                Vector3 itemPosition = itemArguments.offset + new Vector3(spacing * j - size.x*.5f, 0, 0);
                GameObject item = Instantiate(itemArguments.item, itemPosition, Quaternion.identity, items.transform);
                item.transform.localPosition = itemPosition;
                item.transform.localScale = Vector3.one*.05f;
                item.transform.localEulerAngles = Vector3.zero;
                SetSprite(item.transform.GetChild(0).GetComponent<SpriteRenderer>());
            }
        }
    }

    public void SetSprite(SpriteRenderer spriteRenderer){
        Texture2D chosenTexture = ImageLoader.Instance.GetImageWithIndex("Item");
        spriteRenderer.sprite = Sprite.Create(chosenTexture, new Rect(0, 0, chosenTexture.width, chosenTexture.height), new Vector2(0.5f, 0.5f));
        spriteRenderer.material.SetTexture("_MainTex",chosenTexture);
    }

}
