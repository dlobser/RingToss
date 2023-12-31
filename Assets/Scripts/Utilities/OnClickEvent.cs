using UnityEngine;
using UnityEngine.Events;

public class OnClickEvent : MonoBehaviour
{
    public UnityEvent unityEvent;

    private void Update()
    {
        // Check if the left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            unityEvent.Invoke();
            // Convert the mouse position to world point
            Vector2 mousePos = Vector2.zero;
            if(Camera.main!=null)
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            else
                Debug.LogWarning("Main Camera Missing!");

            // Cast a ray from the camera to the point where the mouse clicked
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                unityEvent.Invoke();
                print("Clicked");
                // Check if the object that was hit is a child of this object
                // if (hit.transform.parent == transform)
                // {
                //     // Hide the child object
                //     hit.transform.gameObject.SetActive(false);
                // }
            }
        }
    }

    public void ShowChild()
    {
        this.transform.GetChild(0).gameObject.SetActive(true);
    }
}
