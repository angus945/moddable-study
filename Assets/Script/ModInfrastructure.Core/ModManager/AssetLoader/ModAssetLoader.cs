
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
        private readonly HashSet<string> errorSet = new HashSet<string>();

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
        public void LoadAssets(string modDirectory, string[] paths)
        {
            foreach (var path in paths)
            {
                string extension = Path.GetExtension(path);

                if (errorSet.Contains(extension)) continue;

                // Load asset from the given path
                if (assetLoaders.TryGetValue(extension, out var loader))
                {
                    loader.LoadAsset(modDirectory, path);
                }
                else
                {
                    ModLogger.LogError($"No asset loader registered for extension: {extension}");
                    errorSet.Add(extension);
                }
            }
        }

    }
}