using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quilt{
    public class ScoreManager : Manager
    {
        public int totalScore = 0;
        public float elapsedTime = 0f;

        void Update()
        {
            OnUpdate();
        }

        public virtual void OnUpdate(){
            elapsedTime += Time.deltaTime;
        }

        public virtual void AddScore(int scoreToAdd)
        {
            totalScore+=scoreToAdd;
        }

        public virtual void ItemCollected()
        {
        
        }

        public virtual void RegisterActions()
        {
            // Subscribe to the event
            // GameManager.Instance.IncrementScore  += OnIncrementScore;
            // Globals.GlobalSettings.Managers.gameManager.GameStart += OnLevelStart;
            Globals.GetEventManager().OnStartGame += OnLevelStart;
        }

        public virtual void OnLevelStart()
        {
            elapsedTime = 0;
            totalScore = 0;
        }

    }
}