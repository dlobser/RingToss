using UnityEngine;

public class Target : MonoBehaviour
{
    public CustomTag customTag = CustomTag.Target;

    public int scoreValue = 10; // Score value for this target

    private void Start()
    {

    }

    public void Hit()
    {
        Destroy(this.gameObject);
    }
}
