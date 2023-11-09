using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using MM.Msg;
using UnityEngine.UI;

namespace MM
{
    public static class ImageHelper
    {

        public static string UseProxy(string url)
        {
            return  url; //ChapterXRemoteConfig.Instance.GetProxyURL()
        }
        //default astronaut icon
        //public static string s_DefaultUserIcon = "https://res.cloudinary.com/dtgfpjvoi/image/upload/v1637484959///moonverse.world%20-%20assets/moonverse_logo_white_dety7v.png";
     //   public static string s_DefaultUserIcon = "https://res.cloudinary.com/dtgfpjvoi/image/upload/v1646326778/moonverse.market%20-%20assets/MOONVERSE-MARKET/SpaceKid_yumyw8.png";

        public static Sprite GetItemIcon(string icon)
        {
            return Resources.Load<Sprite>($"Datas/Icons/" + icon);
        }

        //The texture resize and return a new texturfe
        public static Texture2D ResizeTexture(this Texture2D self, int targetWidth, int targetHeight)
        {
            //use own texture format
            return self.ResizeTexture(targetWidth, targetHeight, self.format);
        }

        //The texture resize and change format
        public static Texture2D ResizeTexture(this Texture2D self, int targetWidth, int targetHeight, TextureFormat format)
        {
            if (self.width < self.height) targetWidth = self.width * targetHeight / self.height;
            else if (self.width > self.height) targetHeight = self.height * targetWidth / self.width;
            
            Texture2D result = new Texture2D(targetWidth, targetHeight, format, false);
            Color[] newPixels = result.GetPixels(0);
            float incX = (1f / self.width) * (self.width / (float)targetWidth);
            float incY = (1f / self.height) * (self.height / (float)targetHeight);
            for (int px = 0; px < newPixels.Length; px++)
            {
                newPixels[px] = self.GetPixelBilinear(incX * (px % (float)targetWidth),
                                  incY * Mathf.Floor(px / (float)targetWidth));
            }
            result.SetPixels(newPixels, 0);
            result.Apply();
            GameObject.Destroy(self, 1.0f);
            return result;
        }

        //Get the first frame of the gif, refer to GifHelper.cs
        // public static Texture2D GetFirstFrameFromGif(byte[] bytes)
        // {
        //     //Init a gif
        //     var gif = new GifPlayer.GifProtocol.GraphicsInterchangeFormat(bytes);
        //     //Init first frame
        //     var firstFromeTex = new Texture2D(gif.Width, gif.Height);
        //
        //     //frameIndex force to 0
        //     var frameIndex = 0;
        //
        //     //Image Descriptor
        //     var frameImageDescriptor = gif.FrameImageDescriptors[frameIndex];
        //     //Graphic Controller
        //     var frameGraphicController = gif.FrameGraphicControllers[frameIndex];
        //
        //     var blockWidth = frameImageDescriptor.Width;
        //     var blockHeight = frameImageDescriptor.Height;
        //
        //     var leftIndex = frameImageDescriptor.MarginLeft;//Inclusive
        //     var rightBorder = leftIndex + blockWidth;//Exclusive
        //
        //     var topBorder = gif.Height - frameImageDescriptor.MarginTop;//Exclusive
        //     var bottomIndex = topBorder - blockHeight;//Inclusive
        //
        //     //descriptor pixel array
        //     var descriptorPixels = frameImageDescriptor.GetPixels(frameGraphicController, gif);
        //     //the index of descriptor pixel array
        //     var colorIndex = -1;
        //     //The y of gif is from large to small, and the y of texture is from small to large
        //     for (var y = topBorder - 1; y >= bottomIndex; y--)
        //     {
        //         for (var x = leftIndex; x < rightBorder; x++)
        //         {
        //             colorIndex++;
        //             firstFromeTex.SetPixel(x, y, descriptorPixels[colorIndex]);
        //         }
        //     }
        //
        //     //Save
        //     firstFromeTex.wrapMode = TextureWrapMode.Clamp;
        //     firstFromeTex.Apply();
        //
        //     return firstFromeTex;
        // }

        //TODO: now formats other than lists are not supported
        //Detect the image format by the first byte and the second byte
        public static Dictionary<string, ImageFormat> s_ImageFormatTable = new Dictionary<string, ImageFormat>(){
            {"FFD8",ImageFormat.JPG1},
            {"FFD9",ImageFormat.JPG2},
            {"8950",ImageFormat.PNG},
            {"4749",ImageFormat.GIF},
        };

        public static ImageFormat GetImageFormat(byte[] bytes)
        {
            ImageFormat _format = ImageFormat.Empty;
            //empty image
            if (bytes.Length < 2) return _format;

            string formatKey = bytes[0].ToString("X") + bytes[1].ToString("X");
            var result = s_ImageFormatTable.TryGetValue(formatKey, out _format);
            if (result) return _format;

            //unknown format
            return ImageFormat.Unknown;
        }


    }
    public enum ImageFormat
    {
        Empty,
        JPG1,//SOI
        JPG2,//EOI
        PNG,
        GIF,
        Unknown,
    }

}