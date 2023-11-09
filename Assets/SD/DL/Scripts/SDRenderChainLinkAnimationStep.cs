using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SDRenderChainLinkAnimationStep : SDRenderChainLink
{

    AnimationController anim;
    public override void RunUnityFunction(string image)
    {
        if (anim == null)
        {
            anim = FindObjectOfType<AnimationController>();
        }
        if (anim != null)
        {
            anim.StepAnimation();
        }
    }


}
