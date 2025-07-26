
using System;
using System.Collections.Generic;
using System.IO;
using ModArchitecture.Logger;
using ModArchitecture.Utils;

namespace ModArchitecture
{

    public class ModAssetsLoader
    {
        private readonly Dictionary<string, IAssetLoader> assetLoaders = new Dictionary<string, IAssetLoader>();
        private readonly HashSet<string> errorExtensions = new HashSet<string>(); // 記錄有問題的檔案副檔名，避免重複錯誤
        private ModManager modManager;

        public ModAssetsLoader(ModManager modManager)
        {
            this.modManager = modManager;
        }

        public void RegisterDeserializers()
        {
            var deserializerTypes = ReflectionUtils.GetTypesAssignableFrom<IAssetLoader>();

            foreach (var type in deserializerTypes)
            {
                var deserializer = ReflectionUtils.SafeCreateInstance<IAssetLoader>(type);
                if (deserializer != null)
                {
                    foreach (var extension in deserializer.HandlesFileExtensions)
                    {
                        if (assetLoaders.ContainsKey(extension))
                        {
                            ModLogger.LogWarning($"Asset loader for extension {extension} is already registered, overwriting {type.Name}");
                        }

                        assetLoaders[extension] = deserializer;
                        ModLogger.Log($"Registered asset loader: {type.Name} for file <{extension}>");
                    }
                }
            }
        }
        public void LoadAssets(string modId, string modDirectory, string[] paths)
        {
            foreach (var path in paths)
            {
                string extension = Path.GetExtension(path);

                if (errorExtensions.Contains(extension)) continue;

                // Load asset from the given path
                if (assetLoaders.TryGetValue(extension, out var loader))
                {
                    try
                    {
                        loader.LoadAsset(modDirectory, path);
                    }
                    catch (Exception ex)
                    {
                        modManager.AddModError(modId, ModErrorType.AssetLoading, $"Error loading asset {path}: {ex.Message}", ex);
                    }
                }
                else
                {
                    // 記錄錯誤擴展名，避免重複報錯
                    if (!errorExtensions.Contains(extension))
                    {
                        errorExtensions.Add(extension);
                        ModLogger.LogError($"No asset loader registered for extension: {extension}");
                    }
                }
            }
        }

    }
}