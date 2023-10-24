using UnityEngine;

public class PlayerController : MonoBehaviour
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

    bool canToss = false;

    void Start(){
        imageLoader = FindObjectOfType<ImageLoader>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDown();
        }
        else if (Input.GetMouseButton(0))
        {
            OnMouseDrag();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnMouseUp();
        }
    }



    private void OnMouseDown()
    {
        Vector2 mousePos = Input.mousePosition;
        dragEndPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));
        dragDistance = Vector3.Distance(ringSpawnPoint.position, dragEndPos);

        if (dragDistance < 1)
        {
            canToss = true;
        }
    }

    private void OnMouseDrag()
    {
        if (displayRing != null && canToss)
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
            displayRing.transform.localScale = new Vector3(1, 1, dragDistance);
            displayRing.transform.position = ringSpawnPoint.position ;

        }
    }

    private void OnMouseUp()
    {
        if (canToss)
        {
            if (displayRing != null)
            {
                displayRing.transform.position = new Vector3(0, 0, 1000);
                //Destroy(displayRing); // Remove the display ring
            }

            // Instantiate the currentRing for the actual toss
            currentRing = Instantiate(ringPrefab, ringSpawnPoint.position, Quaternion.identity);
            if(currentRing.transform.GetChild(0).GetComponent<SpriteRenderer>()!=null){
                imageLoader.AssignRandomImage(currentRing.transform.GetChild(0).GetComponent<SpriteRenderer>(),"Projectile");
            }
            Rigidbody2D rb = currentRing.GetComponent<Rigidbody2D>();

            // Apply the force to the currentRing when releasing the mouse button
            rb.AddForce(dragDirection * Mathf.Clamp(dragDistance * tossForceMultiply, 0, maxTossForce), ForceMode2D.Impulse);

            canToss = false;
        }
    }
}
