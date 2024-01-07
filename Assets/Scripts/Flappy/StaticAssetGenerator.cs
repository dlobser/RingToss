using UnityEngine;

namespace Quilt
{
    public enum ObjectType
    {
        PlatformSimpleItem,
        PlatformHoop,
        ProjectileSimple,
        // Other types
    }

    public abstract class GeneratorConfig
    {
        public ObjectType type;
        public string name;
    }

    [System.Serializable]
    public class PlatformSimpleItemConfig : GeneratorConfig
    {
        public float size = 1;
        public CollisionBehaviorSettings collisionSettings;
        public string texturePath;
        // Specific properties for TypeA
    }

    [System.Serializable]
    public class PlatformHoopConfig : GeneratorConfig
    {
        public float size = 1;
        public CollisionBehaviorSettings pointCollisionSettings;
        public CollisionBehaviorSettings blockerCollisionSettings;
        public string texturePath;
        // Specific properties for TypeB
    }

    [System.Serializable]
    public class ProjectileSimpleConfig : GeneratorConfig
    {
        public float size = 1;
        public float gravity;
        public float mass;
        public string texturePath;
        // Specific properties for TypeA
    }

    public static class StaticAssetGenerator
    {
        public static GameObject GenerateAsset(GeneratorConfig config)
        {
            GameObject generatedObject;

            switch (config.type)
            {
                case ObjectType.PlatformSimpleItem:
                    generatedObject = GeneratePlatformSimpleItem(config as PlatformSimpleItemConfig);
                    break;
                case ObjectType.PlatformHoop:
                    generatedObject = GeneratePlatformHoop(config as PlatformHoopConfig);
                    break;
                case ObjectType.ProjectileSimple:
                    generatedObject = GenerateProjectileSimple(config as ProjectileSimpleConfig);
                    break;
                default:
                    generatedObject = new GameObject("Empty");
                    break;
            }

            // Common object setup (if any)

            return generatedObject;
        }

        private static GameObject GeneratePlatformSimpleItem(PlatformSimpleItemConfig config)
        {
            GameObject root = new GameObject(config.name);
            root.transform.localPosition = Vector3.zero;
            root.transform.localScale = Vector3.one * config.size;
            root.transform.localRotation = Quaternion.identity;

            root.AddComponent<Platform>();

            Rigidbody2D rb = root.AddComponent<Rigidbody2D>();
            rb.isKinematic = true;

            string texturePath = config.texturePath;
            Texture2D texture = Resources.Load<Texture2D>(texturePath);

            GameObject child = new GameObject("Child");
            child.transform.parent = root.transform;
            child.transform.localScale = Vector3.one;

            SpriteRenderer spriteRenderer = child.AddComponent<SpriteRenderer>();
            if (texture != null)
            {
                Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.width);
                spriteRenderer.sprite = newSprite;
            }
            else
            {
                Debug.LogError("Texture not found at path: " + texturePath);
            }

            CircleCollider2D c = child.AddComponent<CircleCollider2D>();
            c.isTrigger = true;
            CollisionBehavior collisionBehavior = child.AddComponent<CollisionBehavior>();
            collisionBehavior.settings = config.collisionSettings;

            return root;
        }

        private static GameObject GeneratePlatformHoop(PlatformHoopConfig config)
        {
            GameObject root = new GameObject(config.name);
            root.transform.localPosition = Vector3.zero;
            root.transform.localScale = Vector3.one * config.size;
            root.transform.localRotation = Quaternion.identity;

            root.AddComponent<Platform>();

            Rigidbody2D rb = root.AddComponent<Rigidbody2D>();
            rb.isKinematic = true;

            string texturePath = config.texturePath;
            Texture2D texture = Resources.Load<Texture2D>(texturePath);

            // Main scoring point object
            GameObject scoringPoint = new GameObject("Scorer");//CreateChildWithSprite(root, texture, "ScoringPoint");
            scoringPoint.transform.parent = root.transform;
            scoringPoint.AddComponent<BoxCollider2D>();
            scoringPoint.GetComponent<BoxCollider2D>().size = new Vector2(1, 0.2f);
            scoringPoint.GetComponent<BoxCollider2D>().isTrigger = true;
            scoringPoint.transform.localScale = Vector3.one;
            CollisionBehavior collision = scoringPoint.AddComponent<CollisionBehavior>();
            collision.settings = config.pointCollisionSettings;

            // Left blocker object
            GameObject leftBlocker = CreateChildWithSprite(root, texture, "LeftBlocker");
            leftBlocker.AddComponent<CircleCollider2D>();
            leftBlocker.transform.localPosition = new Vector3(-0.5f, 0, 0);
            leftBlocker.transform.localScale = Vector3.one * .025f;

            // Right blocker object
            GameObject rightBlocker = CreateChildWithSprite(root, texture, "RightBlocker");
            rightBlocker.AddComponent<CircleCollider2D>();
            rightBlocker.transform.localPosition = new Vector3(0.5f, 0, 0);
            rightBlocker.transform.localScale = Vector3.one * .025f;

            // Bottom box object
            GameObject bottomBox = new GameObject("BottomBox");
            bottomBox.transform.parent = root.transform;
            bottomBox.transform.localPosition = new Vector3(0, -0.3f, 0);
            bottomBox.transform.localScale = Vector3.one;
            CollisionBehavior bottomCollision = bottomBox.AddComponent<CollisionBehavior>();
            bottomCollision.settings = config.blockerCollisionSettings;
            bottomBox.transform.parent = scoringPoint.transform;


            BoxCollider2D boxCollider = bottomBox.AddComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(1, 0.2f);

            return root;
        }

        private static GameObject CreateChildWithSprite(GameObject parent, Texture2D texture, string childName)
        {
            GameObject child = new GameObject(childName);
            child.transform.parent = parent.transform;
            child.transform.localScale = Vector3.one;

            SpriteRenderer spriteRenderer = child.AddComponent<SpriteRenderer>();
            if (texture != null)
            {
                Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                spriteRenderer.sprite = newSprite;
            }
            else
            {
                Debug.LogError("Texture not found at path");
            }

            return child;
        }


        private static GameObject GenerateProjectileSimple(ProjectileSimpleConfig config)
        {
            GameObject root = new GameObject(config.name);
            root.transform.localPosition = Vector3.zero;
            root.transform.localScale = Vector3.one * config.size;
            root.transform.localRotation = Quaternion.identity;

            Rigidbody2D rb = root.AddComponent<Rigidbody2D>();
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.mass = config.mass;
            rb.gravityScale = config.gravity;

            string texturePath = config.texturePath;
            Texture2D texture = Resources.Load<Texture2D>(texturePath);

            SpriteRenderer spriteRenderer = root.AddComponent<SpriteRenderer>();
            if (texture != null)
            {
                Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.width);
                spriteRenderer.sprite = newSprite;
            }
            else
            {
                Debug.LogError("Texture not found at path: " + texturePath);
            }

            CircleCollider2D c = root.AddComponent<CircleCollider2D>();

            return root;
        }
    }
}
