using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlatformSprite_Round : Platform
{
    public Transform itemParent;
    public GameObject BGCirclePrefab;
    public GameObject BGCircle;
    public Material material;
    public SpriteRenderer spriteRenderer;
    public Collider2D collider;
    public GameObject bonusItem;
    public float bevel = 1;
    public float colliderSizeNudge = 0;
    public bool hideColliderOnItemEmpty = true;

    void Start()
    {
        // SetMaterial(material);
        if (spriteRenderer == null && spriteRenderer.GetComponent<SpriteRenderer>() != null)
            spriteRenderer.GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        //execute if in edit mode
        if (!Application.isPlaying)
        {
            // if (spriteRenderer != null)
            // {
            //     spriteRenderer.transform.localScale = Vector2.one * bevel;
            //     spriteRenderer.size = (platformScale / bevel) * new Vector2(.75f, 1);
            //     spriteRenderer.transform.localPosition = new Vector3(0, -platformScale.x * .25f, 0);
            // }
            // this.transform.localEulerAngles = new Vector3(platformRotation.x, platformRotation.x, platformRotation.z);
            // SetColliderSize(platformScale);

            collider.transform.localScale = Vector2.one * platformScale.x;
            Debug.Log("Update" + collider.transform.localScale);

        }
        // if (itemParent.GetComponentsInChildren<Item>().Length == 0 && hideColliderOnItemEmpty)
        // collider.gameObject.SetActive(false);
        // SetColliderSize(platformScale);
    }

    public override void OnItemHit()
    {
        Debug.Log("OnItemHit");
        Invoke("MakeBonusItem", .5f);
        collider.gameObject.SetActive(false);
    }

    void MakeBonusItem(){
        GameObject b = Instantiate(bonusItem, this.transform.position, Quaternion.identity, itemParent);
        b.transform.localScale = Vector3.one*.1f;
    }

    public override void SetColliderSize(Vector2 size)
    {
        if (BGCircle == null)
        {
            BGCircle = Instantiate(BGCirclePrefab, this.transform);
            BGCircle.transform.localPosition = Vector3.zero;
            BGCircle.transform.localScale = Vector3.one;
            BGCircle.transform.localEulerAngles = Vector3.zero;
        }

        Vector2 spriteSize = new Vector2(platformScale.x, platformScale.y);
        BGCircle.transform.localScale = new Vector3(platformScale.x * 2, platformScale.x * 2, 1);
        Vector2 scale = transform.localScale;
        Vector2 scaledSize = new Vector2((spriteSize.x / scale.x), spriteSize.y / scale.y) * new Vector2(this.transform.lossyScale.x, this.transform.lossyScale.y);

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
        collider.transform.localScale = Vector2.one * platformScale.x * .45f;
        // Debug.Log("Update" + collider.transform.localScale);

        // spriteRenderer.size = scale;
        // SetColliderSize(scale);
    }

    public override void SetPosition(Vector3 position)
    {
        base.SetPosition(position);
        this.transform.localPosition = position;
        spriteRenderer.transform.localPosition = new Vector3(0, 0, 0);
    }

    public override void SetRotation(float z, float x = 0, float y = 0)
    {
        base.SetRotation(z, x, y);
        this.transform.localEulerAngles = new Vector3(x, y, z);
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

    public override void SetAlpha(float a)
    {
        Color c = spriteRenderer.color;
        c.a = a;
        spriteRenderer.color = c;
    }

    public override void PopulatePlatformWithItems(PlatformItemArguments itemArguments)
    {

        if (itemArguments.amount > 0)
        {

            GameObject items = new GameObject("Items");
            items.transform.SetParent(itemParent);
            items.transform.localPosition = Vector3.zero;
            items.transform.localScale = Vector3.one;
            items.transform.localEulerAngles = Vector3.zero;
            float spacing = platformScale.x / (itemArguments.amount + 1);

            for (int j = 1; j <= itemArguments.amount; j++)
            {
                Vector3 itemPosition = itemArguments.offset + new Vector3(spacing * j - platformScale.x * .5f, 0, 0);
                GameObject item = Instantiate(itemArguments.item, itemPosition, Quaternion.identity, items.transform);
                if (item.GetComponentsInChildren<Item>().Length != 0)
                {
                    Item[] theseItems = item.GetComponentsInChildren<Item>();
                    foreach (Item thisItem in theseItems)
                    {
                        thisItem.platform = this;
                    }
                }
                item.transform.localPosition = Vector3.zero;//itemPosition;
                item.transform.localScale = Vector3.one * .1f;
                item.transform.localEulerAngles = Vector3.zero;
                SetSprite(item.transform.GetChild(0).GetComponent<SpriteRenderer>());
            }
        }

    }

    public void SetSprite(SpriteRenderer spriteRenderer)
    {
        Texture2D chosenTexture = ImageLoader.Instance.GetImageWithIndex("Item", -1);
        spriteRenderer.sprite = Sprite.Create(chosenTexture, new Rect(0, 0, chosenTexture.width, chosenTexture.height), new Vector2(0.5f, 0.5f));
        spriteRenderer.material.SetTexture("_MainTex", chosenTexture);
    }

}
