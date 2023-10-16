using UnityEngine;

public class Ring : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        Target target = other.gameObject.GetComponent<Target>();
        print("Hit");

        if (target != null)
        {
            CustomTag otherTag = target.customTag;

            switch (otherTag)
            {
                case CustomTag.Target:
                    // Handle a successful toss (e.g., scoring points)
                    GameManager.instance.IncrementScore();
                    print("Got it!");
                    target.Hit();
                    Destroy(gameObject); // Remove the ring
                    break;

                case CustomTag.Boundary:
                    // Destroy the ring when it hits a boundary
                    print("Out of bounds!");
                    Destroy(gameObject);
                    break;

                    // Add cases for other custom tags as needed
            }
        }
    }
}
