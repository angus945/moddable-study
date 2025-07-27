namespace AngusChangyiMods.Core
{
    public interface IAssetLoader
    {
        string[] HandlesFileExtensions { get; }

        void LoadAsset(string folderPath, string assetPath);
    }
}