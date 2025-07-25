
using System;
using System.IO;
using ModArchitecture.Logger;
using UnityEngine;

namespace ModArchitecture
{

    public class TextureLoader : IAssetLoader
    {
        public string[] HandlesFileExtensions => new[] { ".png" };

        public void LoadAsset(string folderPath, string assetPath)
        {
            LoadTexture(folderPath, assetPath);
        }

        void LoadTexture(string folderPath, string file)
        {
            try
            {
                byte[] imageData = File.ReadAllBytes(file);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(imageData);
                string assetKey = ModAssetLoaderUtils.GetRelativePath(folderPath, file);
                ModAssetsDatabase.RegisterAsset<Texture2D>(assetKey, tex);

                ModLogger.Log($"Loaded texture: {assetKey}");
            }
            catch (Exception e)
            {
                Debug.LogError($"載入圖像失敗: {file} - {e.Message}");
            }
        }
    }
}