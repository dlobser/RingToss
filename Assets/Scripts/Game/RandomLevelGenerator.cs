using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;


public static class GlobalSettings{

    public static class ImageIndeces{
        public static int Style = 0;
        public static int BG;
        public static int Platform;
        public static int Title;
        public static int Projectile;
        public static string GetStyleString(){
            return Style.ToString("D4");
        }
    }

    public static class Physics{
        public static float ballSpeed = 1;
        public static float ballGravity = 1;
        public static float platformBounce = 1;
        public static float ballSize;
    }

}

public class RandomLevelGenerator : MonoBehaviour
{

    private GameObject root;

    public LevelGenerator[] levelGenerators;

    public void RandomizeLevel(){
        FindObjectOfType<GameManager>().GenerateGame();
    }

    public GameObject GenerateLevel()
    {
        // Cleanup
        // Cleanup();

        // SetRandomPhysics();
        // SetRandomStyle();
        FindObjectOfType<ImageLoader>().SetRandomStyle();

        // Create the root GameObject
        root = levelGenerators[(int)(Random.value*levelGenerators.Length)].GenerateLevel();

        return root;
    }
   
    void Cleanup()
    {
        if (root != null)
        {
            // Destroy(root);
        }

    }
}
