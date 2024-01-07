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
        public event Action GameStart;
        public event Action GameOver;
        public bool win = false;

        public void Start()
        {
            scoreManager = Globals.GlobalSettings.Managers.scoreManager;
            scoreManager.RegisterActions();

            if (EventManager.Instance != null)
            {
                EventManager.Instance.StartGame += OnGameStart;
                EventManager.Instance.EndGame += OnGameEnd;
            }
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.StartGame -= OnGameStart;
                EventManager.Instance.EndGame -= OnGameEnd;
            }
        }

        private void OnGameStart()
        {
            StartGame();
        }

        private void OnGameEnd()
        {
            EndGame();
        }

        public void StartGame()
        {
            gameState = GameState.Playing;
            GameStart?.Invoke();
        }

        public void Update()
        {
            if (gameState != GameState.Playing) return;

            CheckWinCondition();
        }

        private void CheckWinCondition()
        {
            // Example win condition: All items collected
            if (scoreManager.totalScore >= 10)
            {
                win = true;
                gameState = GameState.GameOver;
                EndGame();
            }

            // Additional win/lose conditions can be checked here
        }

        private void EndGame()
        {
            GameOver?.Invoke();
            FindObjectOfType<GameGeneratorManager>().StartGame();
            // Additional logic for when the game is won
        }

    }
}