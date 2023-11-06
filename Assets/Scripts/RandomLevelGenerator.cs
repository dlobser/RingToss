using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;


public static class GlobalSettings{

    public static class ImageIndeces{
        public static int Style = 0;
        public static int BG;
        public static int Platform;
        public static int Title;
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
        FindObjectOfType<GameManager>().ShowNextLevel();
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

    // public void SetRandomPhysics(){
    //     float energy = Random.Range(.5f,5f);
    //     GlobalSettings.Physics.ballSpeed = Random.Range(1,3)*energy;
    //     GlobalSettings.Physics.ballGravity = energy*.5f;
    //     GlobalSettings.Physics.platformBounce = Random.Range(.2f,.9f);
    //     GlobalSettings.Physics.ballSize = Random.Range(.2f,.7f);
    // }

    // public void SetRandomStyle(){
    //     GlobalSettings.ImageIndeces.BG = FindObjectOfType<ImageLoader>().GetRandomStyleNum("Background");
    //     print("BG Index: " + GlobalSettings.ImageIndeces.BG);
    //     GlobalSettings.ImageIndeces.Platform = FindObjectOfType<ImageLoader>().GetRandomStyleNum("Platform");
    //     GlobalSettings.ImageIndeces.Title = FindObjectOfType<ImageLoader>().GetRandomStyleNum("Title");

    // }
   
    void Cleanup()
    {
        if (root != null)
        {
            // Destroy(root);
        }

    }
}
