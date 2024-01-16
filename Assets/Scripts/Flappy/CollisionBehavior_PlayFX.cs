using UnityEngine;
using UnityEngine.Events;

namespace Quilt
{
    public class CollisionBehavior_PlayFX : MonoBehaviour
    {
        public Vector3 colliderPosition {get;set;}

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
            
        }
    }
}

