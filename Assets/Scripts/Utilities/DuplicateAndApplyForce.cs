using UnityEngine;

public class DuplicateAndApplyForce : MonoBehaviour
{
    public float forceMagnitude = 5f; // Adjust the force magnitude as needed
    public float cooldownDuration = 5f; // Time in seconds before the script can duplicate again
    public SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    private float cooldownTimer = 5f;
    Color init;

    void Start(){
        init = spriteRenderer.color;
    }

    void Update()
    {
       
        // Decrease the cooldown timer over time
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
        if(spriteRenderer!=null)
            spriteRenderer.color = Color.Lerp(init, Color.black,  ((cooldownTimer / cooldownDuration)));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the cooldown has elapsed and the object has a Rigidbody2D
        if (cooldownTimer <= 0 && other.GetComponent<Rigidbody2D>() != null)
        {
            // Reset the cooldown timer
            cooldownTimer = cooldownDuration;

            // Duplicate and apply force
            DuplicateAndApplyForceTo(other.gameObject);
        }
    }

    void DuplicateAndApplyForceTo(GameObject obj)
    {
        Rigidbody2D originalRb = obj.GetComponent<Rigidbody2D>();
        if (originalRb != null)
        {
            // Create duplicates and apply force
            CreateDuplicateWithForce(originalRb);
            //ApplyRandomForce(originalRb);
        }
    }

    void CreateDuplicateWithForce(Rigidbody2D originalRb)
    {
        GameObject duplicate = Instantiate(originalRb.gameObject, originalRb.transform.position, originalRb.transform.rotation);
        ApplyRandomForce(duplicate.GetComponent<Rigidbody2D>());
    }

    void ApplyRandomForce(Rigidbody2D rb)
    {
        Vector2 randomForce = Random.insideUnitCircle.normalized * forceMagnitude;
        rb.AddForce(randomForce, ForceMode2D.Impulse);
    }
}
