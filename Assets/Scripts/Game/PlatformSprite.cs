using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlatformSprite : Platform
{
    public Transform itemParent;
    public Material material;
    public SpriteRenderer spriteRenderer;
    public Collider2D collider;
    public float bevel = 1;
    public float colliderSizeNudge = 0;

    void Start(){
        // SetMaterial(material);
        if(spriteRenderer==null && spriteRenderer.GetComponent<SpriteRenderer>()!=null)
            spriteRenderer.GetComponent<SpriteRenderer>();
    }

    void Update(){
        //execute if in edit mode
        if(!Application.isPlaying){
            spriteRenderer.transform.localScale = Vector2.one * bevel;
            spriteRenderer.size = platformScale/bevel;
            this.transform.localEulerAngles = new Vector3(platformRotation.x,platformRotation.x,platformRotation.z);
            SetColliderSize(platformScale);

        }
    }

    public override void SetColliderSize(Vector2 size)
    {
        Vector2 spriteSize = new Vector2(spriteRenderer.size.x, spriteRenderer.size.y);
        Vector2 scale = transform.localScale;
        Vector2 scaledSize = new Vector2(spriteSize.x / scale.x, spriteSize.y / scale.y) * new Vector2(this.transform.lossyScale.x, this.transform.lossyScale.y);
        if (collider is BoxCollider2D boxCollider)
        {
            boxCollider.size = new Vector2(scaledSize.x + colliderSizeNudge, scaledSize.y + colliderSizeNudge);
        }
        else if (collider is CapsuleCollider2D capsuleCollider)
        {
            capsuleCollider.size = new Vector2(scaledSize.x + colliderSizeNudge, scaledSize.y + colliderSizeNudge);
        }
        else if (collider is CircleCollider2D circleCollider)
        {
            circleCollider.radius = scaledSize.x / 2 + colliderSizeNudge; // Assuming circular symmetry for resizing
        }
    }

    public override void SetMaterial(Material mat)
    {
        base.SetMaterial(mat);
    }

    public override void SetSize(Vector2 scale)
    {
        base.SetSize(scale);
        spriteRenderer.transform.localScale = Vector2.one * bevel;
        spriteRenderer.size = scale;
        SetColliderSize(scale);
    }

    public override void SetPosition(Vector3 position)
    {
        base.SetPosition(position);
        this.transform.localPosition = position;
    }

    public override void SetRotation(float z, float x = 0, float y = 0){
        base.SetRotation(z,x,y);
        this.transform.localEulerAngles = new Vector3(x,y,z);
    }

    public override void SetMainTexture(Sprite tex)
    {
        spriteRenderer.material.SetTexture("_TextureTex", tex.texture);
    }

    public override void SetPhysicsBounciness(float bounceValue)
    {
        PhysicsMaterial2D physMat = Instantiate(collider.sharedMaterial);
        collider.sharedMaterial = physMat;
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
            float spacing = platformScale.x / (itemArguments.amount + 1);

            for (int j = 1; j <= itemArguments.amount; j++)
            {
                Vector3 itemPosition = itemArguments.offset + new Vector3(spacing * j - platformScale.x*.5f, 0, 0);
                GameObject item = Instantiate(itemArguments.item, itemPosition, Quaternion.identity, items.transform);
                item.transform.localPosition = itemPosition;
                item.transform.localScale = Vector3.one*.05f;
                item.transform.localEulerAngles = Vector3.zero;
                SetSprite(item.transform.GetChild(0).GetComponent<SpriteRenderer>());
            }
        }

    }

    public void SetSprite(SpriteRenderer spriteRenderer){
        Texture2D chosenTexture = ImageLoader.Instance.GetImageWithIndex("Item",-1);
        spriteRenderer.sprite = Sprite.Create(chosenTexture, new Rect(0, 0, chosenTexture.width, chosenTexture.height), new Vector2(0.5f, 0.5f));
        spriteRenderer.material.SetTexture("_MainTex",chosenTexture);
    }

}
