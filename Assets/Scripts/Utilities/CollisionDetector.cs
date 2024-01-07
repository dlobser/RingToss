using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // This object's name
        string thisObjectName = gameObject.name;

        // The object it collided with
        string collidedObjectName = collision.gameObject.name;

        // Print out the information
        Debug.Log($"Collision detected between {thisObjectName} and {collidedObjectName}");
    }
}
