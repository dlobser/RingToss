using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Quilt
{
    public class GameManager : MonoBehaviour
    {
        public enum GameState { StartMenu, Start, Playing, Paused, GameOver }
        public GameState gameState = GameState.Start;
        private ScoreManager scoreManager;
        public bool win = false;

        public void Start()
        {
            scoreManager = Globals.GetScoreManager();
            scoreManager.RegisterActions();

            if (Globals.GetEventManager() != null)
            {
                Globals.GetEventManager().StartGame += OnGameStart;
                Globals.GetEventManager().EndGame += OnGameEnd;
            }
        }

        private void OnDestroy()
        {
            if (Globals.GetEventManager() != null)
            {
                Globals.GetEventManager().StartGame -= OnGameStart;
                Globals.GetEventManager().EndGame -= OnGameEnd;
            }
        }

        private void OnGameStart()
        {
            // StartGame();
            gameState = GameState.Playing;
        }

        private void OnGameEnd()
        {
            FindObjectOfType<GameGeneratorManager>().BuildGame();
        }

        public void StartGame()
        {
            gameState = GameState.Playing;
        }

        public void Update()
        {
            if (gameState != GameState.Playing) return;

            CheckWinCondition();
        }

        private void CheckWinCondition()
        {
            if (scoreManager.totalScore >= 10)
            {
                win = true;
                gameState = GameState.GameOver;
                Globals.GetEventManager().OnEndGame();
            }

        }

    }
}