using UnityEngine;
using UnityEngine.Events;

namespace Quilt
{
    public class CollisionBehaviorMultiPurpose : CollisionBehavior
    {
        public CollisionBehaviorSettings settings;

        public override void HandleCollision(GameObject collisionObject = null)
        {
            if(!settings.onlyCollideWithPlayer || collisionObject.GetComponent<Player>() != null && Time.timeSinceLevelLoad>0)
            {
           
                if (settings.endGame)
                {
                    Globals.GetEventManager().EndGame();
                }

                if (settings.playSound && settings.collisionSound != null)
                {
                    float velocity = 0;
                    if(collisionObject.GetComponent<Rigidbody2D>() != null)
                        velocity = collisionObject.GetComponent<Rigidbody2D>().velocity.magnitude/10;

                    Globals.GetAudioManager().PlayOneShotAtLocation(
                        settings.collisionSound,
                        colliderPosition,
                        Random.Range(settings.audioVolumeRange.x, settings.audioVolumeRange.y) + 
                        Mathf.LerpUnclamped(0f,velocity,settings.audioVelocityMultiplier));
                }

                if (settings.playEffect && settings.effectPrefab != null)
                {
                    Globals.GetFXManager().PlayEffectAtLocation(
                        settings.effectPrefab,
                        colliderPosition,
                        Random.Range(settings.effectIntensityRange.x, settings.effectIntensityRange.y),
                        Random.Range(settings.effectScaleRange.x, settings.effectScaleRange.y),
                        Random.Range(settings.effectDurationRange.x, settings.effectDurationRange.y));
                }

                if (settings.addToScore)
                {
                    Globals.GetScoreManager().AddScore(settings.scoreValue);
                }

                settings.onTriggerEnterAction?.Invoke();

                if (settings.destroyOnHit)
                {
                    Destroy(gameObject);
                }

                if (settings.destroyColliderOnHit)
                {
                    Destroy(collisionObject);
                }

                if(settings.destroyObjectsOnhit){
                    foreach(GameObject obj in settings.objectsToDestroyOnHit){
                        Destroy(obj);
                    }
                }
            }
        }
    }
}

