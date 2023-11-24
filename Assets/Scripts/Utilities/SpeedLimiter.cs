using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SpeedLimiter : MonoBehaviour
{
    public float maxSpeed = 10f; // The maximum speed

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Check if the current speed exceeds the maximum speed
        if (rb.velocity.magnitude > maxSpeed)
        {
            // Clamp the velocity
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
}
