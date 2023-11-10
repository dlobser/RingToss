using System;
using UnityEngine;

public enum CustomTag
{
    Boundary,
    Item,
    Projectile,
    Platform,
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

    private void Awake()
    {
        Instance = this;
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
        GameStart?.Invoke();
    }

    public void ShowMenu(string name)
    {
        menus.ShowMenu(name);
    }

}
