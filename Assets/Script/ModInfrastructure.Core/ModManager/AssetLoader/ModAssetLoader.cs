
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
        private readonly HashSet<string> errorExtensions = new HashSet<string>(); // Record problematic file extensions to avoid duplicate errors
        private ModManager modManager;

        public ModAssetsLoader(ModManager modManager)
        {
            this.modManager = modManager;
        }

        public void RegisterDeserializers()
        {
            ModLogger.Log("Starting asset loader registration...", "AssetLoader");
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
                            ModLogger.LogWarning($"Asset loader for extension {extension} is already registered, overwriting {type.Name}", "AssetLoader");
                        }

                        assetLoaders[extension] = deserializer;
                        ModLogger.Log($"Asset loader registration successful: {type.Name} for file <{extension}>", "AssetLoader");
                    }
                }
            }
            ModLogger.Log($"Asset loader registration completed, total: {assetLoaders.Count}", "AssetLoader");
        }
        public void LoadAssets(string modId, string modDirectory, string[] paths)
        {
            if (paths.Length == 0) return;

            ModLogger.Log($"Starting to load {paths.Length} asset files for mod {modId}...", "AssetLoader");
            int successCount = 0;
            int skipCount = 0;

            foreach (var path in paths)
            {
                string extension = Path.GetExtension(path);

                if (errorExtensions.Contains(extension))
                {
                    skipCount++;
                    continue;
                }

                // Load asset from the given path
                if (assetLoaders.TryGetValue(extension, out var loader))
                {
                    try
                    {
                        loader.LoadAsset(modDirectory, path);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        modManager.AddModError(modId, ModErrorType.AssetLoading, $"Error loading asset {path}: {ex.Message}", ex);
                    }
                }
                else
                {
                    // Record error extension to avoid duplicate error reporting
                    if (!errorExtensions.Contains(extension))
                    {
                        errorExtensions.Add(extension);
                        ModLogger.LogError($"No asset loader registered for extension: {extension}", "AssetLoader");
                    }
                    skipCount++;
                }
            }

            ModLogger.Log($"Mod {modId} asset loading completed, success: {successCount}, skipped: {skipCount}", "AssetLoader");
        }

    }
}