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

        void Start()
        {
           
        }

        private void OnDisable()
        {

        }

        public override void PlayEffectAtLocation(GameObject effect, Vector3 location,
        float intensity = 1f, float scale = 1f, float duration = 1f)
        {
            // Instantiate effect at location
            GameObject newEffect = Instantiate(effect, location, Quaternion.identity);
            // Set effect scale
            newEffect.transform.localScale = Vector3.one * scale;
            // Destroy effect after duration
            Destroy(newEffect, duration);
        }
    }
}