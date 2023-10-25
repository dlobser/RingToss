using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public static class GlobalPhysics{
    public static float ballSpeed = 1;
    public static float ballGravity = 1;
    public static float platformBounce = 1;
    public static float ballSize;
}

public class RandomLevelGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject platformPrefab;
    public Material platformMaterial;
    public GameObject ringPrefab;

    [Header("Platform Settings")]
    public int minPlatforms;
    public int maxPlatforms;
    public Vector2 platformPositionBounds;
    public Vector2 platformWidthBounds;

    [Header("Ring Settings")]
    public int minRingsPerPlatform;
    public int maxRingsPerPlatform;
    
    private GameObject root;
    private List<Material> instantiatedMaterials = new List<Material>();

    public GameObject titleSprite;
    public GameObject bgSprite;

    public Vector2 minMaxPlatformAnimateBounds;
    public Vector2 minMaxPlatformAnimateSpeed;

    public Vector2 minMaxProjectileSpeed;
    public Vector2 minMaxProjectileGravity;

    public void RandomizeLevel(){
        FindObjectOfType<GameManager>().ShowNextLevel();
    }

    public GameObject GenerateLevel()
    {
        SetRandomPhysics();

        // Cleanup
        Cleanup();

        // Create the root GameObject
        root = Random.value>.5f ? GenerateRoundPlatforms() : GenerateFlatPlatforms();

        return root;
    }

    public void SetRandomPhysics(){
        float energy = Random.Range(.5f,5f);
        GlobalPhysics.ballSpeed = Random.Range(1,3)*energy;
        GlobalPhysics.ballGravity = energy*.5f;
        GlobalPhysics.platformBounce = Random.Range(.2f,.9f);
        GlobalPhysics.ballSize = Random.Range(.2f,.7f);
    }

    public GameObject GenerateFlatPlatforms(){
        root = new GameObject("Root");
        titleSprite.GetComponent<HideChildOnClick>().ShowChild();
        GetComponent<ImageLoader>().SetRandomStyle();
        GetComponent<ImageLoader>().AssignRandomImage(titleSprite.transform.GetChild(0).GetComponent<SpriteRenderer>(),"Title");
        GetComponent<ImageLoader>().AssignRandomImage(bgSprite.GetComponent<SpriteRenderer>(),"Background");

        int platformCount = Mathf.Max(1,Random.Range(minPlatforms, maxPlatforms + 1));
        float ySpacing = (platformPositionBounds.y - platformPositionBounds.x) / (platformCount - 1);
        if (float.IsNaN(ySpacing)) ySpacing = 0;

        for (int i = 0; i < platformCount; i++)
        {
            float platformPositionY = platformPositionBounds.x + (i * ySpacing);
            GameObject newPlatform = Instantiate(platformPrefab, new Vector3(0, platformPositionY, 0), Quaternion.identity, root.transform);

            TransformUniversal transformUniversal = newPlatform.AddComponent<TransformUniversal>();
            transformUniversal.doTranslateOscillate = true;
            transformUniversal.translateOscillateLowerBounds = new Vector3(minMaxPlatformAnimateBounds.x,0,0);
            transformUniversal.translateOscillateUpperBounds = new Vector3(minMaxPlatformAnimateBounds.y,0,0);
            transformUniversal.translateOscillateSpeed = new Vector3(Random.Range(minMaxPlatformAnimateSpeed.x,minMaxPlatformAnimateSpeed.y),0,0);
            transformUniversal.translateOscillateOffset = new Vector3(Random.Range(-100,100),0,0);

            // Scale the platform based on a random width
            float platformWidth = Random.Range(platformWidthBounds.x, platformWidthBounds.y);
            SpriteRenderer spriteRenderer = newPlatform.transform.GetChild(0).GetComponent<SpriteRenderer>();
            PhysicsMaterial2D physMat = Instantiate(newPlatform.transform.GetChild(0).GetComponent<BoxCollider2D>().sharedMaterial);
            newPlatform.transform.GetChild(0).GetComponent<BoxCollider2D>().sharedMaterial = physMat;
            physMat.bounciness = GlobalPhysics.platformBounce;
            Material mat = Instantiate(platformMaterial);
            instantiatedMaterials.Add(mat);  // Track the instantiated material
            spriteRenderer.material = mat;
            GetComponent<ImageLoader>().AssignRandomImage(spriteRenderer,"Platforms",true);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Random.Range(0, 1f));
            spriteRenderer.size = new Vector2(platformWidth, spriteRenderer.size.y);

            int ringsForThisPlatform = Random.Range(minRingsPerPlatform, maxRingsPerPlatform + 1);
            int timeout = 0;
            while(timeout < 1000 && ringsForThisPlatform*9 > platformWidth)
            {
                timeout++;
                ringsForThisPlatform = Random.Range(minRingsPerPlatform, maxRingsPerPlatform + 1);
                // print(ringsForThisPlatform + " for platform");
                // print(platformWidth + " platform width");
                // print(timeout);
            }

            float spacing = platformWidth / (ringsForThisPlatform + 1);

            for (int j = 1; j <= ringsForThisPlatform; j++)
            {
                Vector3 ringPosition = newPlatform.transform.position + new Vector3((spacing * j - platformWidth*.5f)*spriteRenderer.transform.localScale.x, 0.5f, 0);
                GameObject newRing = Instantiate(ringPrefab, ringPosition, Quaternion.identity, newPlatform.transform);
                GetComponent<ImageLoader>().AssignRandomImage(newRing.transform.GetChild(0).GetComponent<SpriteRenderer>(),"Items");
            }
        }
        return root;
    }

    public GameObject GenerateRoundPlatforms()
    {
        root = new GameObject("Root");
        
        titleSprite.GetComponent<HideChildOnClick>().ShowChild();
        GetComponent<ImageLoader>().SetRandomStyle();
        GetComponent<ImageLoader>().AssignRandomImage(titleSprite.transform.GetChild(0).GetComponent<SpriteRenderer>(), "Title");
        GetComponent<ImageLoader>().AssignRandomImage(bgSprite.GetComponent<SpriteRenderer>(), "Background");

        int platformCount = Mathf.Max(1, Random.Range(minPlatforms, maxPlatforms + 1));

        // Calculate the angle of separation between each platform.
        float angle = 360.0f / platformCount;

        for (int i = 0; i < platformCount; i++)
        {
            GameObject rotator = new GameObject("Rotator");
            rotator.transform.SetParent(root.transform);

            // Calculate the platform position in a circle

            // float radius = 4;
            // float theta = angle * i * Mathf.Deg2Rad;
            // float x = radius * Mathf.Cos(theta);
            // float y = radius * Mathf.Sin(theta);
            
            GameObject newPlatform = Instantiate(platformPrefab, new Vector3(0, Random.Range(-1,platformPositionBounds.x), 0), Quaternion.identity, rotator.transform);
            
            TransformUniversal transformUniversal = newPlatform.AddComponent<TransformUniversal>();
            transformUniversal.doTranslateOscillate = true;
            transformUniversal.translateOscillateLowerBounds = new Vector3(0,minMaxPlatformAnimateBounds.x*.1f,0);
            transformUniversal.translateOscillateUpperBounds = new Vector3(0,minMaxPlatformAnimateBounds.y*.1f,0);
            transformUniversal.translateOscillateSpeed = new Vector3(0,Random.Range(minMaxPlatformAnimateSpeed.x*5,minMaxPlatformAnimateSpeed.y*5),0);
            transformUniversal.translateOscillateOffset = new Vector3(Random.Range(-100,100),0,0);

            float platformWidth = Random.Range(platformWidthBounds.x, platformWidthBounds.y)*.5f;
            SpriteRenderer spriteRenderer = newPlatform.transform.GetChild(0).GetComponent<SpriteRenderer>();
            PhysicsMaterial2D physMat = Instantiate(newPlatform.transform.GetChild(0).GetComponent<BoxCollider2D>().sharedMaterial);
            newPlatform.transform.GetChild(0).GetComponent<BoxCollider2D>().sharedMaterial = physMat;
            physMat.bounciness = GlobalPhysics.platformBounce;
            Material mat = Instantiate(platformMaterial);
            instantiatedMaterials.Add(mat);  // Track the instantiated material
            spriteRenderer.material = mat;
            GetComponent<ImageLoader>().AssignRandomImage(spriteRenderer,"Platforms",true);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Random.Range(0, 1f));
            spriteRenderer.size = new Vector2(platformWidth, spriteRenderer.size.y);

            int ringsForThisPlatform = Random.Range(minRingsPerPlatform, maxRingsPerPlatform + 1);
            int timeout = 0;
            
            while(timeout < 1000 && ringsForThisPlatform*9 > platformWidth)
            {
                timeout++;
                ringsForThisPlatform = Random.Range(minRingsPerPlatform, maxRingsPerPlatform + 1);
                // print(ringsForThisPlatform + " for platform");
                // print(platformWidth + " platform width");
                // print(timeout);
            }

            float spacing = platformWidth / (ringsForThisPlatform + 1);

            for (int j = 1; j <= ringsForThisPlatform; j++)
            {
                Vector3 ringPosition = newPlatform.transform.position + new Vector3((spacing * j - platformWidth*.5f)*spriteRenderer.transform.localScale.x, 0.5f, 0);
                GameObject newRing = Instantiate(ringPrefab, ringPosition, Quaternion.identity, newPlatform.transform);
                GetComponent<ImageLoader>().AssignRandomImage(newRing.transform.GetChild(0).GetComponent<SpriteRenderer>(),"Items");
            }

            rotator.transform.localEulerAngles = new Vector3(0,0,((float)i/(float)platformCount)*360);

        }

        TransformUniversal tUniversal = root.gameObject.AddComponent<TransformUniversal>();
        tUniversal.doRotateOscillate = true;
        tUniversal.rotateOscillateLowerBounds = new Vector3(0,0,Random.Range(-180,-360));
        tUniversal.rotateOscillateUpperBounds = new Vector3(0,0,Random.Range(180,360));
        tUniversal.rotateOscillateSpeed = new Vector3(0,0,Random.Range(.05f,.2f));
        
        return root;

    }

    public GameObject GenerateRoundPlatforms2(){

        root = new GameObject("Root");
        
        titleSprite.GetComponent<HideChildOnClick>().ShowChild();
        GetComponent<ImageLoader>().SetRandomStyle();
        GetComponent<ImageLoader>().AssignRandomImage(titleSprite.transform.GetChild(0).GetComponent<SpriteRenderer>(),"Title");
        GetComponent<ImageLoader>().AssignRandomImage(bgSprite.GetComponent<SpriteRenderer>(),"Background");

        int platformCount = Mathf.Max(1,Random.Range(minPlatforms, maxPlatforms + 1));
        float ySpacing = (platformPositionBounds.y - platformPositionBounds.x) / (platformCount - 1);
        if (float.IsNaN(ySpacing)) ySpacing = 0;

        for (int i = 0; i < platformCount; i++)
        {   
            GameObject rotator = new GameObject("Rotator");
        
            float platformPositionY = platformPositionBounds.x + (i * ySpacing);
            GameObject newPlatform = Instantiate(platformPrefab, new Vector3(0, platformPositionY, 0), Quaternion.identity, root.transform);

            TransformUniversal transformUniversal = newPlatform.AddComponent<TransformUniversal>();
            transformUniversal.doTranslateOscillate = true;
            transformUniversal.translateOscillateLowerBounds = new Vector3(minMaxPlatformAnimateBounds.x,0,0);
            transformUniversal.translateOscillateUpperBounds = new Vector3(minMaxPlatformAnimateBounds.y,0,0);
            transformUniversal.translateOscillateSpeed = new Vector3(Random.Range(minMaxPlatformAnimateSpeed.x,minMaxPlatformAnimateSpeed.y),0,0);
            transformUniversal.translateOscillateOffset = new Vector3(Random.Range(-100,100),0,0);

            // Scale the platform based on a random width
            float platformWidth = Random.Range(platformWidthBounds.x, platformWidthBounds.y);
            SpriteRenderer spriteRenderer = newPlatform.transform.GetChild(0).GetComponent<SpriteRenderer>();
            Material mat = Instantiate(platformMaterial);
            instantiatedMaterials.Add(mat);  // Track the instantiated material
            spriteRenderer.material = mat;
            GetComponent<ImageLoader>().AssignRandomImage(spriteRenderer,"Platforms",true);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Random.Range(0, 1f));
            spriteRenderer.size = new Vector2(platformWidth, spriteRenderer.size.y);

            int ringsForThisPlatform = Random.Range(minRingsPerPlatform, maxRingsPerPlatform + 1);
            int timeout = 0;
            while(timeout < 1000 && ringsForThisPlatform*9 > platformWidth)
            {
                timeout++;
                ringsForThisPlatform = Random.Range(minRingsPerPlatform, maxRingsPerPlatform + 1);
                // print(ringsForThisPlatform + " for platform");
                // print(platformWidth + " platform width");
                // print(timeout);
            }

            float spacing = platformWidth / (ringsForThisPlatform + 1);

            for (int j = 1; j <= ringsForThisPlatform; j++)
            {
                Vector3 ringPosition = newPlatform.transform.position + new Vector3((spacing * j - platformWidth*.5f)*spriteRenderer.transform.localScale.x, 0.5f, 0);
                GameObject newRing = Instantiate(ringPrefab, ringPosition, Quaternion.identity, newPlatform.transform);
                GetComponent<ImageLoader>().AssignRandomImage(newRing.transform.GetChild(0).GetComponent<SpriteRenderer>(),"Items");
            }
        }
        
        return root;

    }

    // public GameObject Generate 

    private void Cleanup()
    {
        if (root != null)
        {
            Destroy(root);
        }

        // Cleanup materials
        foreach (Material mat in instantiatedMaterials)
        {
            Destroy(mat);
        }
        instantiatedMaterials.Clear();
    }
}

// using UnityEngine;

// public class RandomLevelGenerator : MonoBehaviour
// {
//     [Header("Prefabs")]
//     public GameObject platformPrefab;
//     public Material platformMaterial;
//     public GameObject ringPrefab;

//     [Header("Platform Settings")]
//     public int minPlatforms;
//     public int maxPlatforms;
//     public Vector2 platformPositionBounds;
//     public Vector2 platformWidthBounds;

//     [Header("Ring Settings")]
//     public int minRingsPerPlatform;
//     public int maxRingsPerPlatform;
    
//     private GameObject root;

//     public void GenerateLevel()
//     {
//         // Cleanup
//         if(root != null)
//         {
//             DestroyImmediate(root);
//         }

//         // Create the root GameObject
//         root = new GameObject("Root");

//         int platformCount = Random.Range(minPlatforms, maxPlatforms + 1);

//         float ySpacing = (platformPositionBounds.y - platformPositionBounds.x) / (platformCount - 1);

//         for (int i = 0; i < platformCount; i++)
//         {
//             float platformPositionY = platformPositionBounds.x + (i * ySpacing);
//             GameObject newPlatform = Instantiate(platformPrefab, new Vector3(0, platformPositionY, 0), Quaternion.identity, root.transform);

//             TransformUniversal transformUniversal = newPlatform.AddComponent<TransformUniversal>();
//             transformUniversal.doTranslateNoise = true;
//             transformUniversal.translateNoiseLowerBounds = new Vector3(-3.3f,0,0);
//             transformUniversal.translateNoiseUpperBounds = new Vector3(3.3f,0,0);
//             transformUniversal.translateNoiseSpeed = new Vector3(Random.Range(.02f,.1f),0,0);
//             transformUniversal.translateNoiseOffset = new Vector3(Random.Range(-100,100),0,0);


//             // Scale the platform based on a random width
//             float platformWidth = Random.Range(platformWidthBounds.x, platformWidthBounds.y);
//             SpriteRenderer spriteRenderer = newPlatform.transform.GetChild(0).GetComponent<SpriteRenderer>();
//             Material mat = Instantiate(platformMaterial);
//             spriteRenderer.material = mat;
//             spriteRenderer.color = new Color(mat.color.r,mat.color.g,mat.color.b,Random.Range(0,255));

//             spriteRenderer.size = new Vector2(platformWidth,spriteRenderer.size.y);
//             // newPlatform.transform.localScale = new Vector3(platformWidth, newPlatform.transform.localScale.y, newPlatform.transform.localScale.z);

//             int ringsForThisPlatform = Random.Range(minRingsPerPlatform, maxRingsPerPlatform + 1);
//             int timeout = 0;
//             while(timeout < 1000 && ringsForThisPlatform*9 > platformWidth){
//                 timeout++;
//                 ringsForThisPlatform = Random.Range(minRingsPerPlatform, maxRingsPerPlatform + 1);
//             }

//             float spacing = platformWidth / (ringsForThisPlatform + 1);

//             for (int j = 1; j <= ringsForThisPlatform; j++)
//             {
//                 Vector3 ringPosition = newPlatform.transform.position + new Vector3((spacing * j - platformWidth*.5f)*spriteRenderer.transform.localScale.x, 0.5f, 0);
//                 GameObject newRing = Instantiate(ringPrefab, ringPosition, Quaternion.identity, newPlatform.transform);
//             }
//         }
//     }
// }
