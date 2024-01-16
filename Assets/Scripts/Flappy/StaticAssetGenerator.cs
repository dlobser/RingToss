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
            CollisionBehaviorMultiPurpose collisionBehavior = child.AddComponent<CollisionBehaviorMultiPurpose>();
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
            scoringPoint.GetComponent<BoxCollider2D>().size = new Vector2(1, 0.05f);
            scoringPoint.GetComponent<BoxCollider2D>().isTrigger = true;
            scoringPoint.transform.localScale = Vector3.one;
            CollisionBehaviorMultiPurpose collision = scoringPoint.AddComponent<CollisionBehaviorMultiPurpose>();
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
            bottomBox.transform.localPosition = new Vector3(0, -0.25f, 0);
            bottomBox.transform.localScale = Vector3.one;
            CollisionBehaviorMultiPurpose bottomCollision = bottomBox.AddComponent<CollisionBehaviorMultiPurpose>();
            bottomCollision.settings = config.blockerCollisionSettings;
            bottomBox.transform.parent = scoringPoint.transform;


            BoxCollider2D boxCollider = bottomBox.AddComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(1, 0.2f);

            //set sprites to black (comment out for testing)
            foreach (Transform child in root.transform)
            {
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = new Color(0, 0, 0, 0); // Black and fully transparent
                }
            }

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

        public static GameObject GenerateBoundary(string name, Vector3 position, Vector3 scale, bool destroy = true){
            GameObject boundary = new GameObject(name);
            boundary.transform.position = position;
            boundary.transform.localScale = scale;
            boundary.transform.SetParent(Globals.GetGameRoot());
            boundary.AddComponent<BoxCollider2D>();
            boundary.GetComponent<BoxCollider2D>().isTrigger = true;
            if(destroy)
                boundary.AddComponent<CollisionBehavior_EndGame>();
            return boundary;
        } 
        
        public static GameObject GenerateSquare(string name, Vector3 position, Vector3 scale){
            GameObject obj = new GameObject(name);
            obj.transform.position = position;
            obj.transform.localScale = scale;
            obj.transform.SetParent(Globals.GetGameRoot());
            obj.AddComponent<BoxCollider2D>();
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            string texturePath = "Textures/Square";
            Texture2D texture = Resources.Load<Texture2D>(texturePath);
            if (texture != null)
            {
                Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.width);
                sr.sprite = newSprite;
            }
            return obj;
        }
        

        public static GameObject GenerateCircle(string name, Vector3 position, Vector3 scale){
            GameObject obj = new GameObject(name);
            obj.transform.position = position;
            obj.transform.localScale = scale;
            obj.transform.SetParent(Globals.GetGameRoot());
            obj.AddComponent<CircleCollider2D>();
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            string texturePath = "Textures/Circle";
            Texture2D texture = Resources.Load<Texture2D>(texturePath);
            if (texture != null)
            {
                Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.width);
                sr.sprite = newSprite;
            }
            return obj;
        }

        public static GameObject GenerateSpriteWithAlpha(string type, string alpha){
            GameObject spriteA = new GameObject("Sprite");
            SpriteRenderer srA = spriteA.AddComponent<SpriteRenderer>();
            srA.sprite = ImageLoader.GetSpriteWithIndex(type);
            srA.sprite.texture.wrapMode = TextureWrapMode.Clamp;    
            srA.material = new Material(Shader.Find("Custom/SpriteAlphaTex"));
            Sprite spriteAlpha = ImageLoader.CreateSpriteFromResource("Textures/ImagesFlappy/"+alpha);
            spriteAlpha.texture.wrapMode = TextureWrapMode.Clamp;
            srA.material.SetTexture("_AlphaTex", spriteAlpha.texture);
            spriteA.name = type;
            return spriteA;
        }

        public static GameObject GenerateRing(Transform parent = null)
        {
            GameObject p = new GameObject("Ring");
            GameObject g = StaticAssetGenerator.GenerateSpriteWithAlpha("ring", "RingAlpha");
            g.transform.SetParent(p.transform, false);
            g.transform.localPosition = new Vector3(0, -0.5f, 0);
            g.transform.localScale = Vector3.one;
            g.transform.localEulerAngles = Vector3.zero;
            
            GameObject g2 = Object.Instantiate(g, p.transform);
            g2.GetComponent<SpriteRenderer>().material.SetTextureOffset("_MainTex", new Vector2(0, 0.47f));
            g2.transform.localPosition = new Vector3(0, 0.5f, 0);
            g2.GetComponent<SpriteRenderer>().sortingOrder = -1;

            g.GetComponent<SpriteRenderer>().material.SetTextureOffset("_MainTex", new Vector2(0, -0.53f));
            g.GetComponent<SpriteRenderer>().sortingOrder = 1;
            p.transform.SetParent(parent);
            p.transform.ResetTransform();

            return p;
        }

        public static GameObject GenerateQuadWithTexture(string type, Vector2 tiling, int imageIndex = -1)
        {
            GameObject background = GameObject.CreatePrimitive(PrimitiveType.Quad);
            background.GetComponent<MeshCollider>().enabled = false;
            background.name = type;
            background.transform.SetParent(Globals.GetGameRoot());
            background.transform.ResetTransform();

            // Add MeshFilter and MeshRenderer components
            MeshRenderer meshRenderer = background.GetComponent<MeshRenderer>();

            // Set the texture
            Texture2D texture = ImageLoader.GetImageWithIndex(type, imageIndex);
            if (texture != null)
            {
                Material material = new Material(Shader.Find("Unlit/Texture")); // Use a standard shader or a custom shader if you have one
                material.mainTexture = texture;
                material.mainTextureScale = tiling; // Set tiling
                meshRenderer.material = material;
            }
            else{
                Debug.LogError("Texture not found at path: " + type + " " + imageIndex);
            }

            return background;
        }

        // public static GameObject GenerateBackgroundWithTexture(string type){
        //     GameObject background = new GameObject("Background");
        //     background.transform.SetParent(Globals.GetGameRoot());
        //     background.transform.ResetTransform();
        //     SpriteRenderer sr = background.AddComponent<SpriteRenderer>();
        //     Texture2D texture = ImageLoader.GetImageWithIndex(type);
        //     if (texture != null)
        //     {
        //         Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.width);
        //         sr.sprite = newSprite;
        //     }
        //     return background;
        // }
    }
}
