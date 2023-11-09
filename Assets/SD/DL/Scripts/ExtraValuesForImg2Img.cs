using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraValuesForImg2Img : ExtraValues
{
    public List<string> init_images;
    public int seed = -1;
    public string prompt = "a nice looking doggy";
    public string negative_prompt;
    public float steps = 20;
    public float cfg_scale = 7;
    public float width = 512;
    public float height = 512;
    public bool useControlNet;
    public float controlnet_weight = 1;
    public float controlnet_guidance_end = 1f;

    public int resize_mode;
    public int firstphase_width = 8;
    public int firstphase_height = 8;
    public float denoising_strength = .75f;
    public float hr_scale;
    public int hr_upscaler;
    public int hr_second_pass_steps;
    public int hr_resize_x;
    public int hr_resize_y;


    public string mask;
    public int mask_blur;
    public int inpainting_fill;
    public bool inpaint_full_res;
    public int inpaint_full_res_padding;
    public bool inpainting_mask_invert;
    public List<string> styles;
    public int subseed = -1;
    public int subseed_strength = 0;
    public int seed_resize_from_h = -1;
    public int seed_resize_from_w = -1;
    public int batch_size = 1;
    public int n_iter;
    public bool restore_faces = false;
    public bool tiling = false;
    public int eta;
    public int s_churn;
    public int s_tmax;
    public int s_tmin;
    public int s_noise = 1;
    // public OverrideSettingsImg override_settings ;
    public string sampler_index = "Euler a";
    public bool include_init_images;
    public List<string> script_args;
    public string script_name;


}
