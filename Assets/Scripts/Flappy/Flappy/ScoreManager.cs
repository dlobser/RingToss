using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Quilt.Flappy
{
    public class ScoreManager : Quilt.ScoreManager
    {
        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void AddScore(int scoreToAdd)
        {
            totalScore += scoreToAdd;

        }

    }
}
