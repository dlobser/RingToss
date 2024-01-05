using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quilt;

namespace Quilt.Flappy
{
    public class FXManager : Quilt.FXManager
    {
        // Add references to particle systems or other effects
        public ParticleSystem itemCollectedEffect;
        public ParticleSystem wallHitEffect;

        private void OnEnable()
        {
            Globals.GlobalSettings.Managers.eventManager.ItemCollected += PlayItemCollectedEffect;
            Globals.GlobalSettings.Managers.eventManager.Collision += PlayCollisionEffect;
        }

        private void OnDisable()
        {
            Globals.GlobalSettings.Managers.eventManager.ItemCollected -= PlayItemCollectedEffect;
            Globals.GlobalSettings.Managers.eventManager.Collision -= PlayCollisionEffect;
        }

        private void PlayItemCollectedEffect()
        {
            // Play item collected effect
            itemCollectedEffect.Play();
        }

        private void PlayCollisionEffect(CollisionEventArgs e)
        {
            // Play wall hit effect
            wallHitEffect.Play();
        }
    }
}