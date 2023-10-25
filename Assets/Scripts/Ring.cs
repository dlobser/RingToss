using UnityEngine;

public class Ring : MonoBehaviour
{

    bool emitted = false;
    public GameObject burster;

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
                    if(burster!=null){
                        GameObject burst = Instantiate(burster);
                        burst.transform.position = this.transform.position;
                        Destroy(burst.gameObject,5);
                    }
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
                case CustomTag.Projectile:
                    // Handle a successful toss (e.g., scoring points)
                    if(!target.GetComponent<Ring>().emitted)
                        BurstObjects(2,8);
                    Destroy(gameObject); // Remove the ring
                    Destroy(target.gameObject);
                    break;

            }
        }
    }

    public void BurstObjects(int minObjects, int maxObjects)
    {
        int numberOfObjects = Random.Range(minObjects, maxObjects + 1);

        for (int i = 0; i < numberOfObjects; i++)
        {
            // Randomly position inside a circle
            Vector2 randomPosInCircle = Random.insideUnitCircle;
            Vector3 spawnPosition = new Vector3(randomPosInCircle.x, randomPosInCircle.y, 0) + transform.position; // Assuming you want the burst centered around the position of the GameObject this script is attached to.

            // Instantiate
            GameObject obj = Instantiate(this.gameObject, spawnPosition, Quaternion.identity);
            obj.transform.localScale = Vector3.one*.2f;
            obj.GetComponent<Ring>().emitted = true;
            obj.GetComponent<CircleCollider2D>().enabled = true;
        

            // Apply outward force
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null) 
            {
                Vector2 directionOutward = (obj.transform.position - transform.position).normalized;
                rb.AddForce(directionOutward * 2, ForceMode2D.Impulse);
            }
            else
            {
                Debug.LogError("The instantiated object does not have a Rigidbody2D component attached!");
            }
        }
    }


}
