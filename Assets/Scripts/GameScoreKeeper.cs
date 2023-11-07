using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class GameScoreKeeper : MonoBehaviour
{

    public int score;
    public int totalItemsInLevel;
    public bool win;

    public virtual void RegisterActions()
    {
        // Subscribe to the event
        // GameManager.instance.IncrementScore  += OnIncrementScore;
        GameManager.instance.GameStart  += OnLevelStart;


    }

    public virtual void DeregisterActions()
    {
        // Unsubscribe from the event
        // GameManager.instance.IncrementScore  -= OnIncrementScore;
        GameManager.instance.GameStart  -= OnLevelStart;
    }

    public virtual void OnLevelStart(){
        totalItemsInLevel = 0;
        score = 0;
        // Find all GameObjects with the 'Target' tag
        Item[] targets = GameManager.instance.root.GetComponentsInChildren<Item>();// FindObjectsOfType<Target>();
        foreach (Item t in targets)
        {
            if (t.customTag == CustomTag.Item)
                totalItemsInLevel++;
        }
    }

    public virtual void IncrementScore(){
        score++;
        CheckLevelFinished();
    }

    public void CheckLevelFinished()
    {
        // If the score matches the number of 'Target' objects, set 'levelFinished' to true
        if (score >= totalItemsInLevel)
        {
            GameManager.instance.GenerateGame();
        }
    }
}
