using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class GameScoreKeeper : MonoBehaviour
{

    public int score;
    public int totalItemsInLevel;
    public bool win;
    public GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public virtual void RegisterActions()
    {
        // Subscribe to the event
        // GameManager.Instance.IncrementScore  += OnIncrementScore;
        gameManager.GameStart += OnLevelStart;
    }

    public virtual void DeregisterActions()
    {
        // Unsubscribe from the event
        // GameManager.Instance.IncrementScore  -= OnIncrementScore;
        gameManager.GameStart -= OnLevelStart;
    }

    public virtual void OnLevelStart()
    {
        totalItemsInLevel = 0;
        score = 0;
        // Find all GameObjects with the 'Target' tag
        Item[] targets = GameManager.Instance.root.GetComponentsInChildren<Item>();// FindObjectsOfType<Target>();
        foreach (Item t in targets)
        {
            if (t.customTag == CustomTag.Item)
                totalItemsInLevel++;
        }
    }

    public virtual void IncrementProjectile()
    {

    }

    public virtual void IncrementScore()
    {
        score++;
        CheckLevelFinished();
    }

    public virtual void CheckLevelFinished()
    {
        // If the score matches the number of 'Target' objects, set 'levelFinished' to true
        if (score >= totalItemsInLevel)
        {
            GameManager.Instance.GenerateGame();
        }
    }
}
