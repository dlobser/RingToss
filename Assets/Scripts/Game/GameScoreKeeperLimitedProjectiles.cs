using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using TMPro;

public class GameScoreKeeperLimitedProjectiles : GameScoreKeeper
{

    public int totalProjectiles;
    public int usedProjectiles;

    public TextMeshProUGUI scoreUI;
    public TextMeshProUGUI projectilesUI;

    private const string HighScoreKey = "HighScore"; // Key to store the high score in PlayerPrefs


    public override void RegisterActions()
    {
        gameManager.GameStart += OnLevelStart;
    }

    public override void DeregisterActions()
    {
        gameManager.GameStart -= OnLevelStart;
    }

    public override void OnLevelStart()
    {
        base.OnLevelStart();
        usedProjectiles = 0;
        totalScore = totalProjectiles;
        if (projectilesUI != null)
            projectilesUI.text = usedProjectiles + " / " + totalProjectiles;
        if (scoreUI != null)
            scoreUI.text = totalScore + "";

    }

    public override void IncrementScore(int amount)
    {
        base.IncrementScore(amount);
        SaveHighScore();
    }

    public override void IncrementProjectile()
    {
        usedProjectiles++;
        totalScore--;
        if (projectilesUI != null)
            projectilesUI.text = usedProjectiles + " / " + totalProjectiles;
        CheckLevelFinished();
    }

    public override void CheckLevelFinished()
    {
        if (scoreUI != null)
            scoreUI.text = items + " / " + totalItemsInLevel;
        base.CheckLevelFinished();
        if(usedProjectiles>=totalProjectiles-1){
            totalScore+=totalProjectiles-usedProjectiles;
            GameManager.Instance.GameOver();
        }
        
    }

    public void SaveHighScore()
    {
        int currentHighScore = PlayerPrefs.GetInt(HighScoreKey+GlobalSettings.randomSeed, 0);
        if (totalScore > currentHighScore)
        {
            PlayerPrefs.SetInt(HighScoreKey+GlobalSettings.randomSeed, totalScore);
            PlayerPrefs.Save();
        }
    }

    public void LoadHighScore()
    {
        // Optionally, you can load the high score at the start to display it or use it
        int highScore = PlayerPrefs.GetInt(HighScoreKey+GlobalSettings.randomSeed, 0);
        // Do something with highScore if needed
    }
}
