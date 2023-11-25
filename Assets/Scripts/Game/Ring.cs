using UnityEngine;

public class Ring : MonoBehaviour
{

    bool emitted = false;
    public GameObject burster;
    public GameObject bounceEffect;
    public int destroyOnBounces = 1;
    int bounces = 0;
    public TrailRenderer trailRenderer;

    float initScale = 1;//this.transform.localScale.x;
    float initLineTime = 1;//this.trailRenderer.time;

    void OnEnable()
    {
        initScale = this.transform.localScale.x;
        initLineTime = this.trailRenderer.time;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        bounces++;

        this.transform.localScale = Mathf.Lerp(initScale, 0, (float)bounces / (float)destroyOnBounces) * Vector3.one;
        trailRenderer.time = Mathf.Lerp(initLineTime, 0, (float)bounces / (float)destroyOnBounces);

        if (bounces > destroyOnBounces)
            Destroy(gameObject);

        Item target = null;

        if (other.gameObject.GetComponent<Item>() != null)
            target = other.gameObject.GetComponent<Item>();

        if (target != null)
        {
            CustomTag otherTag = target.customTag;

            switch (otherTag)
            {
                case CustomTag.Item:
                    // Handle a successful toss (e.g., scoring points)
                    if (GameManager.Instance.gameScoreKeeper != null)
                        GameManager.Instance.gameScoreKeeper.IncrementScore();
                    if (burster != null)
                    {
                        GameObject burst = Instantiate(burster);
                        burst.transform.position = this.transform.position;
                        // Destroy(burst.gameObject, 5);
                    }
                    print("Got it!");
                    target.Hit();

                    break;

                case CustomTag.Boundary:
                    // Destroy the ring when it hits a boundary
                    print("Out of bounds!");
                    Destroy(gameObject);
                    break;

                case CustomTag.PlatformBouncy:
                    // instantiate a bounce effect
                    this.transform.localScale *= .9f;
                    if (bounceEffect != null)
                    {
                        GameObject bounce = Instantiate(bounceEffect);
                        bounce.transform.position = this.transform.position;
                    }
                    break;

                case CustomTag.Platform:
                    // instantiate a bounce effect
                    this.transform.localScale *= .9f;
                    if (bounceEffect != null)
                    {
                        GameObject bounce = Instantiate(bounceEffect);
                        bounce.transform.position = this.transform.position;
                    }
                    break;

                // Add cases for other custom tags as needed
                case CustomTag.Projectile:
                    // Handle a successful toss (e.g., scoring points)
                    if (!target.GetComponent<Ring>().emitted)
                        BurstObjects(2, 8, GetComponent<Rigidbody2D>().velocity);
                    Destroy(gameObject); // Remove the ring
                    Destroy(target.gameObject);
                    break;

            }
        }
    }

    public void BurstObjects(int minObjects, int maxObjects, Vector2 velocity)
    {
        int numberOfObjects = Random.Range(minObjects, maxObjects + 1);

        for (int i = 0; i < numberOfObjects; i++)
        {
            // Randomly position inside a circle
            Vector2 randomPosInCircle = Random.insideUnitCircle;
            Vector3 spawnPosition = new Vector3(randomPosInCircle.x, randomPosInCircle.y, 0) + transform.position; // Assuming you want the burst centered around the position of the GameObject this script is attached to.

            // Instantiate
            GameObject obj = Instantiate(this.gameObject, spawnPosition, Quaternion.identity);
            obj.transform.localScale = Vector3.one * .2f;
            obj.GetComponent<Ring>().emitted = true;
            obj.GetComponent<CircleCollider2D>().enabled = true;


            // Apply outward force
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 directionOutward = (obj.transform.position - transform.position).normalized;
                rb.AddForce(directionOutward * 2 + new Vector2(velocity.x, velocity.y), ForceMode2D.Impulse);
            }
            else
            {
                Debug.LogError("The instantiated object does not have a Rigidbody2D component attached!");
            }
        }
    }


}
