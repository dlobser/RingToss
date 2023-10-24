using UnityEngine;

public class HideChildOnClick : MonoBehaviour
{
    private void Update()
    {
        // Check if the left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            // Convert the mouse position to world point
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Cast a ray from the camera to the point where the mouse clicked
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                // Check if the object that was hit is a child of this object
                if (hit.transform.parent == transform)
                {
                    // Hide the child object
                    hit.transform.gameObject.SetActive(false);
                }
            }
        }
    }

    public void ShowChild()
    {
        this.transform.GetChild(0).gameObject.SetActive(true);
    }
}
