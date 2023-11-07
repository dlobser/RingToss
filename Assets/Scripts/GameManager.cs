using System;
using UnityEngine;

public enum CustomTag
{
    Boundary,
    Item,
    Projectile,
    Platform,
    // Add more custom tags as needed
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int score;
    public bool levelFinished = false;
    public int totalTargetsInLevel;

    // public Transform levelsParent; // Reference to the "Levels" GameObject
    public RandomLevelGenerator levelGenerator;
    public GameObject root;
    public GameScoreKeeper gameScoreKeeper;
    // public int currentLevelIndex = 0; // Index of the current level
    public int maxLevels = 100;

    public event Action IncrementScore;
    public event Action GameStart;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        score = 0;
        // levelsParent = GameObject.Find("Levels").transform; // Assuming the "Levels" object exists
        // ShowLevel(currentLevelIndex); // Show the initial level

        root = levelGenerator.GenerateLevel();
        LevelStart();
    }

    public void AddToScore()
    {
        // score++;
        // CheckLevelFinished();
        IncrementScore?.Invoke();
    }

    public int GetScore()
    {
        return score;
    }

    public void GenerateGame(){
        root = levelGenerator.GenerateLevel();
        LevelStart();
    }

    public void LevelStart()
    {
        GameScreenManager.Instance.ShowScreen(GameScreenManager.Screen.Title);
        gameScoreKeeper.OnLevelStart();
        GameStart?.Invoke();
        // totalTargetsInLevel = 0;
        // // Find all GameObjects with the 'Target' tag
        // Item[] targets = root.GetComponentsInChildren<Item>();// FindObjectsOfType<Target>();
        // foreach (Item t in targets)
        // {
        //     if (t.customTag == CustomTag.Item)
        //         totalTargetsInLevel++;
        // }
        // print(targets.Length);
    }

    // public void EndGame()
    // {
    //     // Implement game over logic
    // }

    // public void CheckLevelFinished()
    // {
    //     // If the score matches the number of 'Target' objects, set 'levelFinished' to true
    //     if (score >= totalTargetsInLevel)
    //     {
    //         levelFinished = true;
    //         ShowNextLevel();
    //     }
    // }

    // private void ShowLevel(int levelIndex)
    // {
    //     // for (int i = 0; i < levelsParent.childCount; i++)
    //     // {
    //     //     levelsParent.GetChild(i).gameObject.SetActive(i == levelIndex);
    //     // }
    // }

    // public void ShowNextLevel()
    // {
    //     // currentLevelIndex++;
    //     // if (currentLevelIndex < maxLevels)
    //     // {
    //         // ShowLevel(currentLevelIndex);
    //         root = levelGenerator.GenerateLevel();
    //         // ResetTargets();
    //         LevelStart();
    //     //     // levelFinished = false;
    //     //     // score = 0;
    //     // }
    //     // else
    //     // {
    //     //     // All levels are finished, implement game completion logic
    //     // }
    // }

    // private void ResetTargets()
    // {
    //     // Reset the number of targets for the next level
    //     totalTargetsInLevel = 0;
    //     // Implement any other logic for resetting the level
    // }
}
