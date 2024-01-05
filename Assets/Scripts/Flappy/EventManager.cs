using System;
using UnityEngine;

namespace Quilt{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }

        public event Action ItemCollected;
        public event Action<CollisionEventArgs> Collision;
        public event Action StartMenu;
        public event Action StartGame;
        public event Action EndGame;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void OnItemCollected()
        {
            ItemCollected?.Invoke();
        }

        public void OnCollision(CollisionEventArgs e)
        {
            Collision?.Invoke(e);
        }

        public void OnStartMenu()
        {
            StartMenu?.Invoke();
        }

        public void OnStartGame()
        {
            StartGame?.Invoke();
        }

        public void OnEndGame()
        {
            EndGame?.Invoke();
        }
    }
}