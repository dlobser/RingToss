using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quilt
{
    [ExecuteAlways]
    public class AssetGenerator_SimplePlatform : AssetGenerator
    {
        public string texturePath; // Path of the texture in the Resources folder
        public bool addCollider = true; // Option to add or not add the collider
        public bool addPlatformComponent = true; // Option to add or not add the platform component

        public override void GenerateAsset()
        {
            // Call the base method to create the root object
            base.GenerateAsset();

            // Add a SpriteRenderer component
            SpriteRenderer spriteRenderer = root.AddComponent<SpriteRenderer>();
            Texture2D texture = Resources.Load<Texture2D>(texturePath);
            if (texture != null)
            {
                Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.width);
                spriteRenderer.sprite = newSprite;
            }
            else
            {
                Debug.LogError("Texture not found at path: " + texturePath);
            }

            // Optionally add a CircleCollider2D component
            if (addCollider)
            {
                root.AddComponent<CircleCollider2D>();
            }

            // Optionally add a 'Platform' component
            if (addPlatformComponent)
            {
                // Assuming 'Platform' is a component that exists in your project
                root.AddComponent<Platform>(); // Replace 'Platform' with the actual name of your component
            }
        }
    }
}
