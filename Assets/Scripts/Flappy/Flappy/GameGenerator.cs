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

        public override void InitializeGame()
        {
            base.InitializeGame();
            if (creationSettings is CreationSettings flappySettings)
            {
                int platformCount = flappySettings.maxPlatforms;
                float platformSize = Random.Range(flappySettings.minMaxPlatformSize.x, flappySettings.minMaxPlatformSize.y);
                separation = Random.Range(flappySettings.minMaxPlatformSeparation.x, flappySettings.minMaxPlatformSeparation.y);

                flappySettings.projectileConfig.size = platformSize * .5f;
                player = StaticAssetGenerator.GenerateAsset(flappySettings.projectileConfig);//Instantiate(StaticAssetGenerator.GenerateAsset(flappySettings.projectileConfig), Globals.GetGameRoot()).gameObject;
                player.transform.parent = Globals.GetGameRoot().transform;
                StaticAssetGenerator.GenerateSquare("PlayerSeat",new Vector3(0,-2,0),new Vector3(3,.5f,1));
                // player.transform.localScale = Vector3.one * platformSize * .5f;

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

                        wall.transform.localScale = new Vector3(1, 10, 1);
                        wall.transform.position = platform.transform.position + new Vector3(platformSize *2 , Random.Range(-4f,-6f), 0);
                        SpriteRenderer sr = wall.AddComponent<SpriteRenderer>();
                        string texturePath = "Textures/Square";
                        Texture2D texture = Resources.Load<Texture2D>(texturePath);

                        if (texture != null)
                        {
                            Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.width);
                            sr.sprite = newSprite;
                        }

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
            if (cam != null && player != null)
                cam.transform.position = new Vector3(
                    Mathf.Lerp(cam.transform.position.x, player.transform.position.x + .75f, Time.deltaTime * 10),
                    0, -10);

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