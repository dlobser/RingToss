using UnityEngine;
using System.Collections;

public class CollisionScaleEffect : MonoBehaviour
{
    public float holdTime = 2.0f; // Time to hold the object at zero scale
    private Vector3 originalScale;
    private bool isScaling = false; // To prevent concurrent scaling coroutines
    Collider2D collider;
    public GameObject particleSystem;

    void Start()
    {
        // Store the original scale of the object
        originalScale = transform.localScale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        // Trigger the scaling effect when a collision occurs
        if (!isScaling)
        {
            StartCoroutine(ScaleEffectCoroutine());
        }
    }

    IEnumerator ScaleEffectCoroutine()
    {
        isScaling = true;
        
        if(collider==null)
            collider = GetComponent<Collider2D>();
        if(collider!=null)
            collider.enabled = false;

        
        // Animate scale down to zero over one second
        yield return StartCoroutine(AnimateScale(Vector3.zero, 1f));
        if(particleSystem!=null)
            particleSystem.SetActive(false);
        // Wait for the specified hold time
        yield return new WaitForSeconds(holdTime);

        // Animate scale back to the original scale over one second
        yield return StartCoroutine(AnimateScale(originalScale, 1f));
        if(collider!=null)
            collider.enabled = true;
        if(particleSystem!=null)
            particleSystem.SetActive(true);
        isScaling = false;
    }

    IEnumerator AnimateScale(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float time = 0;

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }
}
