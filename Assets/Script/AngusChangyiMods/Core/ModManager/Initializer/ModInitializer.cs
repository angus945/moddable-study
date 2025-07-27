
using System;
using System.Collections.Generic;
using System.Linq;
using AngusChangyiMods.Core.Utils;

namespace AngusChangyiMods.Core
{
    public class ModInstancer
    {
        readonly Dictionary<string, IModEntry> modInstances = new Dictionary<string, IModEntry>();
        private ModManager modManager;

        public ModInstancer(ModManager modManager)
        {
            this.modManager = modManager;
        }

        public void InstanceMod(string[] order)
        {
            ModLogger.Log($"Starting instantiation of {order.Length} mods...", "ModInstancer");
            IEnumerable<Type> instanceTypes = ReflectionUtils.GetTypesAssignableFrom<IModEntry>();
            Dictionary<string, Type> typeMap = instanceTypes.ToDictionary(type => type.FullName, type => type);

            foreach (var modId in order)
            {
                string expectedEntryClass = $"{modId}Entry";

                if (typeMap.TryGetValue(expectedEntryClass, out Type type))
                {
                    if (modInstances.ContainsKey(type.FullName))
                    {
                        ModLogger.LogError($"Mod instance {type.FullName} already exists. registration will be overwritten.", "ModInstancer");
                    }

                    IModEntry instance = ReflectionUtils.SafeCreateInstance<IModEntry>(type);
                    if (instance != null)
                    {
                        modInstances[type.FullName] = instance;
                        ModLogger.Log($"Mod instantiation successful: {type.FullName}", "ModInstancer");
                    }
                    else
                    {
                        modManager.AddModError(modId, ModErrorType.Instancing, $"Failed to create instance for mod entry: {type.FullName}");
                    }
                }
                else
                {
                    modManager.AddModError(modId, ModErrorType.Instancing, $"Mod entry for {modId} not found. Expected class ending with '{expectedEntryClass}'. Skipping registration.");
                }


            }
            ModLogger.Log($"Mod instantiation completed, success: {modInstances.Count}", "ModInstancer");
        }
        public void InitializeMods(string[] order)
        {
            ModLogger.Log($"Starting initialization of {order.Length} mods...", "ModInstancer");
            int successCount = 0;

            foreach (var modId in order)
            {
                string expectedEntryClass = $"{modId}Entry";
                if (!modInstances.TryGetValue(expectedEntryClass, out IModEntry instance))
                {
                    modManager.AddModError(modId, ModErrorType.Initialization, $"Mod entry for {modId} not found. Expected class ending with '{expectedEntryClass}'. Skipping initialization.");
                    continue;
                }

                try
                {
                    instance.Initialize();
                    ModLogger.Log($"Mod initialization successful: {instance.GetType().FullName}", "ModInstancer");
                    successCount++;
                }
                catch (Exception ex)
                {
                    modManager.AddModError(modId, ModErrorType.Initialization, $"Error initializing mod {modId}: {ex.Message}", ex);
                    continue;
                }
            }
            ModLogger.Log($"Mod initialization completed, success: {successCount}/{order.Length}", "ModInstancer");
        }
        internal void StartGame(string[] order)
        {
            ModLogger.Log($"Starting game start for {order.Length} mods...", "ModInstancer");
            int successCount = 0;

            foreach (var modId in order)
            {
                // 根據 modId 找到對應的 Entry 類別完整名稱
                string expectedEntryClass = $"{modId}Entry";
                if (!modInstances.TryGetValue(expectedEntryClass, out IModEntry instance))
                {
                    modManager.AddModError(modId, ModErrorType.GameStart, $"Mod entry for {modId} not found. Expected class ending with '{expectedEntryClass}'. Skipping game start.");
                    continue;
                }

                try
                {
                    instance.OnGameStart();
                    ModLogger.Log($"Mod game start successful: {instance.GetType().FullName}", "ModInstancer");
                    successCount++;
                }
                catch (Exception ex)
                {
                    modManager.AddModError(modId, ModErrorType.GameStart, $"Error starting game for mod {modId}: {ex.Message}", ex);
                    continue;
                }
            }
            ModLogger.Log($"Game start completed, success: {successCount}/{order.Length}", "ModInstancer");
        }

    }
}