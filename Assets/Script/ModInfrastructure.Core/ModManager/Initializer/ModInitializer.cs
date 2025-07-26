
using System;
using System.Collections.Generic;
using System.Linq;
using ModArchitecture.Logger;
using ModArchitecture.Utils;

namespace ModArchitecture
{
    public class ModInstancer
    {
        readonly Dictionary<string, IModEntry> modInstances = new Dictionary<string, IModEntry>();
        HashSet<string> errorSet = new HashSet<string>();

        public void InstanceMod(string[] order)
        {
            IEnumerable<Type> instanceTypes = ReflectionUtils.GetTypesAssignableFrom<IModEntry>();
            Dictionary<string, Type> typeMap = instanceTypes.ToDictionary(type => type.FullName, type => type);

            foreach (var modId in order)
            {
                string expectedEntryClass = $"{modId}Entry";

                if (typeMap.TryGetValue(expectedEntryClass, out Type type))
                {
                    if (modInstances.ContainsKey(type.FullName))
                    {
                        ModLogger.LogError($"Mod instance {type.FullName} already exists. registration will be overwritten.");
                    }

                    IModEntry instance = ReflectionUtils.SafeCreateInstance<IModEntry>(type);
                    if (instance != null)
                    {
                        modInstances[type.FullName] = instance;
                        ModLogger.Log($"Registered mod instance: {type.FullName}");
                    }
                    else
                    {
                        errorSet.Add(modId);
                        ModLogger.LogError($"Failed to create instance for mod entry: {type.FullName}");
                    }
                }
                else
                {
                    errorSet.Add(modId);
                    ModLogger.LogError($"Mod entry for {modId} not found. Expected class ending with '{expectedEntryClass}'. Skipping registration.");
                }


            }
        }
        public void InitializeMods(string[] order)
        {
            foreach (var modId in order)
            {
                if (errorSet.Contains(modId))
                {
                    ModLogger.LogWarning($"Mod ID {modId} has errors. Skipping initialization.");
                    continue;
                }

                string expectedEntryClass = $"{modId}Entry";
                if (!modInstances.TryGetValue(expectedEntryClass, out IModEntry instance))
                {
                    ModLogger.LogError($"Mod entry for {modId} not found. Expected class ending with '{expectedEntryClass}'. Skipping initialization.");
                    errorSet.Add(modId);
                    continue;
                }

                try
                {
                    instance.Initialize();
                    ModLogger.Log($"Initializing mod: {instance.GetType().FullName}");
                }
                catch (Exception ex)
                {
                    ModLogger.LogError($"Error initializing mod {modId}: {ex.Message}");
                    errorSet.Add(modId);
                    continue;
                }
            }
        }
        internal void StartGame(string[] order)
        {
            foreach (var modId in order)
            {
                if (errorSet.Contains(modId))
                {
                    ModLogger.LogWarning($"Mod ID {modId} has errors. Skipping game start.");
                    continue;
                }
                // 根據 modId 找到對應的 Entry 類別完整名稱
                string expectedEntryClass = $"{modId}Entry";
                if (!modInstances.TryGetValue(expectedEntryClass, out IModEntry instance))
                {
                    ModLogger.LogError($"Mod entry for {modId} not found. Expected class ending with '{expectedEntryClass}'. Skipping game start.");
                    errorSet.Add(modId);
                    continue;
                }

                try
                {
                    instance.OnGameStart();
                    ModLogger.Log($"Starting game for mod: {instance.GetType().FullName}");
                }
                catch (Exception ex)
                {
                    ModLogger.LogError($"Error starting game for mod {modId}: {ex.Message}");
                    errorSet.Add(modId);
                    continue;
                }
            }
        }

    }
}