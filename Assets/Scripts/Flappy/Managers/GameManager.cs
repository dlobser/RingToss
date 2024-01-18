using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Quilt
{
    public class GameManager : Manager
    {
        public static GameManager Instance { get; private set; }

        public enum GameState { StartMenu, Start, Playing, Paused, GameOver }
        public GameState gameState = GameState.Start;
        public bool win = false;

        public EventManager eventManager;
        public MenuManager menuManager;
        public ScoreManager scoreManager;
        public InteractionManager interactionManager;
        public FXManager fxManager;
        public AudioManager audioManager;
        public UIManager uiManager;

        private void Awake()
        {
            //DL: this might be causing the issue with the game not restarting properly.

            // if (Instance == null)
            // {
                Instance = this;
            // }
            // else
            // {
            //     Debug.Log("GameManager already exists.");
            // }
        }

        public void Start()
        {
            scoreManager.RegisterActions();

            if (eventManager != null)
            {
                eventManager.OnStartGame += OnStartGame;
                eventManager.OnEndGame += OnEndGame;
            }
        }

        private void OnDestroy()
        {
            if (eventManager != null)
            {
                eventManager.OnStartGame -= OnStartGame;
                eventManager.OnEndGame -= OnEndGame;
            }
        }

        private void OnStartGame()
        {
            gameState = GameState.Playing;
            StartCoroutine(LevelStartDelay());
        }

        IEnumerator LevelStartDelay(){
            yield return null;
            eventManager.LevelStart();
        }

        private void OnEndGame()
        {
            Debug.Log("Game Over");
            //DL: Change this to a coroutine that waits for the level to end before building a new one.
            FindObjectOfType<GameGeneratorManager>().BuildGame();
        }

        public void StartGame()
        {
            gameState = GameState.Playing;
            OnStartGame();
        }

        public void Update()
        {
            if (gameState != GameState.Playing) return;
            CheckWinCondition();
        }

        private void CheckWinCondition()
        {
            if (scoreManager.totalScore >= 1000)
            {
                win = true;
                gameState = GameState.GameOver;
                Globals.GetEventManager().EndGame();
            }

        }

    }
}