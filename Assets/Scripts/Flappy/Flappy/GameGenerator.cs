using UnityEngine;
using Quilt;

namespace Quilt.Flappy{
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
                float platformSize = Random.Range(flappySettings.minMaxPlatformSize.x,flappySettings.minMaxPlatformSize.y);
                separation = Random.Range(flappySettings.minMaxPlatformSeparation.x,flappySettings.minMaxPlatformSeparation.y);
                player = Instantiate(flappySettings.projectilePrefab,Globals.GetGameRoot()).gameObject;
                player.transform.localScale = Vector3.one*platformSize*.5f;
                if(Globals.GetInteractionManager() is InteractionManager interactionManager)
                {
                    interactionManager.player = player;
                }
                else
                {
                    Debug.LogError("InteractionManager is not of type InteractionManager");
                }

                cam = Instantiate(flappySettings.camera,Globals.GetGameRoot()).GetComponent<Camera>();

                for (int i = 0; i < platformCount; i++)
                {
                    Platform platform = Instantiate(flappySettings.platformPrefab, Globals.GetGameRoot());
                    platform.transform.position = new Vector3(i*separation, Random.Range(flappySettings.minMaxPlatformHeight.x, flappySettings.minMaxPlatformHeight.y), 0);
                    platform.transform.localScale = new Vector3(platformSize, platformSize, 1);
                    platform.transform.SetParent(Globals.GetGameRoot().transform);
                }
            }
        }

        void Update(){
            cam.transform.position = new Vector3(
                Mathf.Lerp(cam.transform.position.x,player.transform.position.x+.75f,Time.deltaTime*10), 
                0, -10);
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