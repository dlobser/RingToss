using UnityEngine;
using Quilt;
using Unity.VisualScripting;

namespace Quilt.Flappy
{
    public class GameGenerator : Quilt.GameGenerator
    {
        float separation;
        GameObject player;
        Camera cam;

        public GameObject[] platforms;
        int creationCount = 0;
        public override void InitializeGame()
        {
            base.InitializeGame();
            if (creationSettings is CreationSettings flappySettings)
            {
                Globals.GlobalSettings.randomSeed = Random.Range(0, 15);
                ImageLoader.Directory = "Textures/ImagesFlappy";
                
                int platformCount = flappySettings.maxPlatforms;
                float platformSize = Random.Range(flappySettings.minMaxPlatformSize.x, flappySettings.minMaxPlatformSize.y);
                separation = Random.Range(flappySettings.minMaxPlatformSeparation.x, flappySettings.minMaxPlatformSeparation.y);
                Debug.Log(++creationCount + " platforms created.");
                //make player
                flappySettings.projectileConfig.size = platformSize * .5f;
                player = StaticAssetGenerator.GenerateAsset(flappySettings.projectileConfig);//Instantiate(StaticAssetGenerator.GenerateAsset(flappySettings.projectileConfig), Globals.GetGameRoot()).gameObject;
                player.transform.parent = Globals.GetGameRoot().transform;
                GameObject ballSprite = StaticAssetGenerator.GenerateSpriteWithAlpha("ball", "BallAlpha");
                player.GetComponent<SpriteRenderer>().sprite = ballSprite.GetComponent<SpriteRenderer>().sprite;
                player.GetComponent<SpriteRenderer>().material = ballSprite.GetComponent<SpriteRenderer>().material;
                Destroy(ballSprite);

                //make background
                GameObject background = StaticAssetGenerator.GenerateQuadWithTexture("bg",
                new Vector2(platformCount,platformCount),Random.Range(0,15));
                background.transform.position = new Vector3(platformCount*separation*.5f,0,10);
                background.transform.localScale = new Vector3(platformCount*separation*2,platformCount*separation*2,1);
                GameObject BGFade = StaticAssetGenerator.GenerateSquare("BGFade",background.transform.position,background.transform.localScale*1.5f);
                BGFade.GetComponent<SpriteRenderer>().color = new Color(1,1,1,.75f);
                BGFade.GetComponent<SpriteRenderer>().sortingOrder = -1;
                BGFade.GetComponent<BoxCollider2D>().enabled = false;
                StaticAssetGenerator.GenerateSquare("PlayerSeat",new Vector3(0,-2,0),new Vector3(3,.5f,1));

                if (Globals.GetInteractionManager() is InteractionManager interactionManager)
                {
                    interactionManager.player = player;
                    Debug.Log("Player assigned to InteractionManager: " + interactionManager.player.name);
                }
                else
                {
                    Debug.LogError("InteractionManager is not of type InteractionManager");
                }

                cam = Instantiate(flappySettings.camera, Globals.GetGameRoot()).GetComponent<Camera>();
                GameObject generatedPlatform = StaticAssetGenerator.GenerateAsset(flappySettings.platformConfig);
                GameObject g = StaticAssetGenerator.GenerateRing(generatedPlatform.transform);
                g.transform.localScale = Vector3.one*1.15f;
                platforms = new GameObject[platformCount];

                for (int i = 1; i < platformCount; i++)
                {
                    flappySettings.platformConfig.size = platformSize;
                    Platform platform = Instantiate(generatedPlatform.GetComponent<Platform>(), Globals.GetGameRoot());
                    platform.transform.position = new Vector3(i * separation, Random.Range(flappySettings.minMaxPlatformHeight.x, flappySettings.minMaxPlatformHeight.y), 0);
                    platform.transform.localScale = new Vector3(platformSize, platformSize, 1);
                    platform.transform.SetParent(Globals.GetGameRoot().transform);
                    
                    

                    float fraction = ((float)i/(float)platformCount);
                    bool odds = Random.value < fraction;

                    if(odds){
                        if(Random.value > .5f)
                            platform.transform.localEulerAngles = new Vector3(0, 0, -25f);
                        else
                            platform.transform.localEulerAngles = new Vector3(0, 0, 25f);
                        
                        if (fraction > .33f && Random.value > .5f){
                            TransformUniversal tForm = platform.AddComponent<TransformUniversal>();
                            tForm.doTranslateOscillate = true;
                            tForm.translateOscillateLowerBounds = new Vector3(0, -1, 0);
                            tForm.translateOscillateUpperBounds = new Vector3(0, 1, 0);
                            tForm.translateOscillateSpeed = new Vector3(0, Random.Range(.5f,1f), 0);
                        }
                        if (fraction > .5f && Random.value > .75f){
                            TransformUniversal tForm = platform.AddComponent<TransformUniversal>();
                            tForm.doRotateOscillate = true;
                            tForm.rotateOscillateLowerBounds = new Vector3(0, 0, -25f);
                            tForm.rotateOscillateUpperBounds = new Vector3(0, 0, 25f);
                            tForm.rotateOscillateSpeed = new Vector3(0, 0, Random.Range(.5f,1f));
                        }
                    }
                    
                    GameObject boundary = StaticAssetGenerator.GenerateBoundary(
                        "Boundary",
                        platform.transform.position + new Vector3(platformSize *2 , 0, 0),
                        new Vector3(.1f, 100, 1));

                    bool turnIntoWall = Random.value > .5f;

                    if(turnIntoWall){
                        GameObject wall = StaticAssetGenerator.GenerateBoundary("Wall",
                        new Vector3(0 , -5, 0),
                        new Vector3(1, 10, 1));
                        float tiles = Random.Range(.2f,.5f);
                        GameObject tex = StaticAssetGenerator.GenerateQuadWithTexture("bg",
                        new Vector2(tiles,tiles*10),
                        (int)Random.Range(0,15));
                        tex.transform.parent = wall.transform;
                        tex.transform.ResetTransform();

                        wall.transform.localScale = new Vector3(1, 10, 1);
                        wall.transform.position = platform.transform.position + new Vector3(platformSize *2 , Random.Range(-4f,-6f), 0);
                        // SpriteRenderer sr = wall.AddComponent<SpriteRenderer>();
                        // string texturePath = "Textures/Square";
                        // Texture2D texture = Resources.Load<Texture2D>(texturePath);

                        // if (texture != null)
                        // {
                        //     Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.width);
                        //     sr.sprite = newSprite;
                        // }

                    }
                    if(Random.value>.5f){
                        GameObject fx = Instantiate(flappySettings.platforms[Random.Range(0,flappySettings.platforms.Length)],
                        Globals.GetRoot());
                        fx.transform.position = platform.transform.position + new Vector3(Random.Range(-1,1), Random.Range(2,4), 0);
                        fx.transform.localScale = Vector3.one*Random.Range(1f,2f);
                        fx.AddComponent<TransformUniversal>().doTranslateOscillate = true;
                        fx.GetComponent<TransformUniversal>().translateOscillateLowerBounds = new Vector3(0, 0, 0);
                        fx.GetComponent<TransformUniversal>().translateOscillateUpperBounds = new Vector3(0, Random.Range(0,2f), 0);
                        fx.GetComponent<TransformUniversal>().translateOscillateSpeed = new Vector3(Random.Range(.5f,1f), Random.Range(.5f,1f), 0);
                        fx.GetComponent<TransformUniversal>().doRotateOscillate = true;
                        fx.GetComponent<TransformUniversal>().rotateOscillateLowerBounds = new Vector3(0, 0, 0);
                        fx.GetComponent<TransformUniversal>().rotateOscillateUpperBounds = new Vector3(0, 0, Random.Range(-180,180));
                        fx.GetComponent<TransformUniversal>().rotateOscillateSpeed = new Vector3(Random.Range(.5f,1f), 0, Random.Range(-1,1f));
                    }

                    boundary.layer = LayerMask.NameToLayer("Walls");

                    CollisionBehavior[] platformCollisionBehavior = 
                        platform.GetComponentsInChildren<CollisionBehavior>();

                    foreach (CollisionBehavior cbs in platformCollisionBehavior)
                    {
                        cbs.settings = Instantiate(cbs.settings);
                        cbs.settings.destroyObjectsOnhit = true;
                        cbs.settings.objectsToDestroyOnHit = new GameObject[] { boundary };
                    }

                    platforms[i] = platform.gameObject;
                }

                GameObject lowerBoundary = StaticAssetGenerator.GenerateBoundary("LowerBoundary",new Vector3(0 , -5, 0),new Vector3(1000, .5f, 1));
                //  new GameObject("LowerBoundary");
                // lowerBoundary.transform.position = new Vector3(0 , -5, 0);
                // lowerBoundary.transform.localScale = new Vector3(1000, .5f, 1);
                // lowerBoundary.transform.SetParent(Globals.GetGameRoot());
                // lowerBoundary.AddComponent<BoxCollider2D>();
                // After creating the GameObject
                // boundary.layer = LayerMask.NameToLayer("Boundary");
                lowerBoundary.layer = LayerMask.NameToLayer("Walls");


                // lowerBoundary.GetComponent<BoxCollider2D>().isTrigger = true;
                // CollisionBehavior lowerCollision = lowerBoundary.AddComponent<CollisionBehavior>();
                // CollisionBehaviorSettings lowerSettings = new CollisionBehaviorSettings();
                // lowerSettings.endGame = true;
                // lowerBoundary.GetComponent<CollisionBehavior>().settings = lowerSettings;

                Destroy(generatedPlatform);
            }
        }

        void Update()
        {
            if (cam != null && player != null){
                cam.transform.position = new Vector3(
                    Mathf.Lerp(cam.transform.position.x, player.transform.position.x + .85f, Time.deltaTime * 10),
                    0, -10);
            }

            // Debug.Log(_managersGameObject);
        }

        public override void StartGame()
        {
            base.StartGame();
        }

        public override void StopGame()
        {
            base.StopGame();
        }
    }
}