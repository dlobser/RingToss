using UnityEngine;
using Quilt;

namespace Quilt.Flappy
{
    public class GameGenerator : Quilt.GameGenerator
    {
        float separation;
        GameObject player;
        Camera cam;

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

                for (int i = 1; i < platformCount; i++)
                {
                    flappySettings.platformConfig.size = platformSize;
                    Platform platform = Instantiate(generatedPlatform.GetComponent<Platform>(), Globals.GetGameRoot());
                    platform.transform.position = new Vector3(i * separation, Random.Range(flappySettings.minMaxPlatformHeight.x, flappySettings.minMaxPlatformHeight.y), 0);
                    platform.transform.localScale = new Vector3(platformSize, platformSize, 1);
                    platform.transform.SetParent(Globals.GetGameRoot().transform);
                }

                Destroy(generatedPlatform);
            }
        }

        void Update()
        {
            if (cam != null && player != null)
                cam.transform.position = new Vector3(
                    Mathf.Lerp(cam.transform.position.x, player.transform.position.x + .75f, Time.deltaTime * 10),
                    0, -10);

            Debug.Log(_managersGameObject);
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