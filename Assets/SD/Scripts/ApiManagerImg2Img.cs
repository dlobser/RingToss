using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MM.Msg;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ApiManagerImg2Img : Singleton<ApiManagerImg2Img>
{
    public string bearer;
    public TMP_InputField promptInput;
    public TMP_InputField NegativepromptInput;


    public TMP_InputField samplingStep;
    public TMP_InputField width;
    public TMP_InputField height;
    public TMP_InputField cfgScale;
    public Toggle enable_hr;
    public TMP_InputField resize_mode;
    public TMP_InputField denoising_strength;
    public TMP_InputField firstphase_height;
    public TMP_InputField firstphase_width;
    public TMP_InputField seed;
    public TMP_InputField subseed;
    public TMP_InputField subseed_strength;
    public TMP_InputField seed_resize_from_h;
    public TMP_InputField seed_resize_from_w;
    public TMP_InputField batch_size;
    public TMP_InputField n_iter;
    public Toggle restore_faces;
    public Toggle tiling;
    public Toggle includeInitImage;
    public TMP_InputField eta;
    public TMP_InputField s_churn;
    public TMP_InputField s_tmax;
    public TMP_InputField s_tmin;
    public TMP_InputField s_noise;
    public TMP_InputField sampler_index;
    public TMP_InputField script_names;
    public TMP_InputField mask_blur;
    public TMP_InputField inpainting_fill;
    public TMP_InputField inpaint_full_res_padding;
    public TMP_InputField script_args;
    public TMP_InputField styles;
    public Toggle inpaint_full_res;
    public Toggle inpainting_mask_invert;
    [HideInInspector] public string lastDownloadedBase64;

    public string imageBase64;
    public string maskBase64;


    public class Image2ImageRequestWithControlNet : Image2ImageRequest
    {

        public ApiManagerTxt2Img.AlwaysonScripts alwayson_scripts { get; set; }

        public override string PostUrl() =>
            "http://127.0.0.1:7860/sdapi/v1/img2img";
    }

    public class Image2ImageRequest : BaseRequest
    {
        public List<string> init_images { get; set; }
        public int resize_mode { get; set; }
        public int firstphase_width { get; set; }
        public int firstphase_height { get; set; }
        public float denoising_strength { get; set; }
        public float hr_scale { get; set; }
        public int hr_upscaler { get; set; }
        public int hr_second_pass_steps { get; set; }
        public int hr_resize_x { get; set; }
        public int hr_resize_y { get; set; }


        public string mask { get; set; }
        public int mask_blur { get; set; }
        public int inpainting_fill { get; set; }
        public bool inpaint_full_res { get; set; }
        public int inpaint_full_res_padding { get; set; }
        public bool inpainting_mask_invert { get; set; }

        public string prompt { get; set; }

        public List<string> styles { get; set; }
        public int seed { get; set; }
        public int subseed { get; set; }
        public int subseed_strength { get; set; }
        public int seed_resize_from_h { get; set; }
        public int seed_resize_from_w { get; set; }
        public int batch_size { get; set; }
        public int n_iter { get; set; }
        public float steps { get; set; }
        public float cfg_scale { get; set; }
        public float width { get; set; }
        public float height { get; set; }
        public bool restore_faces { get; set; }
        public bool tiling { get; set; }
        public string negative_prompt { get; set; }
        public int eta { get; set; }
        public int s_churn { get; set; }
        public int s_tmax { get; set; }
        public int s_tmin { get; set; }
        public int s_noise { get; set; }
        public OverrideSettingsImg override_settings { get; set; }
        public string sampler_index { get; set; }
        public bool include_init_images { get; set; }
        // public List<string> script_args { get; set; }
        public object[] script_args { get; set; }
        // public ApiManagerTxt2Img.ScriptArgs script_args {get;set;}

        public string script_name { get; set; }


        public override string PostUrl() =>
            "http://127.0.0.1:7860/sdapi/v1/img2img";//"http://3.137.201.231:8080/sdapi/v1/txt2img";

        // public override string PostUrl() =>
        //     "http://3.137.201.231:8080/sdapi/v1/img2img";
    }


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class OverrideSettingsImg
    {
    }

    public class ParametersImg
    {
        public object init_images { get; set; }
        public int resize_mode { get; set; }
        public double denoising_strength { get; set; }
        public object image_cfg_scale { get; set; }
        public object mask { get; set; }
        public int mask_blur { get; set; }
        public int inpainting_fill { get; set; }
        public bool inpaint_full_res { get; set; }
        public int inpaint_full_res_padding { get; set; }
        public int inpainting_mask_invert { get; set; }
        public object initial_noise_multiplier { get; set; }
        public string prompt { get; set; }
        public List<object> styles { get; set; }
        public int seed { get; set; }
        public int subseed { get; set; }
        public double subseed_strength { get; set; }
        public int seed_resize_from_h { get; set; }
        public int seed_resize_from_w { get; set; }
        public object sampler_name { get; set; }
        public int batch_size { get; set; }
        public int n_iter { get; set; }
        public int steps { get; set; }
        public double cfg_scale { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public bool restore_faces { get; set; }
        public bool tiling { get; set; }
        public string negative_prompt { get; set; }
        public double eta { get; set; }
        public double s_churn { get; set; }
        public double s_tmax { get; set; }
        public double s_tmin { get; set; }
        public double s_noise { get; set; }
        public OverrideSettingsImg override_settings { get; set; }
        public bool override_settings_restore_afterwards { get; set; }
        // public List<string> script_args { get; set; }
        public object[] script_args { get; set; }
        public string sampler_index { get; set; }
        public bool include_init_images { get; set; }
        public string script_name { get; set; }
    }

    public class Image2ImageResponse : BaseResponse
    {
        public List<string> images { get; set; }
        public ParametersImg parameters { get; set; }
        public string info { get; set; }
    }


    // public void SendRequestImg2Img(Action callback)
    // {
    //     Debug.Log(NetCenter.Instance);
    //     ScriptArgs a = new ScriptArgs();
    //     NetCenter.Instance.Send<Image2ImageResponse>(new Image2ImageRequest()
    //     {
    //         init_images = new List<string>() { imageBase64 },
    //         mask = maskBase64,
    //         resize_mode = Int32.Parse(resize_mode.text),
    //         denoising_strength = float.Parse(denoising_strength.text),
    //         mask_blur = Int32.Parse(mask_blur.text),
    //         inpainting_fill = Int32.Parse(inpainting_fill.text),
    //         inpaint_full_res = inpaint_full_res.isOn,
    //         inpaint_full_res_padding = Int32.Parse(inpaint_full_res_padding.text),
    //         inpainting_mask_invert = inpainting_mask_invert.isOn,
    //         styles = styles.text.Split(',').ToList(), //
    //         seed = Int32.Parse(seed.text),
    //         subseed = Int32.Parse(subseed.text),
    //         subseed_strength = Int32.Parse(subseed_strength.text),
    //         seed_resize_from_h = Int32.Parse(seed_resize_from_h.text),
    //         seed_resize_from_w = Int32.Parse(seed_resize_from_w.text),
    //         batch_size = Int32.Parse(batch_size.text),
    //         n_iter = Int32.Parse(n_iter.text),
    //         steps = Int32.Parse(samplingStep.text),
    //         cfg_scale = Int32.Parse(cfgScale.text),
    //         width = Int32.Parse(width.text),
    //         height = Int32.Parse(height.text),
    //         restore_faces = restore_faces.isOn,
    //         tiling = tiling.isOn,
    //         negative_prompt = NegativepromptInput.text,
    //         eta = Int32.Parse(eta.text),
    //         s_churn = Int32.Parse(s_churn.text),
    //         s_tmax = Int32.Parse(s_tmax.text),
    //         s_tmin = Int32.Parse(s_tmin.text),
    //         s_noise = Int32.Parse(s_noise.text),
    //         sampler_index = sampler_index.text,
    //         script_name = script_names.text,
    //         script_args = ParseScriptArgs(script_names.text, script_args.text.Split(',').ToList()),// script_args.text.Split(',').ToList(),
    //         include_init_images = includeInitImage.isOn
    //     },
    //         msg =>
    //         {
    //             var response = msg as Image2ImageResponse;
    //             callback?.Invoke();
    //             ConvertFromBase(response.images[0]);
    //         }, e => { Debug.Log(e); });
    // }


    public static object[] ParseScriptArgs(string scriptName, List<string> args)
    {
        switch (scriptName)
        {
            case "sd upscale":
                return ParseSDUpscale(args);
            case "Asymmetric Tiling":
                return ParseAsymmetricTiling(args);
            default:
                break;
        }
        return new object[0];
    }

    public class ScriptArgs
    {
        public object[] args;
    }

    public static object[] ParseAsymmetricTiling(List<string> args)
    {
        object[] r = new object[5];
        ScriptArgs a = new ScriptArgs();
        print("Parsing SD Upscale Args");

        if (args.Count != 4)
        {
            Debug.LogWarning("Not enough arguments for SD Upscale");
            return r;
        }
        else
        {
            r[0] = bool.Parse(args[0]);
            r[1] = bool.Parse(args[1]);
            r[2] = bool.Parse(args[2]);
            r[3] = 0;
            r[4] = -1;
            a.args = r;
            print(args[1]);
            print("Parse SD Upscale Script: " + JsonUtility.ToJson(a));
            return r;
        }

    }

    public static object[] ParseSDUpscale(List<string> args)
    {
        object[] r = new object[4];
        ScriptArgs a = new ScriptArgs();
        print("Parsing SD Upscale Args");

        if (args.Count != 4)
        {
            Debug.LogWarning("Not enough arguments for SD Upscale");
            return r;
        }
        else
        {
            r[0] = args[0].ToString();
            r[1] = int.Parse(args[1]);
            r[2] = args[2].ToString();
            r[3] = float.Parse(args[3]);
            a.args = r;
            print(args[1]);
            print("Parse SD Upscale Script: " + JsonUtility.ToJson(a));
            return r;
        }

    }





    public void ConvertFromBase(string base64Image)
    {
        lastDownloadedBase64 = base64Image;
        Debug.Log(base64Image);
        byte[] imageBytes = Convert.FromBase64String(base64Image);
        Texture2D tex = new Texture2D(512, 512);
        tex.LoadImage(imageBytes);
        Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f),
            100.0f);
        UIManager.Instance.centralImage.sprite = sprite;
    }
}