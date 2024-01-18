using UnityEngine;
using UnityEngine.Events;

namespace Quilt
{
    public class CollisionBehavior_EndGame : CollisionBehavior
    {
        public override void HandleCollision(GameObject collisionObject = null)
        {
            Debug.Log("CollisionBehavior_EndGame " + collisionObject.name + " " + this.name);
            GameManager.Instance.eventManager.EndGame();
        }
    }
}

