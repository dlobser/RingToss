using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public GameObject ringPrefab;
    public Transform ringSpawnPoint;
    public float maxTossForce = 10.0f;
    public float tossForceMultiply = 10;

    GameScoreKeeperLimitedProjectiles scoreKeeper;
    public GameObject projectileParent;
    public GameObject projectileDisplay;

    private GameObject currentRing;
    public GameObject displayRing;
    private Vector3 dragEndPos;
    Vector3 dragDirection;
    float dragDistance;

    public SpriteRenderer emitterSprite;

    public GameObject indicatorSprite;
    GameObject indicator;

    ImageLoader imageLoader;

    bool canToss = false;

    public Transform anchorSprite;

    bool gameOver = false;

    void OnEnable()
    {
       GameManager.Instance.GameEnd  += OnGameEnd;
    }

    void OnDisable()
    {
       GameManager.Instance.GameEnd  -= OnGameEnd;
    }

    void OnGameEnd(){
        gameOver = true;
        projectileParent.SetActive(false);
        DisableAllOfType<Ring>();
    }

    private void DisableAllOfType<T>() where T : MonoBehaviour
    {
        T[] objects = FindObjectsOfType<T>(true); // true to include inactive objects
        foreach (T obj in objects)
        {
            obj.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        imageLoader = FindObjectOfType<ImageLoader>();
        if(FindObjectOfType<GameScoreKeeperLimitedProjectiles>()!=null)
            scoreKeeper = FindObjectOfType<GameScoreKeeperLimitedProjectiles>();
        projectileParent.transform.parent = GameManager.Instance.root.transform;
        print(emitterSprite);
        print(ImageLoader.Instance);
        print(GlobalSettings.ImageIndeces.Emitter);
        ImageLoader.Instance.SetSprite(emitterSprite, "Emitter", GlobalSettings.ImageIndeces.Emitter);
        GameManager.Instance.playerController = this;
    }

    private void Update()
    {
        if(!gameOver){
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
            if(scoreKeeper!=null)
                if(scoreKeeper.totalProjectiles-scoreKeeper.usedProjectiles != projectileParent.transform.childCount)
                    UpdateProjectileDisplay();
            dragEndPos = Vector3.Lerp(dragEndPos,Vector3.up,Time.deltaTime);
            anchorSprite.LookAt(dragEndPos,Vector3.forward);
        }

    }

    // Assuming projectileParent is a Transform and projectileDisplay is a GameObject prefab
    // void UpdateProjectileDisplay()
    // {
    //     int count = projectileParent.transform.childCount;

    //     for (int i = 0; i < count; i++)
    //     {
    //         Destroy(projectileParent.transform.GetChild(i).gameObject);
    //     }

    //     // Instantiate new objects based on the game score
    //     for (int i = 0; i < GameManager.Instance.gameScoreKeeper.usedProjectiles; i++)
    //     {
    //         GameObject g = Instantiate(projectileDisplay, projectileParent.transform.position, Quaternion.identity, projectileParent.transform);
    //         g.transform.localPosition = new Vector3((float)i*.3f + 1,0,0);
    //     }
    // }
    void UpdateProjectileDisplay()
    {
        // Destroy existing children
        while (projectileParent.transform.childCount > 0)
        {
            DestroyImmediate(projectileParent.transform.GetChild(0).gameObject);
        }

        // Instantiate new objects based on the used projectiles
        int itemsPerRow = 5;
        float spacing = 0.3f; // Adjust spacing as needed
        for (int i = 0; i < (scoreKeeper.totalProjectiles - scoreKeeper.usedProjectiles); i++)
        {
            int row = i / itemsPerRow;
            int col = i % itemsPerRow;

            // Calculate position for each new object
            Vector3 position = new Vector3(col * spacing, -row * spacing, 0) + projectileParent.transform.position;

            GameObject g = Instantiate(projectileDisplay, position, Quaternion.identity, projectileParent.transform);
            projectileDisplay.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = ImageLoader.Instance.GetSpriteWithIndex("Projectile", GlobalSettings.ImageIndeces.Projectile);
            g.transform.localPosition = position - projectileParent.transform.position; // Adjust local position relative to the parent
            g.transform.localScale = Vector3.one * 1f/(float)itemsPerRow; // Adjust scale as needed
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
            indicator = Instantiate(indicatorSprite, dragEndPos, Quaternion.identity, GameManager.Instance.root.transform);;
        }
    }

    private void OnMouseDrag()
    {
        if (displayRing != null && canToss)
        {

            Vector2 mousePos = Input.mousePosition;
           dragEndPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));

            // Quantize the world position to the nearest 10 unit grid
            dragEndPos.x = QuantizeValue(dragEndPos.x, .1f);
            dragEndPos.y = QuantizeValue(dragEndPos.y, .1f);

            indicator.transform.position = dragEndPos;
            // anchorSprite.LookAt(dragEndPos);
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

    private float QuantizeValue(float value, float gridUnit)
    {
        return Mathf.Round(value / gridUnit) * gridUnit;
    }


    private void OnMouseUp()
    {
        if (canToss)
        {
            Vector2 mousePos = Input.mousePosition;
            dragEndPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));
            
            if(Vector2.Distance(dragEndPos,ringSpawnPoint.position ) < 1 || dragEndPos.y<ringSpawnPoint.position.y){
                canToss = false;
                Destroy(indicator);
                return;
            }
            tossForceMultiply = GlobalSettings.Physics.ballSpeed;

            if (displayRing != null)
            {
                displayRing.transform.position = new Vector3(0, 0, 1000);
                //Destroy(displayRing); // Remove the display ring
            }

            // Instantiate the currentRing for the actual toss
            currentRing = Instantiate(ringPrefab, ringSpawnPoint.position, Quaternion.identity, GameManager.Instance.root.transform);
            currentRing.transform.localScale = Vector3.one * GlobalSettings.Physics.ballSize;
            // currentRing.transform.parent = GameManager.Instance.rootParent.transform.GetChild(0);

            if (currentRing.transform.GetChild(0).GetComponent<SpriteRenderer>() != null)
            {
                // imageLoader.AssignRandomImage(currentRing.transform.GetChild(0).GetComponent<SpriteRenderer>(), "Projectile");
                ImageLoader.Instance.SetSprite(currentRing.transform.GetChild(0).GetComponent<SpriteRenderer>(), "Projectile", GlobalSettings.ImageIndeces.Projectile);
            }

            Rigidbody2D rb = currentRing.GetComponent<Rigidbody2D>();

            // Apply the force to the currentRing when releasing the mouse button
            rb.AddForce(dragDirection * Mathf.Clamp(dragDistance * tossForceMultiply, 0, maxTossForce), ForceMode2D.Impulse);
            rb.gravityScale = GlobalSettings.Physics.ballGravity;

            FindObjectOfType<GameScoreKeeper>().IncrementProjectile();

            canToss = false;
        }
    }
}
