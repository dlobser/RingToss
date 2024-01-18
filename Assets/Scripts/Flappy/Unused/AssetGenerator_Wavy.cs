using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quilt
{
    [ExecuteAlways]
    public class AssetGenerator_Wavy : AssetGenerator
    {
        public string texturePath = "Textures/Circle"; // Path to the texture in the Resources folder
        public int numberOfCircles = 10;
        public float amplitude = 1.0f;
        public float wavelength = 1.0f;

        public override void GenerateAsset()
        {
            // Call the base method to create/reset the root object
            base.GenerateAsset();

            Texture2D texture = Resources.Load<Texture2D>(texturePath);
            if (texture == null)
            {
                Debug.LogError("Texture not found at path: " + texturePath);
                return;
            }

            Sprite circleSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.width);

            for (int i = 0; i < numberOfCircles; i++)
            {
                GameObject circle = new GameObject("Circle" + i);
                circle.transform.SetParent(root.transform);
                circle.transform.localPosition = new Vector3(i, Mathf.Sin(((float)i / (float)numberOfCircles) * Mathf.PI * 2f * wavelength) * amplitude, 0);
                circle.transform.localScale = Vector3.one;

                SpriteRenderer spriteRenderer = circle.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = circleSprite;

                // Optionally add colliders to each circle
                circle.AddComponent<CircleCollider2D>();
            }
        }
    }
}
