using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformItemArguments
{
    public float amount = 0;
    public Vector3 offset = Vector3.zero;
    //percent
    public Vector3 bounds = Vector3.one;
    public GameObject item;
    public GameObject bonusItem;
    public float size;
}

public class Platform : MonoBehaviour
{

    public CustomTag customTag;
    public Vector3 platformScale;
    public Vector3 platformPosition;
    public Vector3 platformRotation;
    public float bounce;
    public float friction;
    // public Renderer renderer;

    public virtual void SetSize(Vector3 scale)
    {
        // this.transform.localScale = scale;
        platformScale = scale;
    }

    public virtual void SetSize(Vector2 scale)
    {
        SetSize(new Vector3(scale.x, scale.y, 1));
    }

    public virtual void SetPosition(Vector3 position)
    {
        this.platformPosition = position;
        // this.transform.localPosition = position;
    }

    public virtual void SetPosition(Vector2 position)
    {
        SetPosition(new Vector3(position.x, position.y, this.transform.localPosition.z));
    }

    public virtual void SetRotation(float z, float x = 0, float y = 0)
    {
        platformRotation = new Vector3(x, y, z);
    }

    public virtual void SetMaterial(Material mat)
    {
        // renderer.material = mat;
    }

    public virtual void SetColor(Color color)
    {

    }

    public virtual Color GetColor()
    {
        return Color.white;
    }

    public virtual void SetAlpha(float a)
    {

    }

    public virtual void SetMainTexture(Texture2D tex)
    {

    }

    public virtual void SetMainTexture(Sprite tex)
    {

    }

    public virtual void SetPhysicsBounciness(float bounceValue)
    {

    }

    public virtual void PopulatePlatformWithItems(PlatformItemArguments itemArguments)
    {

    }

    public virtual void SetColliderSize(Vector2 size)
    {

    }

    public virtual void OnItemHit()
    {

    }


}
