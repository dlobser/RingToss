// using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using JetBrains.Annotations;
using Unity.VisualScripting;

// using System.Numerics;

// using System.Windows.Forms;
// using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using UnityEngine;

public class LevelGenerator_SkeeBall_Round : LevelGenerator
{
    [System.Serializable]
    public class LevelItems
    {
        [Header("Prefabs")]
        public Platform platformPrefab;
        public GameObject forcePrefab;
        public GameObject wallPrefab;
        public Material platformMaterial;

        [Header("Platform Settings")]
        public int minPlatforms;
        public int maxPlatforms;
        public Vector2 platformPositionBounds;
        public Vector2 platformWidthBounds;
        public float platformHeight = 1;
        public float platformPositionYOffset = 0;

        [Header("Item Settings")]
        public int minItemsPerPlatform;
        public int maxItemsPerPlatform;
        public float itemSize;
        public float minPlatformSizeForItems = .4f;

        [HideInInspector]
        public List<Material> instantiatedMaterials = new List<Material>();

        public Vector2 minMaxPlatformAnimateBounds;
        public Vector2 minMaxPlatformAnimateSpeed;

        public Vector2 minMaxProjectileSpeed;
        public Vector2 minMaxProjectileGravity;

        public GameObject target;
        public GameObject bonusTarget;
        public float ballSize;
    }

    [SerializeField]
    public LevelItems levelItems;
    public bool reload = false;
    public int randomSeed;
    public bool randomizeSeed = false;
    List<Platform> allPlatforms;
    List<GameObject> movingPlatforms;
    List<GameObject> platformsWithItems;

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
        allPlatforms = new List<Platform>();
        movingPlatforms = new List<GameObject>();
        platformsWithItems = new List<GameObject>();
    }

    void Update()
    {
        if (reload)
        {
            GenerateLevel();
            reload = false;
        }
    }

    public GameObject GeneratePlatformPositions()
    {

        print("Random Seed For Platforms: " + GlobalSettings.randomSeed);
        Random.InitState(GlobalSettings.randomSeed);
        GameObject platforms = new GameObject("Platforms");
        allPlatforms = new List<Platform>();

        // if(root!=null)
        //     platforms.transform.SetParent(root.transform);

        int platformCount = Mathf.Max(levelItems.minPlatforms, levelItems.maxPlatforms);
        List<Vector3> initCircles = new List<Vector3>();
        initCircles.Add(new Vector3(0, 0, levelItems.platformWidthBounds.y));

        for (int i = 0; i < platformCount; i++)
        {
            initCircles.Add(new Vector3(
                Random.Range(0, levelItems.platformPositionBounds.x),
                Random.Range(-levelItems.platformPositionBounds.y, levelItems.platformPositionBounds.y),
                Random.Range(levelItems.platformWidthBounds.x, levelItems.platformWidthBounds.y)));
        }

        // {
        //     new Vector3(0, 0,levelItems.platformWidthBounds.y)

        // };

        List<Vector3> circles = PackCircles(initCircles);
        // CirclePacker.PackCircles(
        //     levelItems.platformWidthBounds.x, 
        //     levelItems.platformWidthBounds.y, 
        //     platformCount,levelItems.platformPositionBounds.x,
        //     levelItems.platformPositionBounds.y,
        //     initCircles);

        print(circles.Count);
        circles = ProcessCircles(circles);
        // circles = MoveCirclesCloser(circles);

        for (int i = 0; i < circles.Count; i++)
        {
            // GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // sphere.transform.position = new Vector3(circles[i].x,circles[i].y+levelItems.platformPositionYOffset,0);
            // sphere.transform.localScale = new Vector3(1,1,.001f) * circles[i].z;
            // sphere.transform.SetParent(platforms.transform);
            // allPlatforms.Add(sphere.AddComponent<Platform>());

            Platform newPlatform = Instantiate(levelItems.platformPrefab, platforms.transform);
            float platformWidth = Random.Range(levelItems.platformWidthBounds.x, levelItems.platformWidthBounds.y);
            newPlatform.SetSize(new Vector2(circles[i].z, Mathf.Min(circles[i].z, levelItems.platformHeight)));
            newPlatform.SetPosition(new Vector3(circles[i].x, circles[i].y + levelItems.platformPositionYOffset, 0));
            allPlatforms.Add(newPlatform);
        }

        return platforms;
    }

    public List<Vector3> PackCircles(List<Vector3> circles)
    {
        int overFlow = 10000;
        int count = 0;
        bool overlapping = true;

        while (count < overFlow && overlapping)
        {
            overlapping = false;

            for (int i = 0; i < circles.Count; i++)
            {
                for (int j = i + 1; j < circles.Count; j++)
                {
                    Vector3 A = circles[i];
                    Vector3 B = circles[j];

                    float distance = Vector2.Distance(new Vector2(A.x, A.y), new Vector2(B.x, B.y));
                    if (B.x + B.z > levelItems.platformPositionBounds.x || B.x - B.z < -levelItems.platformPositionBounds.x)
                    {
                        circles[j] = new Vector3(B.x, B.y, B.z - .01f);
                        if (circles[j].z < .3f)
                        {
                            circles.RemoveAt(j);
                            j--; // Decrement j because the list size has decreased
                            continue; // Skip to the next iteration
                        }
                        overlapping = true;
                    }
                    if (distance < (A.z + B.z) * .5f)
                    {
                        overlapping = true;
                        Vector3 angle = new Vector3(B.x - A.x, B.y - A.y, 0);
                        angle.Normalize();
                        circles[j] = new Vector3(B.x + angle.x * .01f, B.y + angle.y * .01f, B.z);
                    }
                }
            }
            count++;
            if (count > overFlow - 1)
            {
                print("Overflow");
                for (int i = 0; i < circles.Count; i++)
                {
                    for (int j = 1; j < circles.Count; j++)
                    {
                        if (i == j) continue;
                        Vector3 A = circles[i];
                        Vector3 B = circles[j];
                        float distance = Vector2.Distance(new Vector2(A.x, A.y), new Vector2(B.x, B.y));
                        if (distance < (A.z + B.z) * .5f)
                        {
                            circles[j] = new Vector3(-1, 0, 0);
                        }
                    }
                }
            }

        }
        return circles;

    }


    // public List<Vector3> MoveCirclesCloser(List<Vector3> circles){
    //     int overFlow  = 1000;
    //     int count = 0;
    //     bool overlapping = true;
    //     bool[] combined = new bool[circles.Count];
    //     combined[0] = true;
    //     while(count<overFlow && overlapping==true){
    //         overlapping = false;
    //         for (int i = 0; i < circles.Count; i++)
    //         {
    //             for (int j = 1; j < circles.Count; j++)
    //             {
    //                 if(i==j) continue;
    //                 if(combined[j]) continue;
    //                 Vector3 A = circles[i];
    //                 Vector3 B = circles[j];
    //                 float distance = Vector2.Distance(new Vector2(A.x,A.y),new Vector2(B.x,B.y));
    //                 if(distance<(A.z+B.z)*.5f){
    //                     combined[j] = true;
    //                     break;
    //                 }
    //                 if(distance>(A.z+B.z)*.5f){
    //                     overlapping = true;
    //                     Vector3 angle = new Vector3(B.x-A.x,B.y-A.y,0);
    //                     angle.Normalize();
    //                     circles[j] = new Vector3(B.x - angle.x * .01f, B.y - angle.y * .01f, B.z);
    //                 }
    //             }
    //         }
    //         count++;
    //     }
    //     return circles;
    // }

    public void GeneratePlatforms()
    {
        Random.InitState(GlobalSettings.randomSeed);
        GameObject platforms = GeneratePlatformPositions();
        // root = FindObjectOfType<GameParent>().gameObject;
        // GameManager.Instance.root = root;
        if (root == null)
        {
            Debug.LogError("No GameParent, attach GameParent component to an object in the menu");
            // root = new GameObject("Root");
        }
        platforms.transform.SetParent(root.transform);
        // int platformCount = Mathf.Max(1, Random.Range(levelItems.minPlatforms, levelItems.maxPlatforms + 1));

        // // CirclePacker.randomSeed = randomSeed;
        // List<Vector3> circles = CirclePacker.PackCircles(levelItems.platformWidthBounds.x, levelItems.platformWidthBounds.y, platformCount,levelItems.platformPositionBounds.y);
        // print("Packed Circles: " + circles.Count + ", platform count: " + platformCount);
        // circles = ProcessCircles(circles);

        platformsWithItems = new List<GameObject>();
        // circles = CombineOverlappingCircles(circles);

        bool even = false;
        bool evenForce = false;
        float bounds = Random.Range(30, 180);
        float speed = Random.Range(.5f, 1.5f);
        float platformStyleCoinFlip = Random.value;
        for (int i = 0; i < allPlatforms.Count; i++)
        {

            Platform newPlatform = allPlatforms[i];

            newPlatform.SetPhysicsBounciness(GlobalSettings.Physics.platformBounce);
            Material mat = Instantiate(levelItems.platformMaterial);
            // Texture2D chosenTexture = ImageLoader.Instance.GetImageWithIndex("Platform", GlobalSettings.ImageIndeces.Platform);
            // newPlatform.SetMainTexture(Sprite.Create(chosenTexture, new Rect(0, 0, chosenTexture.width, chosenTexture.height), new Vector2(0.5f, 0.5f)));
            newPlatform.SetAlpha(Random.Range(0, 1f));
            TransformUniversal trans = newPlatform.AddComponent<TransformUniversal>();

            trans.doRotateOscillate = true;
            trans.rotateOscillateLowerBounds = new Vector3(0, 0, -bounds);
            trans.rotateOscillateUpperBounds = new Vector3(0, 0, bounds);

            trans.rotateOscillateSpeed = new Vector3(0, 0, even ? speed : speed * -1);
            // newPlatform.transform.LookAt(Vector2.zero, Vector3.forward);
            if (i % 2 == 0)
            {
                bounds = Random.Range(-180, 180);
                speed = Random.Range(.5f, 1.5f);
            }
            else
            {
                even = !even;
                platformStyleCoinFlip = Random.value;
            }

            // newPlatform.SetSize(new Vector2(circles[i].z/2, Mathf.Min(circles[i].z/2, levelItems.platformHeight)));
            // newPlatform.SetPosition(new Vector3(circles[i].x,circles[i].y,0));

            if (newPlatform.platformScale.x * .5f > levelItems.minPlatformSizeForItems)
            {
                platformsWithItems.Add(newPlatform.gameObject);
                // TransformUniversal trans = newPlatform.AddComponent<TransformUniversal>();

                // trans.doRotateOscillate = true;
                // trans.rotateOscillateLowerBounds = new Vector3(0, 0, -bounds);
                // trans.rotateOscillateUpperBounds = new Vector3(0, 0, bounds);

                // trans.rotateOscillateSpeed = new Vector3(0, 0, even ? speed : speed * -1);
            }
            else
            {   
                if (even)
                {
                    RotateObjectXAxisTowardsTarget(newPlatform.transform, Vector3.up, Vector3.up);

                    foreach (Transform child in newPlatform.transform)
                    {
                        child.gameObject.SetActive(false);
                    }
                    GameObject force = Instantiate(levelItems.forcePrefab, newPlatform.transform);
                    force.transform.localPosition = Vector3.zero;
                    force.transform.localScale = Vector3.one * newPlatform.platformScale.x;
                    float angle = even ? 0 : 180;
                    force.transform.localEulerAngles = new Vector3(0, 0, evenForce ? 0 : 180);
                    float randomAmount = Mathf.Max(newPlatform.platformPosition.y,0)*130;
                    force.transform.Rotate(new Vector3(0, 0, Random.Range(-randomAmount,randomAmount)));
                    evenForce = !evenForce;

                    trans.doRotateOscillate = false;


                    // CapsuleCollider capsuleCollider = newPlatform.GetComponent<CapsuleCollider>();

                    // if (capsuleCollider != null)
                    // {
                    //     CircleCollider2D circleCollider = newPlatform.gameObject.AddComponent<CircleCollider2D>();
                    //     circleCollider.radius = boxCollider.size.x;
                    //     newPlatform.GetComponent<Rigidbody>().isKinematic = false;
                    //     Destroy(boxCollider);
                    // }
                }
                else
                {
                    RotateObjectXAxisTowardsTarget(newPlatform.transform, Vector3.up, Vector3.up);

                    foreach (Transform child in newPlatform.transform)
                    {
                        child.gameObject.SetActive(false);
                    }
                    trans.doRotateOscillate = false;

                    GameObject wall = Instantiate(levelItems.wallPrefab, newPlatform.transform);
                    // TransformUniversal transformUniversal = wall.AddComponent<TransformUniversal>();
                    // TransformUniversal trans = wall.AddComponent<TransformUniversal>();
                    // trans.doRotate = true;
                    // trans.rotate = new Vector3(0, 0, speed * 10);//even ? speed : speed * -1);
                    // transformUniversal.doScaleOscillate = true;
                    // transformUniversal.scaleOscillateLowerBounds = new Vector3(-newPlatform.platformScale.x, -newPlatform.platformScale.x, 0);
                    // transformUniversal.scaleOscillateSpeed = new Vector3(2, 2, 0);
                    // transformUniversal.scaleOscillateOffset = new Vector3(newPlatform.transform.localPosition.y, newPlatform.transform.localPosition.y, 0);
                    wall.transform.localPosition = Vector3.zero;
                    wall.transform.localScale = Vector3.one * newPlatform.platformScale.x;
                    float angle = even ? 0 : 180;
                    wall.transform.localEulerAngles = new Vector3(0, 0, evenForce ? 0 : 180);
                    // evenForce = !evenForce;
                }
            }
        }

        for (int i = 0; i < platformsWithItems.Count; i++)
        {
            GameObject thisPlatform = platformsWithItems[i];
            PlatformItemArguments itemArguments = new PlatformItemArguments();
            itemArguments.item = levelItems.target;
            itemArguments.bonusItem = levelItems.bonusTarget;
            itemArguments.amount = Random.Range(levelItems.minItemsPerPlatform, levelItems.maxItemsPerPlatform + 1);
            itemArguments.offset = new Vector3(0, levelItems.platformHeight * .5f + levelItems.itemSize * .5f, 0);
            itemArguments.size = levelItems.itemSize;
            thisPlatform.GetComponent<Platform>().PopulatePlatformWithItems(itemArguments);

        }
    }

    public void RotateObjectXAxisTowardsTarget(Transform obj, Vector3 targetPos, Vector3 upDirection)
    {
        // Create a temporary GameObject to assist in aligning the X-axis
        GameObject tempObj = new GameObject("TempObjectForAlignment");
        tempObj.transform.position = obj.position;
        tempObj.transform.LookAt(targetPos, upDirection); // Align Z-axis to the target

        // Rotate the temporary object so that its X-axis points towards the target
        tempObj.transform.Rotate(0, 90, 0);
        // if tempobj position x is greater than 0, rotate 180
        if (tempObj.transform.position.x < 0)
        {
            tempObj.transform.Rotate(180, 0, 0);
            tempObj.transform.Rotate(0, 0, 180);
        }




        // Apply the same rotation to the actual object
        obj.rotation = tempObj.transform.rotation;


        // Clean up the temporary object
        Destroy(tempObj);
    }

    // public void GeneratePlatforms(){
    //     Random.InitState(GlobalSettings.randomSeed);
    //     GameObject platforms = new GameObject("Platforms");
    //     if(root==null){
    //         root = new GameObject("Root");
    //     }
    //     platforms.transform.SetParent(root.transform);
    //     int platformCount = Mathf.Max(1, Random.Range(levelItems.minPlatforms, levelItems.maxPlatforms + 1));

    //     // CirclePacker.randomSeed = randomSeed;
    //     List<Vector3> circles = CirclePacker.PackCircles(levelItems.platformWidthBounds.x, levelItems.platformWidthBounds.y, platformCount,levelItems.platformPositionBounds.y);
    //     print("Packed Circles: " + circles.Count + ", platform count: " + platformCount);
    //     circles = ProcessCircles(circles);

    //     platformsWithItems = new List<GameObject>();
    //     // circles = CombineOverlappingCircles(circles);

    //     for (int i = 0; i < Mathf.Min(circles.Count,platformCount); i++)
    //     {
    //         // GameObject rotator = new GameObject("Rotator");
    //         // rotator.transform.SetParent(platforms.transform);

    //         Platform newPlatform = Instantiate(levelItems.platformPrefab, platforms.transform);

    //         float platformWidth = Random.Range(levelItems.platformWidthBounds.x, levelItems.platformWidthBounds.y);
    //         SpriteRenderer spriteRenderer = newPlatform.transform.GetChild(0).GetComponent<SpriteRenderer>();

    //         newPlatform.SetPhysicsBounciness(GlobalSettings.Physics.platformBounce);

    //         Material mat = Instantiate(levelItems.platformMaterial);
    //         Texture2D chosenTexture = ImageLoader.Instance.GetImageWithIndex("Platform", GlobalSettings.ImageIndeces.Platform);
    //         newPlatform.SetMainTexture(Sprite.Create(chosenTexture, new Rect(0, 0, chosenTexture.width, chosenTexture.height), new Vector2(0.5f, 0.5f)));
    //         newPlatform.SetAlpha(Random.Range(0, 1f));
    //         newPlatform.SetSize(new Vector2(circles[i].z/2, Mathf.Min(circles[i].z/2, levelItems.platformHeight)));
    //         newPlatform.SetPosition(new Vector3(circles[i].x,circles[i].y,0));

    //         if(circles[i].z*.5f > levelItems.minPlatformSizeForItems)
    //             platformsWithItems.Add(newPlatform.gameObject);
    //         else{
    //             BoxCollider2D boxCollider = newPlatform.GetComponent<BoxCollider2D>();

    //             if (boxCollider != null)
    //             {
    //                 CircleCollider2D circleCollider = newPlatform.gameObject.AddComponent<CircleCollider2D>();
    //                 circleCollider.radius = boxCollider.size.x;
    //                 Destroy(boxCollider);
    //             }
    //         }
    //     }

    //     for (int i = 0; i < platformsWithItems.Count; i++)
    //     {
    //         GameObject thisPlatform = platformsWithItems[i];
    //         PlatformItemArguments itemArguments = new PlatformItemArguments();
    //         itemArguments.item = levelItems.target;
    //         itemArguments.bonusItem = levelItems.bonusTarget;
    //         itemArguments.amount = Random.Range(levelItems.minItemsPerPlatform, levelItems.maxItemsPerPlatform + 1);
    //         itemArguments.offset = new Vector3(0, levelItems.platformHeight * .5f + levelItems.itemSize * .5f, 0);
    //         itemArguments.size = levelItems.itemSize;
    //         thisPlatform.GetComponent<Platform>().PopulatePlatformWithItems(itemArguments);

    //     }
    // }

    public List<Vector3> ProcessCircles(List<Vector3> originalCircles)
    {
        List<Vector3> processedCircles = new List<Vector3>();

        foreach (Vector3 circle in originalCircles)
        {
            // Only process circles on the right side (x >= 0)
            if (circle.x - (circle.z * .5f) > 0 &&
            circle.x + (circle.z * .5f) < levelItems.platformPositionBounds.x &&
            circle.y - (circle.z * .5f) < levelItems.platformPositionBounds.y)
            {
                // Add the original circle

                // Create and add the mirrored circle
                Vector3 shrinkCircle = new Vector3(-circle.x, circle.y, circle.z);
                Vector3 mirroredCircle = new Vector3(-shrinkCircle.x, shrinkCircle.y, shrinkCircle.z);
                // if(Vector3.Distance(mirroredCircle,circle)<circle.z*2){
                //     processedCircles.Add(new Vector3(0,mirroredCircle.y,mirroredCircle.z));
                // }
                // else{
                processedCircles.Add(shrinkCircle);
                processedCircles.Add(mirroredCircle);
                // }
            }
            else if (circle.x == 0)
            {
                // Add the original circle
                processedCircles.Add(circle);
            }
        }
        // processedCircles[0] = new Vector3(processedCircles[0].x,processedCircles[0].y,processedCircles[0].z*2);
        return processedCircles;
    }


    private List<Vector3> CombineOverlappingCircles(List<Vector3> circles)
    {
        bool[] combined = new bool[circles.Count];
        List<Vector3> combinedCircles = new List<Vector3>();

        for (int i = 0; i < circles.Count; i++)
        {
            if (combined[i]) continue;

            for (int j = 0; j < circles.Count; j++)
            {
                if (j == i)
                {
                    j++;
                    continue;
                }
                else
                {
                    if (combined[j]) continue;

                    Vector3 circleA = circles[i];
                    Vector3 circleB = circles[j];

                    float distance = Vector2.Distance(new Vector2(circleA.x, circleA.y), new Vector2(circleB.x, circleB.y));
                    if (distance < circleA.z + circleB.z) // Check if circles overlap
                    {
                        // Combine circles
                        combinedCircles.Add(new Vector3(0, circleA.y, circleA.z));
                        combined[j] = true; // Mark the second circle as combined
                        combined[i] = true;
                    }
                }
            }
        }

        // Filter out combined circles
        List<Vector3> result = new List<Vector3>();
        for (int i = 0; i < circles.Count; i++)
        {
            if (!combined[i])
            {
                result.Add(circles[i]);
            }
        }


        // result.AddRange(combinedCircles);

        return result;
    }

    public override void GenerateLevel()
    {
        if (randomizeSeed)
        {
            System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            int currentUnixTimeSeconds = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
            Random.InitState(currentUnixTimeSeconds);
        }

        base.GenerateLevel();
        root = GameManager.Instance.root;
        SetRandomPhysics();
        SetRandomStyle();

        // if (levelSettings.gameManager.rootParent != null)
            // root.transform.SetParent(levelSettings.gameManager.rootParent.transform);

        levelSettings.gameManager.gameScoreKeeper = levelSettings.scoreKeeper;

        menuManager.ModifyMenu("Title", (menu) =>
        {
            menu.menuObject.GetComponent<GameScreenWithSimpleSprite>().spriteRenderer.sprite = ImageLoader.Instance.GetSpriteWithIndex("Title", GlobalSettings.ImageIndeces.Style);
        });

        menuManager.ModifyMenu("Game", (menu) =>
        {
            menu.menuObject.GetComponent<GameScreenWithSimpleSprite>().spriteRenderer.sprite = ImageLoader.Instance.GetSpriteWithIndexSeed("Background", -1, GlobalSettings.randomSeed);
        });

        GeneratePlatforms();
        OnGenerateLevelComplete();

    }

    public override void SetRandomPhysics()
    {
        float energy = Random.Range(levelItems.minMaxProjectileSpeed.x, levelItems.minMaxProjectileSpeed.y);
        GlobalSettings.Physics.ballSpeed = Random.Range(levelItems.minMaxProjectileSpeed.x, levelItems.minMaxProjectileSpeed.y);
        GlobalSettings.Physics.ballGravity = GlobalSettings.Physics.ballSpeed * .2f;// Random.Range(levelItems.minMaxProjectileGravity.x,levelItems.minMaxProjectileGravity.y);
        GlobalSettings.Physics.platformBounce = Random.Range(.8f, .99f);
        GlobalSettings.Physics.ballSize = levelItems.ballSize;//Random.Range(.1f, .2f);
    }

    public override void SetRandomStyle()
    {
        GlobalSettings.ImageIndeces.Style = int.Parse(ImageLoader.Instance.styleNum);
        GlobalSettings.ImageIndeces.BG = FindObjectOfType<ImageLoader>().GetRandomStyleNum("Background");
        // GlobalSettings.ImageIndeces.Platform = FindObjectOfType<ImageLoader>().GetRandomStyleNum("Platform");
        GlobalSettings.ImageIndeces.Title = FindObjectOfType<ImageLoader>().GetRandomStyleNum("Title");

    }

}
