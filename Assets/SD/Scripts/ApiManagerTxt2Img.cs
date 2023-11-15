using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MM.Msg;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class ApiManagerTxt2Img : Singleton<ApiManagerTxt2Img>
{
    public string bearer;
    public TMP_InputField promptInput;
    public TMP_InputField NegativepromptInput;


    public TMP_InputField samplingStep;
    public TMP_InputField width;
    public TMP_InputField height;
    public TMP_InputField cfgScale;
    public Toggle enable_hr;
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
    public TMP_InputField eta;
    public TMP_InputField s_churn;
    public TMP_InputField s_tmax;
    public TMP_InputField s_tmin;
    public TMP_InputField s_noise;
    public TMP_InputField sampler_index;
    public Tiling Asymmetric_tiling_Argument;

    public class AlwaysonScripts
    {
        public Controlnet controlnet { get; set; }

        [JsonProperty("Asymmetric tiling")] 
        public Tiling Asymmetric_tiling { get; set; }
    }

    public class Arg
    {
        public string input_image { get; set; }
        public string module { get; set; }
        public string model { get; set; }
        public float weight { get; set; }
        public float guidance_end { get; set; }
    }

    public class TilingArgs
    {
        public bool active { get; set; }
        public bool tileX { get; set; }
        public bool tileY { get; set; }
        public int startStep { get; set; }
        public int stopStep { get; set; }
    }

    public class Controlnet
    {
        public List<Arg> args { get; set; }
    }

    public class Tiling
    {
        public List<object> args;
    }

    public class ScriptArgs
    {
        public object[] values;
    }

    // public class ScriptArgs_SDUpscale : ScriptArgs{
    //     public string - int - string - float
    // }


    public class Text2ImgNewRequestWithControlNet : Text2ImgNewRequest
    {

        public ApiManagerTxt2Img.AlwaysonScripts alwayson_scripts { get; set; }

        public override string PostUrl() =>
            "http://127.0.0.1:7860/sdapi/v1/txt2img";
    }

    public class Text2ImgNewRequest : BaseRequest
    {
        public bool enable_hr { get; set; }
        public double denoising_strength { get; set; }
        public int firstphase_width { get; set; }
        public int firstphase_height { get; set; }
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
        public string sampler_index { get; set; }
        public string prompt { get; set; }
        public string sampler_name { get; set; }
        // public AlwaysonScripts alwayson_scripts { get; setsS; }

        public override string PostUrl() =>
            "http://127.0.0.1:7860/sdapi/v1/txt2img";//http://3.137.201.231:8080/sdapi/v1/txt2img";
    }


    [HideInInspector] public string lastDownloadedBase64;

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Txt2ImageRequest : BaseRequest
    {
        public bool enable_hr { get; set; }
        public double denoising_strength { get; set; }
        public int firstphase_width { get; set; }
        public int firstphase_height { get; set; }
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
        public string sampler_index { get; set; }
        
        public override string PostUrl() =>
            "http://127.0.0.1:7860/sdapi/v1/txt2img";//"http://3.137.201.231:8080/sdapi/v1/txt2img";
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Parameters
    {
        public bool enable_hr { get; set; }
        public double denoising_strength { get; set; }
        public int firstphase_width { get; set; }
        public int firstphase_height { get; set; }
        public double hr_scale { get; set; }
        public object hr_upscaler { get; set; }
        public int hr_second_pass_steps { get; set; }
        public int hr_resize_x { get; set; }
        public int hr_resize_y { get; set; }
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
        public object override_settings { get; set; }
        public bool override_settings_restore_afterwards { get; set; }
        public List<object> script_args { get; set; }
        public string sampler_index { get; set; }
        public object script_name { get; set; }
    }

    public class Txt2ImageResponse : BaseResponse
    {
        public List<string> images { get; set; }
        public Parameters parameters { get; set; }
        public string info { get; set; }
    }

    public void SendRequestTxt2Img(Action callback)
    {
        NetCenter.Instance.Send<Txt2ImageResponse>(new Txt2ImageRequest()
        {
            enable_hr = enable_hr.isOn,
            denoising_strength = Double.Parse(denoising_strength.text),
            firstphase_height = Int32.Parse(firstphase_height.text),
            firstphase_width = Int32.Parse(firstphase_width.text),
            prompt = promptInput.text,
            seed = Int32.Parse(seed.text),
            subseed = Int32.Parse(subseed.text),
            subseed_strength = Int32.Parse(subseed_strength.text),
            seed_resize_from_h = Int32.Parse(seed_resize_from_h.text),
            seed_resize_from_w = Int32.Parse(seed_resize_from_w.text),
            batch_size = Int32.Parse(batch_size.text),
            n_iter = Int32.Parse(n_iter.text),
            steps = Int32.Parse(samplingStep.text),
            cfg_scale = Int32.Parse(cfgScale.text),
            width = Int32.Parse(width.text),
            height = Int32.Parse(height.text),
            restore_faces = restore_faces.isOn,
            tiling = tiling.isOn,
            negative_prompt = NegativepromptInput.text,
            eta = Int32.Parse(eta.text),
            s_churn = Int32.Parse(s_churn.text),
            s_tmax = Int32.Parse(s_tmax.text),
            s_tmin = Int32.Parse(s_tmin.text),
            s_noise = Int32.Parse(s_noise.text),
            sampler_index = sampler_index.text,

        },
            msg =>
            {
                var response = msg as Txt2ImageResponse;
                callback?.Invoke();
                ConvertFromBase(response.images[0]);
            }, e => { Debug.Log(e); });
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

    public void SaveImage(Sprite sprite)
    {
        // string proj_path = Application.dataPath;
        // var abs_path = Path.Combine(Application.dataPath, proj_path);
        // proj_path = Path.Combine("Assets", proj_path);
        //
        // Directory.CreateDirectory(Path.GetDirectoryName(abs_path));
        // File.WriteAllBytes(abs_path, ImageConversion.EncodeToPNG(sprite.texture));
        //
        // AssetDatabase.Refresh();
        //
        // var ti = AssetImporter.GetAtPath(proj_path) as TextureImporter;
        // ti.spritePixelsPerUnit = sprite.pixelsPerUnit;
        // ti.mipmapEnabled = false;
        // ti.textureType = TextureImporterType.Sprite;
        //
        // EditorUtility.SetDirty(ti);
        // ti.SaveAndReimport();

        //return AssetDatabase.LoadAssetAtPath<Sprite>(proj_path);  //Tu shenaxvis mere amogeba dagvchirda
    }
}