using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class PlayerControllerRedirect : MonoBehaviour
{
    public GameObject ringPrefab;
    public Transform ringSpawnPoint;
    public GameObject onClickDisplayObject;
    public float maxTossForce = 10.0f;
    public float tossForceMultiply = 10;

    private GameObject currentRing;
    public GameObject displayRing;
    private Vector3 dragEndPos;
    Vector3 dragDirection;
    float dragDistance;

    ImageLoader imageLoader;

    bool canToss = false;
    bool bounce = false;

    // public GameObject platformBounce;

    void Start()
    {
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
        Ring[] rings = FindObjectsOfType<Ring>();
        if (rings.Length > 0)
        {
            bounce = true;
        }
        else
        {
            // platformBounce.SetActive(false);
        }

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
        if (bounce)
        {
            // Convert the mouse position from screen space to world space
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));

            // Now set the position of platformBounce
            // We take the x from the mouse, keep the y position of the platform, and set z to 0 (if you are working in 2D)
            // platformBounce.transform.position = new Vector3(mouseWorldPosition.x, platformBounce.transform.position.y, 0);
        }
        else if (displayRing != null && canToss)
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
            displayRing.transform.position = ringSpawnPoint.position;

        }
    }

    private void OnMouseUp()
    {
        if (bounce)
        {
            bounce = false;
            Vector2 mousePos = Input.mousePosition;
            dragEndPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));
            dragEndPos.z = 0;
            
            Ring[] rings = FindObjectsOfType<Ring>();

            Instantiate(onClickDisplayObject,dragEndPos,Quaternion.identity,GameManager.Instance.root.transform);

            foreach(Ring r in rings){
                Rigidbody2D rb = r.GetComponent<Rigidbody2D>();
                dragDirection = (r.transform.position - dragEndPos).normalized;
                dragDistance = Vector3.Distance(r.transform.position, dragEndPos);
                rb.AddForce(dragDirection * Mathf.Clamp(tossForceMultiply/dragDistance, 0, maxTossForce), ForceMode2D.Impulse);
                rb.AddTorque(-100,ForceMode2D.Impulse);
            }

            // platformBounce.SetActive(false);
        }
        else if (canToss)
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
                // imageLoader.AssignRandomImage(currentRing.transform.GetChild(0).GetComponent<SpriteRenderer>(), "Projectile");
                // ImageLoader.Instance.SetSprite(currentRing.transform.GetChild(0).GetComponent<SpriteRenderer>(), "Projectile", GlobalSettings.ImageIndeces.Projectile);
            }

            Rigidbody2D rb = currentRing.GetComponent<Rigidbody2D>();

            // Apply the force to the currentRing when releasing the mouse button
            rb.AddForce(dragDirection * Mathf.Clamp(dragDistance * tossForceMultiply, 0, maxTossForce), ForceMode2D.Impulse);
            rb.gravityScale = GlobalSettings.Physics.ballGravity;

            canToss = false;
        }
    }
}
