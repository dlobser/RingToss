using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;



public class RandomLevelGenerator : MonoBehaviour
{

    // private GameObject root;
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
        UnityEngine.Random.InitState(GlobalSettings.randomSeed);
        FindObjectOfType<ImageLoader>().SetRandomStyle();
        if(levelGenerators.Length>0)
            levelGenerators[(int)(Random.value * levelGenerators.Length)].GenerateLevel();
        // root = levelGenerators[(int)(Random.value * levelGenerators.Length)].GenerateLevel();
        // return root;
    }


    void Cleanup()
    {
        // if (root != null)
        // {
        //     // Destroy(root);
        // }

    }
}
