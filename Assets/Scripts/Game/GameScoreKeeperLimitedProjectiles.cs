using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using TMPro;

public class GameScoreKeeperLimitedProjectiles : GameScoreKeeper
{

    // public int score;
    // public int totalItemsInLevel;
    public int totalProjectiles;
    public int usedProjectiles;

    public TextMeshProUGUI scoreUI;
    public TextMeshProUGUI projectilesUI;
    // public bool win;
    // private GameManager gameManager;

    // void Awake()
    // {
    //     gameManager = FindObjectOfType<GameManager>();
    // }

    public override void RegisterActions()
    {
        // Subscribe to the event
        // GameManager.Instance.IncrementScore  += OnIncrementScore;
        gameManager.GameStart += OnLevelStart;
    }

    public override void DeregisterActions()
    {
        // Unsubscribe from the event
        // GameManager.Instance.IncrementScore  -= OnIncrementScore;
        gameManager.GameStart -= OnLevelStart;
    }

    public override void OnLevelStart()
    {
        // totalItemsInLevel = 0;
        // score = 0;
        // // Find all GameObjects with the 'Target' tag
        // Item[] targets = GameManager.Instance.root.GetComponentsInChildren<Item>();// FindObjectsOfType<Target>();
        // foreach (Item t in targets)
        // {
        //     if (t.customTag == CustomTag.Item)
        //         totalItemsInLevel++;
        // }
        base.OnLevelStart();
        usedProjectiles = 0;
        score = 0;
        projectilesUI.text = usedProjectiles + " / " + totalProjectiles;
        scoreUI.text = score + " / " + totalItemsInLevel;

    }

    public override void IncrementScore()
    {
        base.IncrementScore();
        // scoreUI.text = score + " / " + totalItemsInLevel;
        // print("ON LEVEL START");

    }

    public override void IncrementProjectile()
    {
        usedProjectiles++;
        projectilesUI.text = usedProjectiles + " / " + totalProjectiles;
        CheckLevelFinished();
    }

    public override void CheckLevelFinished()
    {
        scoreUI.text = score + " / " + totalItemsInLevel;
        // If the score matches the number of 'Target' objects, set 'levelFinished' to true
        if (score >= totalItemsInLevel || usedProjectiles > totalProjectiles)
        {
            GameManager.Instance.GenerateGame();
        }
    }
}
