using System;
using UnityEngine;

public enum CustomTag
{
    Boundary,
    Item,
    Projectile,
    Platform,
}


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

    public static int randomSeed;

}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public RandomLevelGenerator levelGenerator;

    [HideInInspector]
    public GameObject root;
    // [HideInInspector]
    public GameScoreKeeper gameScoreKeeper;

    public GameObject rootParent;
    public MenuManager menus;

    public event Action GameStart;

    // public int randomSeed;
    
    // private int seed;

    // public int Seed
    // {
    //     get
    //     {
    //         return GlobalSettings.randomSeed;
    //     }
    //     set
    //     {
    //         seed = value;
    //         GlobalSettings.randomSeed = value;
    //         UnityEngine.Random.InitState(GlobalSettings.randomSeed);
    //     }
    // }


    private void Awake()
    {
        Instance = this;
        UnityEngine.Random.InitState(GlobalSettings.randomSeed);
    }

    private void Start()
    {
        GenerateGame();
    }

    public void GenerateGame()
    {
        LevelStart();
    }

    public void LevelStart()
    {
        // gameScoreKeeper.OnLevelStart();
        print(UnityEngine.Random.seed);
        GameStart?.Invoke();
    }

    public void ShowMenu(string name)
    {
        menus.ShowMenu(name);
    }

}
