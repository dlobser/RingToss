using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraValuesForTxt2Image : ExtraValues
{
    public bool enable_hr;
    public bool useControlNet;
    public double denoising_strength;
    public int firstphase_width;
    public int firstphase_height;
    public List<string> styles;
    public int seed;
    public int subseed;
    public int subseed_strength;
    public int seed_resize_from_h;
    public int seed_resize_from_w;
    public int batch_size;
    public int n_iter;
    public float steps;
    public float cfg_scale;
    public float width;
    public float height;
    public bool restore_faces;
    public bool tiling;
    public string negative_prompt;
    public int eta;
    public int s_churn;
    public int s_tmax;
    public int s_tmin;
    public int s_noise;
    public string sampler_index;
    public string prompt;
    public string sampler_name;

    public float controlnet_weight = 1;
    public float controlnet_guidance_end = 1f;
}