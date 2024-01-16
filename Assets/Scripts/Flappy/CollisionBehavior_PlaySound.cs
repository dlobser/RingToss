using UnityEngine;
using UnityEngine.Events;

namespace Quilt
{
    public class CollisionBehavior_PlaySound : MonoBehaviour
    {
        public Vector3 colliderPosition {get;set;}
        public AudioClip collisionSound;
        public Vector2 audioVolumeRange = new Vector2(0.5f,1f);
        public float audioVelocityMultiplier = 0.5f;

        public virtual void OnCollisionEnter2D(Collision2D collision)
        {
            colliderPosition = collision.transform.position;
            HandleCollision(collision.gameObject);
        }

        public virtual void OnTriggerEnter2D(Collider2D collision)
        {
            colliderPosition = collision.transform.position;
            HandleCollision(collision.gameObject);
        }

        public virtual void HandleCollision(GameObject collisionObject = null)
        {
            float velocity = 0;
            if(collisionObject.GetComponent<Rigidbody2D>() != null)
                velocity = collisionObject.GetComponent<Rigidbody2D>().velocity.magnitude/10;

            Globals.GetAudioManager().PlayOneShotAtLocation(
                collisionSound,
                colliderPosition,
                Random.Range(audioVolumeRange.x, audioVolumeRange.y) + 
                Mathf.LerpUnclamped(0f,velocity,audioVelocityMultiplier));
        }
    }
}

