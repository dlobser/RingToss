using UnityEngine;
using UnityEngine.Events;

namespace Quilt
{
    public class CollisionBehavior_EndGame : CollisionBehavior
    {
        public override void HandleCollision(GameObject collisionObject = null)
        {
            Debug.Log("End Game Collision");
            GameManager.Instance.eventManager.EndGame();
        }
    }
}

