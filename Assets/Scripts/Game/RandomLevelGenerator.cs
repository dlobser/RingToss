using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;


public static class GlobalSettings
{

    public static class ImageIndeces
    {
        public static int Style = 0;
        public static int BG;
        public static int Platform;
        public static int Title;
        public static int Projectile;
        public static string GetStyleString()
        {
            return Style.ToString("D4");
        }
    }

    public static class Physics
    {
        public static float ballSpeed = 1;
        public static float ballGravity = 1;
        public static float platformBounce = 1;
        public static float ballSize;
    }

}

public class RandomLevelGenerator : MonoBehaviour
{

    private GameObject root;
    private GameManager gameManager;

    public LevelGenerator[] levelGenerators;

    public void RandomizeLevel()
    {
        FindObjectOfType<GameManager>().GenerateGame();
    }

    private void OnEnable()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.GameStart += OnGameStart;
    }

    private void OnDisable()
    {
        gameManager.GameStart -= OnGameStart;
    }

    private void OnGameStart()
    {
        // Do something when the game starts
        // ShowMenu("Title");
        Debug.Log("Game has started!");
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        FindObjectOfType<ImageLoader>().SetRandomStyle();
        levelGenerators[(int)(Random.value * levelGenerators.Length)].GenerateLevel();
        // root = levelGenerators[(int)(Random.value * levelGenerators.Length)].GenerateLevel();
        // return root;
    }

    void Cleanup()
    {
        if (root != null)
        {
            // Destroy(root);
        }

    }
}
