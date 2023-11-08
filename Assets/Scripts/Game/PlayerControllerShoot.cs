using UnityEngine;

public class PlayerControllerShoot : MonoBehaviour
{
    public GameObject ringPrefab;
    public Transform ringSpawnPoint;
    public float maxTossForce = 10.0f;
    public float tossForceMultiply = 10;

    private GameObject currentRing;
    public GameObject displayRing;
    private Vector3 dragEndPos;
    Vector3 dragDirection;
    float dragDistance;

    ImageLoader imageLoader;

    public float frequency;
    float counter = 1;

    bool canToss = false;

    void Start()
    {
        imageLoader = FindObjectOfType<ImageLoader>();
        if (displayRing != null)
        {
            displayRing.transform.position = new Vector3(0, 0, 1000);
            //Destroy(displayRing); // Remove the display ring
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDown();
        }
        if (Input.GetMouseButton(0))
        {
            OnMouseDrag();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnMouseUp();
        }

        counter+=Time.deltaTime;
        if(counter>frequency){
            counter = 0;
            canToss = true;
        }
        else
            canToss = false;
        
    }

    private void OnMouseDown()
    {
        // Vector2 mousePos = Input.mousePosition;
        // dragEndPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));
        // dragDistance = Vector3.Distance(ringSpawnPoint.position, dragEndPos);

        // if (dragDistance < 1)
        // {
        //     canToss = true;
        // }
        counter = 1;
    }

    private void OnMouseDrag()
    {
        if (displayRing != null)
        {
            Vector2 mousePos = Input.mousePosition;
            dragEndPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));

            dragEndPos.z = 0;
            dragDirection = (dragEndPos - ringSpawnPoint.position).normalized;
            dragDistance = Vector3.Distance(ringSpawnPoint.position, dragEndPos);

            // Limit the force to the maximum specified
            float tossForce = Mathf.Clamp(dragDistance, 0, maxTossForce);

            // Visualize the force direction by scaling the display ring
            displayRing.transform.LookAt(dragEndPos);
            displayRing.transform.localScale = new Vector3(1, 1, Mathf.Min(dragDistance,maxTossForce*.1f));
            displayRing.transform.position = ringSpawnPoint.position;
        }


        if ( canToss)
        {

          

            tossForceMultiply = GlobalSettings.Physics.ballSpeed;

            if (displayRing != null)
            {
                displayRing.transform.position = new Vector3(0, 0, 1000);
                //Destroy(displayRing); // Remove the display ring
            }

            // Instantiate the currentRing for the actual toss
            currentRing = Instantiate(ringPrefab, ringSpawnPoint.position, Quaternion.identity);
            currentRing.transform.localScale = Vector3.one * GlobalSettings.Physics.ballSize;

            if (currentRing.transform.GetChild(0).GetComponent<SpriteRenderer>() != null)
            {
                ImageLoader.Instance.SetSprite(currentRing.transform.GetChild(0).GetComponent<SpriteRenderer>(), "Projectile", GlobalSettings.ImageIndeces.Projectile);
            }
            Rigidbody2D rb = currentRing.GetComponent<Rigidbody2D>();

            // Apply the force to the currentRing when releasing the mouse button
            rb.AddForce(dragDirection * Mathf.Clamp(dragDistance * tossForceMultiply, 0, maxTossForce), ForceMode2D.Impulse);
            rb.gravityScale = GlobalSettings.Physics.ballGravity;

            // canToss = false;


        }
    }

    private void OnMouseUp()
    {
        // if (canToss)
        // {

            // tossForceMultiply = GlobalSettings.Physics.ballSpeed;

            if (displayRing != null)
            {
                displayRing.transform.position = new Vector3(0, 0, 1000);
                //Destroy(displayRing); // Remove the display ring
            }

            // // Instantiate the currentRing for the actual toss
            // currentRing = Instantiate(ringPrefab, ringSpawnPoint.position, Quaternion.identity);
            // currentRing.transform.localScale = Vector3.one * GlobalSettings.Physics.ballSize;

            // if (currentRing.transform.GetChild(0).GetComponent<SpriteRenderer>() != null)
            // {
            //     imageLoader.AssignRandomImage(currentRing.transform.GetChild(0).GetComponent<SpriteRenderer>(), "Projectile");
            // }
            // Rigidbody2D rb = currentRing.GetComponent<Rigidbody2D>();

            // // Apply the force to the currentRing when releasing the mouse button
            // rb.AddForce(dragDirection * Mathf.Clamp(dragDistance * tossForceMultiply, 0, maxTossForce), ForceMode2D.Impulse);
            // rb.gravityScale = GlobalSettings.Physics.ballGravity;

            // canToss = false;
        // }
    }
}
