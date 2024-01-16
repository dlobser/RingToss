using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quilt{

    public class EventArgs{

    }

    public class CollisionEventArgs : EventArgs
    {
        public GameObject ColliderObject { get; set; }
        public GameObject CollidedObject { get; set; }
        public Vector3 HitPoint { get; set; }
        public Vector3 Velocity { get; set; }

        public CollisionEventArgs(GameObject colliderObject, GameObject collidedObject, Vector3 hitPoint, Vector3 velocity)
        {
            ColliderObject = colliderObject;
            CollidedObject = collidedObject;
            HitPoint = hitPoint;
            Velocity = velocity;
        }
    }



    public static class Globals
    {
        
        public static bool debugA = true;
        public static bool debugB;
        public static bool debugC;

        

        public static void DebugA(string message){
            if(debugA){
                Debug.Log(message);
            }
        }

        public enum CustomTag
        {
            Boundary,
            BoundaryDestroyer,
            BoundaryDestructable,
            Item,
            ItemSpecial,
            ItemBonus,
            Projectile,
            Platform,
            PlatformWithEffect,
            PlatformWithItem,
        }

        public static Transform GetRoot(){
            return GlobalSettings.LevelGlobals.root;
        }

        public static Transform GetGameRoot(){
            return GlobalSettings.LevelGlobals.gameRoot;
        }

        public static Transform GetRootParent(){
            return GlobalSettings.LevelGlobals.rootParent;
        }

        public static Transform GetMenuRoot(){
            return GlobalSettings.LevelGlobals.menuRoot;
        }

        public static Transform GetManagersRoot(){
            return GlobalSettings.LevelGlobals.managersRoot;
        }

        public static GameManager GetGameManager(){
            return GlobalSettings.Managers.gameManager;
        }

        public static MenuManager GetMenuManager(){
            return GlobalSettings.Managers.menuManager;
        }

        public static LevelGenerator GetLevelGenerator(){
            return GlobalSettings.Managers.levelGenerator;
        }

        public static ScoreManager GetScoreManager(){
            return GlobalSettings.Managers.scoreManager;
        }

        public static InteractionManager GetInteractionManager(){
            return GlobalSettings.Managers.interactionManager;
        }

        public static FXManager GetFXManager(){
            return GlobalSettings.Managers.fxManager;
        }

        public static AudioManager GetAudioManager(){
            return GlobalSettings.Managers.audioManager;
        }

        public static EventManager GetEventManager(){
            return GlobalSettings.Managers.eventManager;
        }

        public static class GlobalSettings
        {
            public static void ResetGlobalSettings()
            {
                // Reset Image Indices
                ImageIndeces.Style = 0;
                ImageIndeces.BG = 0;
                ImageIndeces.Platform = 0;
                ImageIndeces.Title = 0;
                ImageIndeces.Projectile = 0;
                ImageIndeces.Font = 0;
                ImageIndeces.Emitter = 0;

                // Reset Physics
                Physics.projectileSpeed = 1.0f;
                Physics.projectileGravity = 1.0f;
                Physics.platformBounce = 1.0f;

                // Reset Level Globals
                LevelGlobals.projectileSize = 1.0f; // Assuming default size as 1.0f
                LevelGlobals.root = null; // References should be reassigned at runtime
                LevelGlobals.gameRoot = null;
                LevelGlobals.rootParent = null;
                LevelGlobals.menuRoot = null;
                LevelGlobals.managersRoot = null;

                // Reset Managers
                Managers.gameManager = null;
                Managers.menuManager = null;
                Managers.levelGenerator = null;
                Managers.scoreManager = null;
                Managers.interactionManager = null;
                Managers.fxManager = null;
                Managers.audioManager = null;
                Managers.eventManager = null;
                Managers.uiManager = null;

                // Reset other GlobalSettings variables
                randomSeed = 0; // Reset to zero or some other default seed
            }

            public static class ImageIndeces
            {
                public static int Style = 0;
                public static int BG = 0;
                public static int Platform = 0;
                public static int Title = 0;
                public static int Projectile = 0;
                public static int Font = 0;
                public static int Emitter = 0;
                public static string GetStyleString()
                {
                    return Style.ToString("D4");
                }
            }

            public static class Physics
            {
                public static float projectileSpeed = 1;
                public static float projectileGravity = 1;
                public static float platformBounce = 1;
            }

            public static class LevelGlobals{
                public static float projectileSize;
                public static Transform root;
                public static Transform gameRoot;
                public static Transform rootParent;
                public static Transform menuRoot;
                public static Transform managersRoot;
            }

            public static class Managers{
                public static GameManager gameManager;
                public static MenuManager menuManager;
                public static LevelGenerator levelGenerator;
                public static ScoreManager scoreManager;
                public static InteractionManager interactionManager;
                public static FXManager fxManager;
                public static AudioManager audioManager;
                public static EventManager eventManager;
                public static UIManager uiManager;
            }

            public static int randomSeed;

        }

    }
}