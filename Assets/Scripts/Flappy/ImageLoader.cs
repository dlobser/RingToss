using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;

namespace Quilt
{
    public static class ImageLoader
    {
        public static string Directory { get; set; }
        public static string TextureChannel { get; set; } = "_MainTex";

        public static float MaxStyles { get; set; }
        public static string StyleNum { get; private set; }
        public static int SetStyle { get; set; }
        public static bool UseSetStyle { get; set; }

        private static List<int> availableStyles = new List<int>();

        static ImageLoader()
        {
            // ResetStyles();
        }

        public static Sprite CreateSpriteFromResource(string resourcePath)
        {
            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            if (texture == null)
            {
                Debug.LogError($"Failed to load texture at path: {resourcePath}");
                return null;
            }

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        public static void ResetStyles()
        {
            string[] allFilenames = GetFilenames(Directory);
            string pattern = @"_(\d{4})_";

            availableStyles.Clear();
            HashSet<int> uniqueStyleNumbers = new HashSet<int>();

            foreach (var filename in allFilenames)
            {
                MatchCollection matches = Regex.Matches(filename, pattern);
                foreach (Match match in matches)
                {
                    if (match.Success && int.TryParse(match.Groups[1].Value, out int styleNum))
                    {
                        uniqueStyleNumbers.Add(styleNum);
                    }
                }
            }

            availableStyles = uniqueStyleNumbers.ToList();
            availableStyles.Sort();
        }

        public static void SetRandomStyle()
        {
            if (availableStyles.Count == 0)
            {
                ResetStyles();
            }
            if (availableStyles.Count != 0)
            {
                int index = Random.Range(0, availableStyles.Count);
                int chosenStyle = availableStyles[index];

                if (UseSetStyle)
                {
                    chosenStyle = SetStyle;
                    availableStyles.Remove(chosenStyle);
                }
                else
                {
                    availableStyles.RemoveAt(index);
                }

                StyleNum = chosenStyle.ToString("D4");
            }
            else
            {
                StyleNum = "0000";
                Debug.LogWarning("No styles available.");
            }
        }

        private static string[] GetFilenames(string resourceFolder)
        {
            Object[] loadedObjects = Resources.LoadAll(resourceFolder);
            Texture2D[] textures = loadedObjects.OfType<Texture2D>().ToArray();
            return textures.Select(texture => texture.name).ToArray();
        }

        public static Sprite GetSpriteWithIndex(string imageType, int index = -1)
        {
            Texture2D tex = GetImageWithIndex(imageType, index);
            if (tex == null)
            {
                tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, Color.white);
                tex.Apply();
            }

            // Use tex.width or tex.height depending on your requirement
            float pixelsPerUnit = tex.width;
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        }


        // public static Texture2D GetImageWithIndex(string imageType, int index = 0)
        // {
        //     Texture2D[] allTextures = Resources.LoadAll<Texture2D>(Directory);
        //     Debug.Log("Theree are: " + allTextures.Length + " textures in the directory: " + Directory);
        //     Debug.Log("Last: " + allTextures[3].name.Split('_').Last() + "Global: " + Globals.GlobalSettings.randomSeed + 
        //     "splitt: " + allTextures[3].name.Split('_')[1] + "imageType: " + imageType);
        //     var matchingTextures = 
        //     allTextures.Where(texture => texture.name.Split('_')[1].Equals(imageType, System.StringComparison.OrdinalIgnoreCase) && 
        //     int.TryParse(texture.name.Split('_').Last(), out int lastNumber) && 
        //     lastNumber == Globals.GlobalSettings.randomSeed).ToArray();
        //     Debug.Log("Theree are: " + matchingTextures.Length + " matching textures in the directory: " + Directory);

        //     return matchingTextures.Length > 0 ? matchingTextures[UnityEngine.Random.Range(0, matchingTextures.Length)] : null;
        // }

        public static Texture2D GetImageWithIndex(string imageType, int index = -1)
        {
            Texture2D[] allTextures = Resources.LoadAll<Texture2D>(Directory);
            Debug.Log("There are: " + allTextures.Length + " textures in the directory: " + Directory);

            foreach (var tex in allTextures)
            {
                var parts = tex.name.Split('_');
                if (parts.Length < 5) // Adjust the number based on expected parts
                {
                    Debug.LogWarning($"Texture name '{tex.name}' does not have enough parts after splitting.");
                    continue;
                }

                int thisIndex = index>=0?index:Globals.GlobalSettings.randomSeed;

                // Debug.Log($"Last: {parts[parts.Length - 1]} Global: {thisIndex} Split: {parts[1]} ImageType: {imageType}");

                bool isMatchingType = parts[1].Equals(imageType, System.StringComparison.OrdinalIgnoreCase);
                bool lastNumberMatches = int.TryParse(parts[parts.Length - 1], out int lastNumber) && lastNumber == thisIndex;

                if (isMatchingType && lastNumberMatches)
                {
                    // Debug.Log("Matching texture found: " + tex.name);
                    return tex;
                }
                // else{
                //     Debug.Log("No matching texture found: " + " " + imageType + " " + tex.name + " " + Directory);
                // }
            }

            Debug.LogWarning("No matching textures found.");
            return null;
        }


        public static Sprite GetSpriteWithIndexSeed(string imageType, int index = -1, int globalSeed = -1)
        {
            Texture2D tex = GetImageWithIndexSeed(imageType, index, globalSeed);
            if (tex == null)
            {
                tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, Color.white);
                tex.Apply();
            }
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

        public static Texture2D GetImageWithIndexSeed(string imageType, int imageSeed = -1, int globalSeed = -1)
        {
            Texture2D[] textures = Resources.LoadAll<Texture2D>(Directory).Where(t => t.name.Contains(StyleNum) && t.name.Contains(imageType) && t.name.EndsWith("_" + globalSeed.ToString())).ToArray();

            if (textures.Length == 0)
            {
                Debug.LogWarning("No textures found with the given criteria.");
                return null;
            }

            if (imageSeed == -1 || imageSeed >= textures.Length)
            {
                imageSeed = Random.Range(0, textures.Length);
            }

            return textures[imageSeed];
        }

        public static void AssignRandomImage(Renderer renderer, string imageType, bool setMaterial = false)
        {
            string resourcePath = Directory + StyleNum + "/" + imageType + "/";
            Texture2D[] textures = Resources.LoadAll<Texture2D>(resourcePath);

            if (textures.Length > 0)
            {
                Texture2D chosenTexture = textures[Random.Range(0, textures.Length)];
                AssignTextureToRenderer(renderer, chosenTexture, setMaterial);
            }
            else
            {
                Debug.LogWarning("No textures found in the specified Resources directory.");
            }
        }

        public static void AssignImage(Renderer renderer, string imageType, int index, bool setMaterial = false)
        {
            string resourcePath = Directory + StyleNum + "/" + imageType + "/";
            Texture2D[] textures = Resources.LoadAll<Texture2D>(resourcePath);

            if (textures.Length > 0 && index < textures.Length)
            {
                Texture2D chosenTexture = textures[index];
                AssignTextureToRenderer(renderer, chosenTexture, setMaterial);
            }
            else
            {
                Debug.LogWarning("No textures found in the specified Resources directory or index out of range.");
            }
        }

        private static void AssignTextureToRenderer(Renderer renderer, Texture2D texture, bool setMaterial)
        {
            if (renderer is SpriteRenderer spriteRenderer)
            {
                if (!setMaterial)
                    spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                else
                    spriteRenderer.material.SetTexture(TextureChannel, texture);
            }
            else if (renderer is MeshRenderer meshRenderer && meshRenderer.materials.Length > 0)
            {
                meshRenderer.materials[0].SetTexture(TextureChannel, texture);
            }
            else
            {
                Debug.LogWarning("Unsupported renderer type or MeshRenderer has no materials assigned.");
            }
        }

        public static void SetSprite(SpriteRenderer spriteRenderer, string imageType, int index = -1)
        {
            Texture2D chosenTexture = GetImageWithIndex(imageType, index);
            spriteRenderer.sprite = Sprite.Create(chosenTexture, new Rect(0, 0, chosenTexture.width, chosenTexture.height), new Vector2(0.5f, 0.5f));
            spriteRenderer.material.SetTexture(TextureChannel, chosenTexture);
        }

        public static void ApplySelectedFont(TextMeshProUGUI textMeshPro, string resourcePath, int fontIndex)
        {
            TMP_FontAsset[] fonts = Resources.LoadAll<TMP_FontAsset>(resourcePath);
            if (fonts.Length > fontIndex && fontIndex >= 0)
            {
                textMeshPro.font = fonts[fontIndex];
            }
            else
            {
                Debug.LogError("Invalid font index or TMP font not found.");
            }
        }

        public static int GetRandomFontIndex(string resourcePath)
        {
            TMP_FontAsset[] fonts = Resources.LoadAll<TMP_FontAsset>(resourcePath);
            int fontCount = fonts.Length;
            return (fontCount > 0) ? Random.Range(0, fontCount) : -1;
        }
    }
}
