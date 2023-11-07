using UnityEngine;

public class Item : MonoBehaviour
{
    public CustomTag customTag = CustomTag.Item;

    public int scoreValue = 10; // Score value for this target

    private void Start()
    {

    }

    public void Hit()
    {
        Destroy(this.gameObject);
    }
}
