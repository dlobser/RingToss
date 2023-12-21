using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShaderFloatAnimator : MonoBehaviour
{
    public GameObject targetObject; // The parent object whose children's materials you want to modify
    public string shaderVariable = "_ShaderVariable"; // The name of the variable in the shader
    public float duration = 2.0f; // The duration of the animation
    public float min = 0f; // Minimum value for the animated float
    public float max = 1f; // Maximum value for the animated float
    public bool useInitialValue = false; // Whether to use the initial value of the shader variable as the minimum value
    public bool reverseTime; // Reverse the animation time

    private List<Material> materials; // List to store materials
    private List<LineRenderer> lineRenderers; // List to store LineRenderers
    private List<SpriteRenderer> spriteRenderers; // List to store SpriteRenderers

    private void Start()
    {
        materials = new List<Material>();
        lineRenderers = new List<LineRenderer>();
        spriteRenderers = new List<SpriteRenderer>();

        // Get all MeshRenderer components in children and store their materials
        MeshRenderer[] meshRenderers = targetObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            foreach (Material mat in meshRenderer.materials)
            {
                if(mat.HasProperty(shaderVariable))
                    materials.Add(mat);
            }
        }

        // Get all LineRenderer components in children
        lineRenderers.AddRange(targetObject.GetComponentsInChildren<LineRenderer>());

        // Get all SpriteRenderer components in children
        spriteRenderers.AddRange(targetObject.GetComponentsInChildren<SpriteRenderer>());

        if (useInitialValue && materials.Count > 0 && materials[0].HasProperty(shaderVariable))
        {
            // Get the initial value of the shader variable
            min = materials[0].GetFloat(shaderVariable);
            max = min + max;
        }

        // Animate();
    }

    public void Animate()
    {
        if(targetObject!=null)
            if(targetObject.activeInHierarchy)
                StartCoroutine(AnimateShaderVariable());
    }

    private IEnumerator AnimateShaderVariable()
    {
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float phase = Mathf.Cos((time / duration) * Mathf.PI * 2) * -0.5f + 0.5f;
            phase = Mathf.Lerp(min, max, reverseTime ? 1 - (time / duration) : (time / duration));

            foreach (Material mat in materials)
            {
                mat.SetFloat(shaderVariable, phase);
            }

            foreach (LineRenderer lineRenderer in lineRenderers)
            {
                lineRenderer.material.SetFloat(shaderVariable, phase);
            }

            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.material.SetFloat(shaderVariable, phase);
            }

            yield return null;
        }

        // Set the final value to ensure it ends at the starting value
        foreach (Material mat in materials)
        {
            mat.SetFloat(shaderVariable, min);
        }

        foreach (LineRenderer lineRenderer in lineRenderers)
        {
            lineRenderer.material.SetFloat(shaderVariable, min);
        }

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.material.SetFloat(shaderVariable, min);
        }
    }
}
