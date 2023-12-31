using System;
using UnityEngine;

public enum CustomTag
{
    Boundary,
    Item,
    ItemSpecial,
    Projectile,
    Platform,
    PlatformBouncy,
    PlatformWithItem,
    PlatformWithForce,
    BonusItem
}

public static class GlobalSettings
{

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
    
    public PlayerController playerController;

    public GameObject rootParent;
    // public MenuManager menus;

    public event Action GameStart;
    public event Action GameEnd;
    public event Action Announcement;
    private string AnnouncementMessage;

    public bool isPaused = false;

    public int maxRandomSeed = 100000;

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

    void Update()
    {
        // print("Global Seed: " + GlobalSettings.randomSeed);

    }

    public void RandomizeSeed()
    {
        GlobalSettings.randomSeed++;
        if (GlobalSettings.randomSeed > maxRandomSeed)
            GlobalSettings.randomSeed = 0;
        UnityEngine.Random.InitState(GlobalSettings.randomSeed);
        MyDebug.Instance.Log("Random Seed: " + GlobalSettings.randomSeed + " " + UnityEngine.Random.state,MyDebug.Instance.A);
    }

    private void Awake()
    {
        Instance = this;
        UnityEngine.Random.InitState(GlobalSettings.randomSeed);
    }

    private void Start()
    {
        if (!isPaused)
            GenerateGame();
    }

    public void GenerateGame()
    {
        print("Generate Game");
        LevelStart();
    }

    public void LevelStart()
    {
        GameStart?.Invoke();
    }

    public void GameOver(){
        print("Game Over");
        GameEnd?.Invoke();
    }

    public void SendAnnouncement(){
        Announcement?.Invoke();
    }

    public string GetAnnouncementMessage(){
        return AnnouncementMessage;
    }

    public void SendAnnouncement(string message){
        AnnouncementMessage = message;
        print("Announcement: " + message);
        SendAnnouncement();
    }

}
