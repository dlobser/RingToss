using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Request : MonoBehaviour
{
    // public ExtraValuesForTxt2Image ExtraValuesForTxt2Image;
    // public ExtraValuesForImg2Img extraValuesForImg2Img;
    // public Image centralSprite;
    // public Button resendButton;
    // public GameObject uploadMask;
    // public GameObject uploadDepth;

    // public SpriteRenderer showImage;

    // public string img2imgLocation;
    private string lastDownloadBase64;
    // [HideInInspector] public string maskFromComputer;
    // [HideInInspector] public string depthFromComputer;

    // public string maskLocation;
    // public string depthLocation;
    // public string downloadLocationForTxt2img;
    // public string downloadLocationForImg2img;

    ApiManagerTxt2Img.AlwaysonScripts alwaysonScripts;// = new ApiManagerTxt2Img.AlwaysonScripts();
    //alwaysonScripts.controlnet = new ApiManagerTxt2Img.Controlnet();
    //alwaysonScripts.controlnet.args = new List<ApiManagerTxt2Img.Arg>();

    // alwaysonScripts.tiling = new ApiManagerImg2Img.ti
    // if (extraValuesForImg2Img.useControlNet)
    // {


    ApiManagerTxt2Img.Arg apimanager_txt2img_arg;// = new ApiManagerTxt2Img.Arg();

    private void Start()
    {
        // depthFromComputer = ConvertImage(depthLocation, showImage);
        // maskFromComputer = ConvertImage(depthLocation, showImage);
        // UploadMaskButton();
        // UploadDepthButton();

        alwaysonScripts = new ApiManagerTxt2Img.AlwaysonScripts();
        alwaysonScripts.controlnet = new ApiManagerTxt2Img.Controlnet();
        alwaysonScripts.controlnet.args = new List<ApiManagerTxt2Img.Arg>();

        // alwaysonScripts.tiling = new ApiManagerImg2Img.ti
        // if (extraValuesForImg2Img.useControlNet)
        // {


        apimanager_txt2img_arg = new ApiManagerTxt2Img.Arg();

    }

    public void SendTxt2Img(ExtraValuesForTxt2Image ExtraValuesForTxt2Image, string depth, Action<string> onComplete)
    {

        ApiManagerTxt2Img.AlwaysonScripts alwaysonScripts = new ApiManagerTxt2Img.AlwaysonScripts();

        // if(ExtraValuesForTxt2Image.useControlNet){
        alwaysonScripts.controlnet = new ApiManagerTxt2Img.Controlnet();
        alwaysonScripts.controlnet.args = new List<ApiManagerTxt2Img.Arg>();
        ApiManagerTxt2Img.Arg arg = new ApiManagerTxt2Img.Arg();

        arg.input_image = depth;
        arg.module = "none";
        // arg.model = "control_sd15_depth [fef5e48e]";
        arg.model = "control_v11f1p_sd15_depth [cfd03158]";
        // arg.model = "diff_control_sd15_depth_fp16 [978ef0a1]";
        arg.weight = ExtraValuesForTxt2Image.controlnet_weight;
        arg.guidance_end = ExtraValuesForTxt2Image.controlnet_guidance_end;
        alwaysonScripts.controlnet.args.Add(arg);
        // }

        alwaysonScripts.Asymmetric_tiling = new ApiManagerTxt2Img.Tiling();
        alwaysonScripts.Asymmetric_tiling.args = new List<object>() { false, true, true, 0, -1 };


        // print("always on: " + alwaysonScripts);

        if (ExtraValuesForTxt2Image.useControlNet || ExtraValuesForTxt2Image.tileX || ExtraValuesForTxt2Image.tileY)
        {
            NetCenter.Instance.Send<ApiManagerTxt2Img.Txt2ImageResponse>(
                new ApiManagerTxt2Img.Text2ImgNewRequestWithControlNet()
                {
                    enable_hr = ExtraValuesForTxt2Image.enable_hr,
                    denoising_strength = ExtraValuesForTxt2Image.denoising_strength,
                    firstphase_width = ExtraValuesForTxt2Image.firstphase_width,
                    firstphase_height = ExtraValuesForTxt2Image.firstphase_height,
                    styles = ExtraValuesForTxt2Image.styles.Count > 0 ? ExtraValuesForTxt2Image.styles : null,
                    seed = ExtraValuesForTxt2Image.seed,
                    subseed = ExtraValuesForTxt2Image.subseed,
                    subseed_strength = ExtraValuesForTxt2Image.subseed_strength,
                    seed_resize_from_h = ExtraValuesForTxt2Image.seed_resize_from_h,
                    seed_resize_from_w = ExtraValuesForTxt2Image.seed_resize_from_w,
                    batch_size = ExtraValuesForTxt2Image.batch_size,
                    n_iter = ExtraValuesForTxt2Image.n_iter,
                    steps = ExtraValuesForTxt2Image.steps,
                    cfg_scale = ExtraValuesForTxt2Image.cfg_scale,
                    width = ExtraValuesForTxt2Image.width,
                    height = ExtraValuesForTxt2Image.height,
                    restore_faces = ExtraValuesForTxt2Image.restore_faces,
                    tiling = ExtraValuesForTxt2Image.tiling,
                    negative_prompt = ExtraValuesForTxt2Image.negative_prompt,
                    eta = ExtraValuesForTxt2Image.eta,
                    s_churn = ExtraValuesForTxt2Image.s_churn,
                    s_tmax = ExtraValuesForTxt2Image.s_tmax,
                    s_tmin = ExtraValuesForTxt2Image.s_tmin,
                    s_noise = ExtraValuesForTxt2Image.s_noise,
                    sampler_index = ExtraValuesForTxt2Image.sampler_index,
                    prompt = ExtraValuesForTxt2Image.prompt,
                    sampler_name = ExtraValuesForTxt2Image.sampler_name,
                    alwayson_scripts = alwaysonScripts
                },
                msg =>
                {
                    var response = msg as ApiManagerTxt2Img.Txt2ImageResponse;
                    lastDownloadBase64 = response.images[0];
                    // Texture2D tex = ConvertAndSave(lastDownloadBase64, downloadLocationForTxt2img); // Location of txt2img
                    onComplete?.Invoke(lastDownloadBase64);
                    // sendImg2Img();
                }, e => { Debug.Log(e); });
        }
        else
        {
            NetCenter.Instance.Send<ApiManagerTxt2Img.Txt2ImageResponse>(
            new ApiManagerTxt2Img.Text2ImgNewRequest()
            {
                enable_hr = ExtraValuesForTxt2Image.enable_hr,
                denoising_strength = ExtraValuesForTxt2Image.denoising_strength,
                firstphase_width = ExtraValuesForTxt2Image.firstphase_width,
                firstphase_height = ExtraValuesForTxt2Image.firstphase_height,
                styles = ExtraValuesForTxt2Image.styles.Count > 0 ? ExtraValuesForTxt2Image.styles : null,
                seed = ExtraValuesForTxt2Image.seed,
                subseed = ExtraValuesForTxt2Image.subseed,
                subseed_strength = ExtraValuesForTxt2Image.subseed_strength,
                seed_resize_from_h = ExtraValuesForTxt2Image.seed_resize_from_h,
                seed_resize_from_w = ExtraValuesForTxt2Image.seed_resize_from_w,
                batch_size = ExtraValuesForTxt2Image.batch_size,
                n_iter = ExtraValuesForTxt2Image.n_iter,
                steps = ExtraValuesForTxt2Image.steps,
                cfg_scale = ExtraValuesForTxt2Image.cfg_scale,
                width = ExtraValuesForTxt2Image.width,
                height = ExtraValuesForTxt2Image.height,
                restore_faces = ExtraValuesForTxt2Image.restore_faces,
                tiling = ExtraValuesForTxt2Image.tiling,
                negative_prompt = ExtraValuesForTxt2Image.negative_prompt,
                eta = ExtraValuesForTxt2Image.eta,
                s_churn = ExtraValuesForTxt2Image.s_churn,
                s_tmax = ExtraValuesForTxt2Image.s_tmax,
                s_tmin = ExtraValuesForTxt2Image.s_tmin,
                s_noise = ExtraValuesForTxt2Image.s_noise,
                sampler_index = ExtraValuesForTxt2Image.sampler_index,
                prompt = ExtraValuesForTxt2Image.prompt,
                sampler_name = ExtraValuesForTxt2Image.sampler_name,
            },
            msg =>
            {
                var response = msg as ApiManagerTxt2Img.Txt2ImageResponse;
                lastDownloadBase64 = response.images[0];
                // Texture2D tex = ConvertAndSave(lastDownloadBase64, downloadLocationForTxt2img); // Location of txt2img
                onComplete?.Invoke(lastDownloadBase64);
                // sendImg2Img();
            }, e => { Debug.Log(e); });
        }
    }

    public void sendImg2Img(ExtraValuesForImg2Img extraValuesForImg2Img, string image, string depthImage, Action<string> onComplete)
    {
        // print("Img2Img Called");
        // print(lastDownloadBase64);

        // ApiManagerTxt2Img.AlwaysonScripts alwaysonScripts = new ApiManagerTxt2Img.AlwaysonScripts();
        // alwaysonScripts.controlnet = new ApiManagerTxt2Img.Controlnet();
        // alwaysonScripts.controlnet.args = new List<ApiManagerTxt2Img.Arg>();

        // // alwaysonScripts.tiling = new ApiManagerImg2Img.ti
        // // if (extraValuesForImg2Img.useControlNet)
        // // {


        // ApiManagerTxt2Img.Arg apimanager_txt2img_arg = new ApiManagerTxt2Img.Arg();

        //Values
        // apimanager_txt2img_arg.input_image = depthImage;//depthFromComputer;
        // apimanager_txt2img_arg.module = "none";
        // apimanager_txt2img_arg.model = "control_sd15_depth [fef5e48e]";
        // // apimanager_txt2img_arg.model = "control_v11f1p_sd15_depth [cfd03158]";//
        // // apimanager_txt2img_arg.model = "diff_control_sd15_depth_fp16 [978ef0a1]";
        // apimanager_txt2img_arg.weight = extraValuesForImg2Img.controlnet_weight;
        // apimanager_txt2img_arg.guidance_end = extraValuesForImg2Img.controlnet_guidance_end;

        // alwaysonScripts.controlnet.args.Add(apimanager_txt2img_arg);
        // }


        // alwaysonScripts.asymmetric_tiling = new ApiManagerTxt2Img.Tiling();
        // alwaysonScripts.asymmetric_tiling.args = new List<ApiManagerTxt2Img.TilingArgs>();
        // ApiManagerTxt2Img.TilingArgs tilingArgs = new ApiManagerTxt2Img.TilingArgs();
        // tilingArgs.active = true;
        // tilingArgs.tileX = true;
        // tilingArgs.tileY = false;
        // tilingArgs.startStep = 0;
        // tilingArgs.stopStep = -1;
        // alwaysonScripts.asymmetric_tiling.args.Add(tilingArgs);

        print(JsonUtility.ToJson(alwaysonScripts));

        ApiManagerImg2Img.ScriptArgs img2img_scriptArgs = new ApiManagerImg2Img.ScriptArgs();
        img2img_scriptArgs.args = ApiManagerImg2Img.ParseScriptArgs(extraValuesForImg2Img.script_name, extraValuesForImg2Img.script_args);

        if (extraValuesForImg2Img.useControlNet)
        {
            ApiManagerImg2Img.Image2ImageRequestWithControlNet req = new ApiManagerImg2Img.Image2ImageRequestWithControlNet()
            {
                init_images = new List<string>() { image },
                // mask = "",//maskFromComputer,
                resize_mode = extraValuesForImg2Img.resize_mode,
                prompt = extraValuesForImg2Img.prompt,
                denoising_strength = extraValuesForImg2Img.denoising_strength,
                // mask_blur = extraValuesForImg2Img.mask_blur,

                firstphase_width = extraValuesForImg2Img.firstphase_width,
                firstphase_height = extraValuesForImg2Img.firstphase_height,
                hr_upscaler = extraValuesForImg2Img.hr_upscaler,
                hr_scale = extraValuesForImg2Img.hr_scale,
                hr_resize_x = extraValuesForImg2Img.hr_resize_x,
                hr_resize_y = extraValuesForImg2Img.hr_resize_y,
                inpainting_fill = extraValuesForImg2Img.inpainting_fill,
                inpaint_full_res = extraValuesForImg2Img.inpaint_full_res,
                inpaint_full_res_padding = extraValuesForImg2Img.inpaint_full_res_padding,
                inpainting_mask_invert = extraValuesForImg2Img.inpainting_mask_invert,

                styles = extraValuesForImg2Img.styles,

                seed = extraValuesForImg2Img.seed,
                subseed = extraValuesForImg2Img.subseed,
                subseed_strength = extraValuesForImg2Img.subseed_strength,
                seed_resize_from_h = extraValuesForImg2Img.seed_resize_from_h,
                seed_resize_from_w = extraValuesForImg2Img.seed_resize_from_w,

                batch_size = extraValuesForImg2Img.batch_size,

                n_iter = extraValuesForImg2Img.n_iter,
                steps = extraValuesForImg2Img.steps,

                cfg_scale = extraValuesForImg2Img.cfg_scale,
                width = extraValuesForImg2Img.width,
                height = extraValuesForImg2Img.height,
                restore_faces = extraValuesForImg2Img.restore_faces,
                tiling = extraValuesForImg2Img.tiling,
                negative_prompt = extraValuesForImg2Img.negative_prompt,
                eta = extraValuesForImg2Img.eta,
                s_churn = extraValuesForImg2Img.s_churn,
                s_tmax = extraValuesForImg2Img.s_tmax,
                s_tmin = extraValuesForImg2Img.s_tmin,
                s_noise = extraValuesForImg2Img.s_noise,
                sampler_index = extraValuesForImg2Img.sampler_index,
                script_name = extraValuesForImg2Img.script_name,
                script_args = img2img_scriptArgs.args,//ApiManagerImg2Img.ParseScriptArgs(extraValuesForImg2Img.script_name, extraValuesForImg2Img.script_args),//extraValuesForImg2Img.script_args,
                include_init_images = extraValuesForImg2Img.include_init_images,
                alwayson_scripts = alwaysonScripts,
            };
            NetCenter.Instance.Send<ApiManagerImg2Img.Image2ImageResponse>(req,
            msg =>
            {

                var response = msg as ApiManagerImg2Img.Image2ImageResponse;
                print(response);
                lastDownloadBase64 = response.images[0];
                // Texture2D tex = ConvertAndSave(lastDownloadBase64,
                // downloadLocationForImg2img); //Location of response image from img2img
                // resendButton.interactable = true;
                onComplete?.Invoke(lastDownloadBase64);
            }, e => { Debug.Log(e); });
        }
        else
        {
            ApiManagerImg2Img.Image2ImageRequest req = new ApiManagerImg2Img.Image2ImageRequest()
            {
                init_images = new List<string>() { image },
                // mask = "",//maskFromComputer,
                resize_mode = extraValuesForImg2Img.resize_mode,
                prompt = extraValuesForImg2Img.prompt,
                denoising_strength = extraValuesForImg2Img.denoising_strength,
                // mask_blur = extraValuesForImg2Img.mask_blur,

                firstphase_width = extraValuesForImg2Img.firstphase_width,
                firstphase_height = extraValuesForImg2Img.firstphase_height,
                hr_upscaler = extraValuesForImg2Img.hr_upscaler,
                hr_scale = extraValuesForImg2Img.hr_scale,
                hr_resize_x = extraValuesForImg2Img.hr_resize_x,
                hr_resize_y = extraValuesForImg2Img.hr_resize_y,
                inpainting_fill = extraValuesForImg2Img.inpainting_fill,
                inpaint_full_res = extraValuesForImg2Img.inpaint_full_res,
                inpaint_full_res_padding = extraValuesForImg2Img.inpaint_full_res_padding,
                inpainting_mask_invert = extraValuesForImg2Img.inpainting_mask_invert,

                styles = extraValuesForImg2Img.styles,

                seed = extraValuesForImg2Img.seed,
                subseed = extraValuesForImg2Img.subseed,
                subseed_strength = extraValuesForImg2Img.subseed_strength,
                seed_resize_from_h = extraValuesForImg2Img.seed_resize_from_h,
                seed_resize_from_w = extraValuesForImg2Img.seed_resize_from_w,

                batch_size = extraValuesForImg2Img.batch_size,

                n_iter = extraValuesForImg2Img.n_iter,
                steps = extraValuesForImg2Img.steps,

                cfg_scale = extraValuesForImg2Img.cfg_scale,
                width = extraValuesForImg2Img.width,
                height = extraValuesForImg2Img.height,
                restore_faces = extraValuesForImg2Img.restore_faces,
                tiling = extraValuesForImg2Img.tiling,
                negative_prompt = extraValuesForImg2Img.negative_prompt,
                eta = extraValuesForImg2Img.eta,
                s_churn = extraValuesForImg2Img.s_churn,
                s_tmax = extraValuesForImg2Img.s_tmax,
                s_tmin = extraValuesForImg2Img.s_tmin,
                s_noise = extraValuesForImg2Img.s_noise,
                sampler_index = extraValuesForImg2Img.sampler_index,
                script_name = extraValuesForImg2Img.script_name,
                script_args = img2img_scriptArgs.args,//ApiManagerImg2Img.ParseScriptArgs(extraValuesForImg2Img.script_name, extraValuesForImg2Img.script_args),//extraValuesForImg2Img.script_args,
                include_init_images = extraValuesForImg2Img.include_init_images,
            };
            NetCenter.Instance.Send<ApiManagerImg2Img.Image2ImageResponse>(req,
            msg =>
            {

                var response = msg as ApiManagerImg2Img.Image2ImageResponse;
                // print(response);
                lastDownloadBase64 = response.images[0];
                // Texture2D tex = ConvertAndSave(lastDownloadBase64,
                // downloadLocationForImg2img); //Location of response image from img2img
                // resendButton.interactable = true;
                onComplete?.Invoke(lastDownloadBase64);
            }, e => { Debug.Log(e); onComplete?.Invoke(""); });
        }
    }

    public Texture2D ConvertAndSave(string base64Image, string whereToSave)
    {
        byte[] imageBytes = Convert.FromBase64String(base64Image);
        Texture2D tex = new Texture2D(512, 512);

        tex.LoadImage(imageBytes);
        Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        // centralSprite.sprite = sprite;

        byte[] bytes = tex.EncodeToPNG();

        if (whereToSave.Length != 0)
        {
            File.WriteAllBytes(whereToSave, bytes);
            Debug.Log("File saved at: " + whereToSave);
        }

        return tex;
    }

    // public void UploadMaskButton()
    // {
    //     var extensions = new[] { new ExtensionFilter("Image files", "png", "jpg", "jpeg", "gif") };
    //     string paths = maskLocation; //Location of mask
    //     // Check if a file was selected
    //     if (paths.Length > 0)
    //     {
    //         // Load the image file as a texture
    //         byte[] fileBytes = File.ReadAllBytes(paths);
    //         Texture2D texture = new Texture2D(2, 2);
    //         texture.LoadImage(fileBytes);

    //         // Set the texture to the image component
    //         uploadMask.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(texture,
    //             new Rect(0, 0, texture.width, texture.height), Vector2.zero);

    //         // Convert the texture to a base64-encoded string
    //         string base64String = Convert.ToBase64String(fileBytes);
    //         maskFromComputer = base64String;
    //     }
    // }

    // public string ConvertImage(string imagePath, SpriteRenderer sprite)
    // {
    //     string r = "";
    //     var extensions = new[] { new ExtensionFilter("Image files", "png", "jpg", "jpeg", "gif") };
    //     string paths = maskLocation; //Location of mask
    //     // Check if a file was selected
    //     if (paths.Length > 0)
    //     {
    //         // Load the image file as a texture
    //         byte[] fileBytes = File.ReadAllBytes(imagePath);
    //         Texture2D texture = new Texture2D(2, 2);
    //         texture.LoadImage(fileBytes);

    //         // Set the texture to the image component
    //         sprite.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

    //         // Convert the texture to a base64-encoded string
    //         string base64String = Convert.ToBase64String(fileBytes);
    //         r = base64String;
    //     }
    //     return r;
    // }

    // public void UploadDepthButton()
    // {
    //     var extensions = new[] { new ExtensionFilter("Image files", "png", "jpg", "jpeg", "gif") };
    //     string paths = maskLocation; //Location of mask
    //     // Check if a file was selected
    //     if (paths.Length > 0)
    //     {
    //         // Load the image file as a texture
    //         byte[] fileBytes = File.ReadAllBytes(paths);
    //         Texture2D texture = new Texture2D(2, 2);
    //         texture.LoadImage(fileBytes);

    //         // Set the texture to the image component
    //         uploadDepth.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(texture,
    //             new Rect(0, 0, texture.width, texture.height), Vector2.zero);

    //         // Convert the texture to a base64-encoded string
    //         string base64String = Convert.ToBase64String(fileBytes);
    //         depthFromComputer = base64String;
    //     }
    // }

    public void RestartScene()
    {
        SceneManager.LoadScene(0);
    }
}