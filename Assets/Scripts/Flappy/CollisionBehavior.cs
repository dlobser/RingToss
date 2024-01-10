using UnityEngine;
using UnityEngine.Events;

namespace Quilt
{
    public class CollisionBehavior : MonoBehaviour
    {
        public CollisionBehaviorSettings settings;
        Vector3 colliderPosition;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            colliderPosition = collision.transform.position;
            HandleCollision(collision.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            colliderPosition = collision.transform.position;
            HandleCollision(collision.gameObject);
        }

        private void HandleCollision(GameObject collisionObject = null)
        {
            if (settings.endGame)
            {
                Globals.GetEventManager().OnEndGame();
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

