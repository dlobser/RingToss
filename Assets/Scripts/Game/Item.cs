using UnityEngine;

public class Item : MonoBehaviour
{
    public CustomTag customTag = CustomTag.Item;
    public int scoreValue = 10; 
    public bool destroyOnHit;
    public AudioClip collisionSound; // Audio clip to be played on collision
    public AudioSource audioSource; // Public AudioSource to be set in the Unity Editor

    private void Start()
    {
        // Ensure the AudioSource is set
        if (audioSource == null)
        {
            // Debug.LogError("AudioSource is not assigned on " + gameObject.name);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Play the collision sound
        if (audioSource != null && collisionSound != null)
        {
            audioSource.PlayOneShot(collisionSound);
        }

        // Call the Hit method
        Hit();
    }

    public virtual void Hit()
    {
        if(destroyOnHit)
            Destroy(this.gameObject);
    }
}
