using UnityEngine;

public class Ring : MonoBehaviour
{

    bool emitted = false;
    public GameObject burster;
    public GameObject bursterTiny;
    public GameObject bounceEffect;
    public int destroyOnBounces = 1;
    int bounces = 0;
    public TrailRenderer trailRenderer;

    public float initScale = 1;//this.transform.localScale.x;
    float initLineTime = 1;//this.trailRenderer.time;

    int itemsCollectedAmount = 0;

    void OnEnable()
    {
        initScale = this.transform.localScale.x;
        initLineTime = this.trailRenderer.time;
    }
    private void OnTriggerEnter2D(Collider2D other){
        Item target = null;

        if (other.gameObject.GetComponent<Item>() != null)
            target = other.gameObject.GetComponent<Item>();

        if (target != null){
            if(target.customTag == CustomTag.ItemSpecial){
                if (GameManager.Instance.gameScoreKeeper != null){
                    GameManager.Instance.gameScoreKeeper.IncrementScore(target.scoreValue);
                }
                if (burster != null)
                {
                    GameObject burst = Instantiate(bursterTiny,GameManager.Instance.root.transform);
                    burst.transform.position = this.transform.position;
                    // Destroy(burst.gameObject, 5);
                }
                Destroy(target.gameObject);
                return;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        bounces++;

        // print("Collide! " + this.gameObject.name + " " + other.gameObject.name);

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

                    itemsCollectedAmount++;
                    // Handle a successful toss (e.g., scoring points)
                    if (GameManager.Instance.gameScoreKeeper != null){
                        GameManager.Instance.gameScoreKeeper.DecrementItem();
                        GameManager.Instance.gameScoreKeeper.IncrementScore(itemsCollectedAmount*itemsCollectedAmount*target.scoreValue);
                    }
                    if(itemsCollectedAmount==1)
                        GameManager.Instance.SendAnnouncement("YES!");
                    else if(itemsCollectedAmount==2)
                        GameManager.Instance.SendAnnouncement("DOUBLE!");
                    else if(itemsCollectedAmount==3)
                        GameManager.Instance.SendAnnouncement("TRIPLE!");
                    else if(itemsCollectedAmount>3)
                        GameManager.Instance.SendAnnouncement("SUPER!");

                    if (burster != null)
                    {
                        GameObject burst = Instantiate(burster,GameManager.Instance.root.transform);
                        burst.transform.position = this.transform.position;
                        // Destroy(burst.gameObject, 5);
                    }
                    print("Got it! " + itemsCollectedAmount);
                    target.Hit();

                    break;
                
                case CustomTag.BonusItem:
                    // Handle a successful toss (e.g., scoring points)
                    Destroy(gameObject);
                    Destroy(target.gameObject);

                    // Destroy(gameObject); // Remove the ring
                    // Destroy(target.gameObject);
                    print("Got Bonus!");
                    if (!emitted)
                        BurstObjects(4, GetComponent<Rigidbody2D>().velocity);

                    if (GameManager.Instance.gameScoreKeeper != null){
                        GameManager.Instance.gameScoreKeeper.IncrementScore(target.scoreValue);
                    }

                    GameManager.Instance.SendAnnouncement("BONUS!");

                    target.Hit();

                    break;

                case CustomTag.Boundary:
                    // Destroy the ring when it hits a boundary
                    print("Out of bounds!");
                    Destroy(gameObject);
                    break;

                case CustomTag.PlatformBouncy:
                    // instantiate a bounce effect
                    this.transform.localScale *= .98f;
                    if (bounceEffect != null)
                    {
                        GameObject bounce = Instantiate(bounceEffect,GameManager.Instance.root.transform);
                        bounce.transform.position = this.transform.position;
                    }
                    break;

                case CustomTag.Platform:
                    // instantiate a bounce effect
                    this.transform.localScale *= .98f;
                    if (bounceEffect != null)
                    {
                        GameObject bounce = Instantiate(bounceEffect);
                        bounce.transform.position = this.transform.position;
                    }
                    break;

                // Add cases for other custom tags as needed
                case CustomTag.Projectile:
                    // Handle a successful toss (e.g., scoring points)
                    // if (!target.GetComponent<Ring>().emitted)
                    //     BurstObjects(2, 8, GetComponent<Rigidbody2D>().velocity);

                    // Destroy(gameObject); // Remove the ring
                    // Destroy(target.gameObject);
                    break;

            }
        }
    }

    public void BurstObjects(int maxObjects, Vector2 velocity)
    {
        int numberOfObjects = maxObjects;// Random.Range(minObjects, maxObjects + 1);

        for (int i = 0; i < numberOfObjects; i++)
        {
            // Randomly position inside a circle
            Vector2 randomPosInCircle = Random.insideUnitCircle *.5f;
            Vector3 spawnPosition = new Vector3(randomPosInCircle.x, randomPosInCircle.y, 0) + transform.position; // Assuming you want the burst centered around the position of the GameObject this script is attached to.

            // Instantiate
            GameObject obj = Instantiate(this.gameObject, spawnPosition, Quaternion.identity, GameManager.Instance.root.transform);
            obj.transform.localScale = this.transform.localScale*.5f;
            obj.GetComponent<Ring>().initScale = this.transform.localScale.x*.5f;
            obj.GetComponent<Ring>().emitted = true;
            obj.GetComponent<Ring>().destroyOnBounces = 3;
            obj.GetComponent<CircleCollider2D>().enabled = true;


            // Apply outward force
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 directionOutward = (Random.insideUnitSphere + Vector3.up).normalized;
                rb.AddForce(directionOutward * 4 + new Vector2(velocity.x*2, velocity.y*2), ForceMode2D.Impulse);
            }
            else
            {
                Debug.LogError("The instantiated object does not have a Rigidbody2D component attached!");
            }
        }
    }


}
