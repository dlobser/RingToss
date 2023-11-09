using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public int steps;
    public Animator animator;
    Animation anim;
    int step = 0;
    public bool autoStep;
    public bool doStep;

    public void StepAnimation()
    {
        animator.SetFloat("time", Mathf.Min(.999f, (float)step / ((float)steps - 1)));
        step++;
        if (step >= steps)
        {
            step = 0;
        }
        print("Animation Step " + step);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (doStep)
        {
            StepAnimation();
            doStep = false;
        }
        if (autoStep)
        {
            animator.SetFloat("time", Mathf.Min(.999f, (float)step / ((float)steps - 1)));
            step++;
            if (step >= steps)
            {
                step = 0;
            }
        }
    }
}
