using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using UnityEngine;

public class LevelGenerator_Stacked : LevelGenerator
{
    [System.Serializable]
    public class LevelItems
    {
        [Header("Prefabs")]
        public Platform platformPrefab;
        public Material platformMaterial;
        public GameObject ringPrefab;
        public GameScoreKeeper scoreKeeper;

        [Header("Platform Settings")]
        public int minPlatforms;
        public int maxPlatforms;
        public Vector2 platformPositionBounds;
        public Vector2 platformWidthBounds;
        public float platformHeight = 1;

        [Header("Item Settings")]
        public int minItemsPerPlatform;
        public int maxItemsPerPlatform;
        public float itemSize;

        [HideInInspector]
        public List<Material> instantiatedMaterials = new List<Material>();

        public GameObject titleSprite;
        public GameObject bgSprite;

        public Vector2 minMaxPlatformAnimateBounds;
        public Vector2 minMaxPlatformAnimateSpeed;

        public Vector2 minMaxProjectileSpeed;
        public Vector2 minMaxProjectileGravity;

        public GameObject target;
        public GameObject bonusTarget;
    }

    [SerializeField]
    public LevelItems levelItems;

    // ImageLoader imageLoader;
    public bool reload = false;

    public override void Destroy()
    {
        base.Destroy();

        foreach (Material mat in levelItems.instantiatedMaterials)
        {
            Destroy(mat);
        }

        levelItems.instantiatedMaterials.Clear();

        Destroy(this.gameObject);

    }

    void Start()
    {
        // imageLoader = FindObjectOfType<ImageLoader>();
        // print(imageLoader);
    }

    void Update()
    {
        if (reload)
        {
            GenerateLevel();
            reload = false;
        }
    }

    public override GameObject GenerateLevel()
    {

        SetRandomPhysics();
        SetRandomStyle();

        if (GameManager.instance.root != null)
            Destroy(GameManager.instance.root);

        if (root != null)
        {
            Destroy(root);
        }

        root = new GameObject("Root");

        if (rootParent != null)
            root.transform.SetParent(rootParent.transform);

        GameManager.instance.gameScoreKeeper = levelItems.scoreKeeper;

        levelItems.titleSprite.GetComponentInChildren<SpriteRenderer>().sprite = ImageLoader.Instance.GetSpriteWithIndex("Title", GlobalSettings.ImageIndeces.Style);
        levelItems.bgSprite.GetComponentInChildren<SpriteRenderer>().sprite = ImageLoader.Instance.GetSpriteWithIndex("Background", GlobalSettings.ImageIndeces.Style);
        // imageLoader.GetImageWithIndex("Background", GlobalSettings.ImageIndeces.Style);

        int platformCount = Mathf.Max(1, Random.Range(levelItems.minPlatforms, levelItems.maxPlatforms + 1));

        // Calculate the angle of separation between each platform.
        float angle = 360.0f / platformCount;

        for (int i = 0; i < platformCount; i++)
        {
            // GameObject rotator = new GameObject("Rotator");
            // rotator.transform.SetParent(root.transform);

            Platform newPlatform = Instantiate(levelItems.platformPrefab, root.transform);

            TransformUniversal transformUniversal = newPlatform.gameObject.AddComponent<TransformUniversal>();
            transformUniversal.doTranslateOscillate = true;
            transformUniversal.translateOscillateLowerBounds = new Vector3(levelItems.minMaxPlatformAnimateBounds.x * .1f, 0, 0);
            transformUniversal.translateOscillateUpperBounds = new Vector3(levelItems.minMaxPlatformAnimateBounds.y * .1f, 0, 0);
            transformUniversal.translateOscillateSpeed = new Vector3(Random.Range(levelItems.minMaxPlatformAnimateSpeed.x * 5, levelItems.minMaxPlatformAnimateSpeed.y * 5), 0, 0);
            transformUniversal.translateOscillateOffset = new Vector3(Random.Range(-100, 100), 0, 0);

            float platformWidth = Random.Range(levelItems.platformWidthBounds.x, levelItems.platformWidthBounds.y);
            SpriteRenderer spriteRenderer = newPlatform.transform.GetChild(0).GetComponent<SpriteRenderer>();

            newPlatform.SetPhysicsBounciness(GlobalSettings.Physics.platformBounce);

            Material mat = Instantiate(levelItems.platformMaterial);
            // newPlatform.SetMaterial(mat);
            // GetComponent<ImageLoader>().AssignRandomImage(spriteRenderer,"Platforms",true);
            Texture2D chosenTexture = ImageLoader.Instance.GetImageWithIndex("Platform", GlobalSettings.ImageIndeces.Platform);
            // print(chosenTexture);
            newPlatform.SetMainTexture(Sprite.Create(chosenTexture, new Rect(0, 0, chosenTexture.width, chosenTexture.height), new Vector2(0.5f, 0.5f)));
            newPlatform.SetAlpha(Random.Range(0, 1f));
            newPlatform.SetSize(new Vector2(platformWidth, levelItems.platformHeight));
            float l = (float)i / (float)platformCount;
            float height = Mathf.Lerp(levelItems.platformPositionBounds.x, levelItems.platformPositionBounds.y, l);
            newPlatform.transform.localPosition = new Vector3(0, height);

            int ringsForThisPlatform = Random.Range(levelItems.minItemsPerPlatform, levelItems.maxItemsPerPlatform + 1);
            // int timeout = 0;

            // while(timeout < 1000 && ringsForThisPlatform*9 > platformWidth)
            // {
            //     timeout++;
            //     ringsForThisPlatform = Random.Range(levelItems.minRingsPerPlatform, levelItems.maxRingsPerPlatform + 1);
            // }

            // float spacing = platformWidth / (ringsForThisPlatform + 1);

            // for (int j = 1; j <= ringsForThisPlatform; j++)
            // {
            //     Vector3 ringPosition = newPlatform.transform.position + new Vector3((spacing * j - platformWidth*.5f)*spriteRenderer.transform.localScale.x, 0.5f, 0);
            //     GameObject newRing = Instantiate(levelItems.ringPrefab, ringPosition, Quaternion.identity, newPlatform.transform);
            //     GetComponent<ImageLoader>().AssignRandomImage(newRing.transform.GetChild(0).GetComponent<SpriteRenderer>(),"Items");
            // }

            PlatformItemArguments itemArguments = new PlatformItemArguments();
            itemArguments.item = levelItems.target;
            itemArguments.bonusItem = levelItems.bonusTarget;
            itemArguments.amount = Random.Range(levelItems.minItemsPerPlatform, levelItems.maxItemsPerPlatform + 1);
            itemArguments.offset = new Vector3(0, levelItems.platformHeight * .5f + levelItems.itemSize * .5f, 0);
            itemArguments.size = levelItems.itemSize;
            newPlatform.PopulatePlatformWithItems(itemArguments);

            // rotator.transform.localEulerAngles = new Vector3(0, 0, ((float)i / (float)platformCount) * 360);

        }

        // TransformUniversal tUniversal = root.gameObject.AddComponent<TransformUniversal>();
        // tUniversal.doRotateOscillate = true;
        // tUniversal.rotateOscillateLowerBounds = new Vector3(0, 0, Random.Range(-180, -360));
        // tUniversal.rotateOscillateUpperBounds = new Vector3(0, 0, Random.Range(180, 360));
        // tUniversal.rotateOscillateSpeed = new Vector3(0, 0, Random.Range(.05f, .2f));

        return root;

    }

    public void SetRandomPhysics()
    {
        float energy = Random.Range(.5f, 5f);
        GlobalSettings.Physics.ballSpeed = Random.Range(1, 3) * energy;
        GlobalSettings.Physics.ballGravity = energy * .5f;
        GlobalSettings.Physics.platformBounce = Random.Range(.2f, .9f);
        GlobalSettings.Physics.ballSize = Random.Range(.2f, .7f);
    }

    public void SetRandomStyle()
    {
        GlobalSettings.ImageIndeces.Style = int.Parse(ImageLoader.Instance.styleNum);
        GlobalSettings.ImageIndeces.BG = FindObjectOfType<ImageLoader>().GetRandomStyleNum("Background");
        GlobalSettings.ImageIndeces.Platform = FindObjectOfType<ImageLoader>().GetRandomStyleNum("Platform");
        GlobalSettings.ImageIndeces.Title = FindObjectOfType<ImageLoader>().GetRandomStyleNum("Title");

    }

}
