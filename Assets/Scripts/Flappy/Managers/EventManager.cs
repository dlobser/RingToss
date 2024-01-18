using System;
using UnityEngine;

namespace Quilt
{
    public class EventManager : Manager
    {
        public event Action OnLevelStart;
        public event Action OnLevelEnd;
        public event Action OnStartMenu;
        public event Action OnStartGame;
        public event Action OnEndGame;

        public void LevelStart()
        {
            OnLevelStart?.Invoke();
        }

        public void LevelEnd()
        {
            OnLevelEnd?.Invoke();
        }

        public void StartMenu()
        {
            OnStartMenu?.Invoke();
        }

        public void StartGame()
        {
            OnStartGame?.Invoke();
        }

        public void EndGame()
        {
            Debug.Log("End Game");
            OnEndGame?.Invoke();
        }
    }
}