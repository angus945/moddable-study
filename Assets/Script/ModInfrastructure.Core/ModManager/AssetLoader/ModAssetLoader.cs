
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
                        if (!assetLoaders.ContainsKey(extension))
                        {
                            assetLoaders.Add(extension, deserializer);
                            ModLogger.Log($"Registered asset loader: {type.Name} for file <{extension}>");
                        }
                        else
                        {
                            ModLogger.LogWarning($"Asset loader for extension {extension} is already registered, skipping {type.Name}");
                        }
                    }
                }
            }
        }
        public void LoadAssets(string modDirectory, string[] paths)
        {
            foreach (var path in paths)
            {
                // Load asset from the given path
                string extension = Path.GetExtension(path);
                if (assetLoaders.TryGetValue(extension, out var loader))
                {
                    loader.LoadAsset(modDirectory, path);
                }
                else
                {
                    ModLogger.LogWarning($"No asset loader registered for extension: {extension}");
                }
            }
        }

    }
}