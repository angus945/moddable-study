using System;
using System.Collections.Generic;
using ModArchitecture.Logger;

public class ModAssetsDatabase
{
    static Dictionary<string, object> assets = new Dictionary<string, object>();

    static string KeyForAsset<T>(string assetId)
    {
        return $"{typeof(T).FullName}:{assetId}";
    }
    public static void RegisterAsset<T>(string assetId, T asset)
    {
        // Implement logic to register the asset with its ID
        string key = KeyForAsset<T>(assetId);
        if (assets.ContainsKey(key))
        {
            ModLogger.Log($"Asset override: {key} already exists, replacing with new asset.");
        }

        assets[key] = asset;
    }
    public static T GetAsset<T>(string assetId) where T : class
    {
        // Implement logic to retrieve the asset by its ID
        string key = KeyForAsset<T>(assetId);
        if (assets.TryGetValue(key, out var asset))
        {
            return asset as T;
        }
        else
        {
            //TODO return error asset
            ModLogger.LogError($"Asset with ID {assetId} not found for type {typeof(T).Name}");
        }
        return null;
    }
    public static void Clear()
    {
        // Implement logic to clear the asset database
        assets.Clear();
    }
    public static Dictionary<string, object> GetAssets()
    {
        // Return a copy of the assets dictionary
        return new Dictionary<string, object>(assets);
    }
}