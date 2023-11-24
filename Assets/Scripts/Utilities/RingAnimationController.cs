using UnityEngine;

public class RingAnimationController : MonoBehaviour
{
    public Renderer ringRenderer;
    // public float speed = 1.0f;
    public AnimationCurve sizeCurve;
    public Gradient colorGradient;
    public float duration = 5.0f;

    public float timeElapsed = 0.0f;

    private Material ringMaterial;
    public bool animate = true;

    private void Start()
    {
        if (ringRenderer != null)
        {
            ringMaterial = ringRenderer.material;
        }
    }

    private void Update()
    {
        if (ringMaterial != null)
        {
            if (timeElapsed < 1 && animate)
                timeElapsed += Time.deltaTime / duration;

            // // Loop the animation

            // {
            //     timeElapsed = 0.0f;
            // }

            // Animate thickness based on the size curve
            float thickness = sizeCurve.Evaluate(timeElapsed / 1);
            ringMaterial.SetFloat("_Thickness", thickness);

            // Animate color based on the color gradient
            Color color = colorGradient.Evaluate(timeElapsed / 1);
            ringMaterial.SetColor("_MainColor", color);
        }
    }
}
