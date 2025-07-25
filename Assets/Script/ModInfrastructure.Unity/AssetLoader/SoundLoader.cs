using System.Threading.Tasks;
using ModArchitecture.Logger;
using UnityEngine;

namespace ModArchitecture
{
    // TODO 有時載入後不會正確寫入 AssetsDatabase，可能是因為 UnityWebRequest 的異步特性
    public class SoundLoader : IAssetLoader
    {
        public string[] HandlesFileExtensions => new[] { ".ogg", ".wav", ".mp3" };

        public async void LoadAsset(string folderPath, string soundPath)
        {
            await LoadSoundsAsync(folderPath, soundPath);
        }
        public async Task LoadSoundsAsync(string folder, string file)
        {
            string url = "file://" + file;
            using (var request = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip(url, AudioType.UNKNOWN))
            {
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    AudioClip clip = UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(request);
                    ModLogger.Log($"Loaded sound: {clip}");
                    string assetKey = ModAssetLoaderUtils.GetRelativePath(folder, file);
                    ModAssetsDatabase.RegisterAsset<AudioClip>(assetKey, clip);
                }
                else
                {
                    ModLogger.LogError($"載入音效失敗: {file} - {request.error}");
                }
            }
        }
    }
}