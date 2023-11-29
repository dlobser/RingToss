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
        totalScore = 0;
        if (projectilesUI != null)
            projectilesUI.text = usedProjectiles + " / " + totalProjectiles;
        if (scoreUI != null)
            scoreUI.text = totalScore + "";

    }

    public override void IncrementScore(int amount)
    {
        base.IncrementScore(amount);
        // scoreUI.text = score + " / " + totalItemsInLevel;
        // print("ON LEVEL START");

    }

    public override void IncrementProjectile()
    {
        usedProjectiles++;
        if (projectilesUI != null)
            projectilesUI.text = usedProjectiles + " / " + totalProjectiles;
        CheckLevelFinished();
    }

    public override void CheckLevelFinished()
    {
        if (scoreUI != null)
            scoreUI.text = items + " / " + totalItemsInLevel;
        base.CheckLevelFinished();
        if(usedProjectiles>totalProjectiles)
            GameManager.Instance.GenerateGame();
        
    }
}
