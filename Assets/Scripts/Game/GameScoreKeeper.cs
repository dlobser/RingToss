using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class GameScoreKeeper : MonoBehaviour
{

    public int totalScore;
    public float elapsedTime;
    public int items;
    public int totalItemsInLevel;
    public bool win;
    public GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update(){
        elapsedTime += Time.deltaTime;
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
        totalScore = 0;
        // Find all GameObjects with the 'Target' tag
        Item[] targets = GameManager.Instance.root.GetComponentsInChildren<Item>();// FindObjectsOfType<Target>();
        foreach (Item t in targets)
        {
            if (t.customTag == CustomTag.Item)
                totalItemsInLevel++;
        }
        items = totalItemsInLevel;
        print("Start Items: " + items);
    }

    public virtual void IncrementProjectile()
    {

    }

    public virtual void DecrementItem()
    {
        items--;
        
    }

    public virtual void IncrementScore(int amount = 1)
    {
        totalScore+=amount;
        CheckLevelFinished();
    }

    // void Update(){
    //      print("Items: " + items);
    // }
    
    public virtual void CheckLevelFinished()
    {
        print("Items: " + items);
        if (items <= 0)
        {
            GameManager.Instance.GameOver();
        }
    }
}
