using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ModArchitecture.Logger;

namespace ModArchitecture
{

    public class ModLoadingRecord
    {
        public XDocument definitionDoc;
    }

    public class ModManager
    {
        Dictionary<string, ModMetaData> modMap = new Dictionary<string, ModMetaData>();

        // Unified error management
        private readonly HashSet<string> errorModIds = new HashSet<string>();
        private readonly List<ModError> errorRecords = new List<ModError>();

        ModFinder finder;
        ModSorter sorter;
        ModAssemblyLoader assemblyLoader;
        ModDefinitionLoader definitionLoader;
        ModDefinitionPatcher definitionPatcher;
        ModDefinitionDeserializer deserializer;
        ModAssetsLoader assetsLoader;
        ModSettings modSettings;
        ModInstancer initializer;

        ModLoadingRecord loadingRecord = new ModLoadingRecord();

        public ModManager(ModFinder finder, ModSorter sorter, ModAssemblyLoader assemblyLoader,
            ModDefinitionLoader definitionLoader, ModDefinitionPatcher patcher, ModDefinitionDeserializer deserializer,
            ModSettings modSettings)
        {
            this.finder = finder;
            this.sorter = sorter;
            this.assemblyLoader = assemblyLoader;
            this.definitionLoader = definitionLoader;
            this.definitionPatcher = patcher;
            this.deserializer = deserializer;
            this.modSettings = modSettings;

            // Initialize subsystems that require ModManager reference
            this.assetsLoader = new ModAssetsLoader(this);
            this.initializer = new ModInstancer(this);
        }

        // Initialization methods
        public void FindMods()
        {
            ModLogger.Log("Starting to search for mods...", "ModManager");
            ModMetaData[] mods = finder.FindMods();
            foreach (var mod in mods)
            {
                if (!modMap.ContainsKey(mod.id))
                {
                    modMap[mod.id] = mod;
                }
            }
            ModLogger.Log($"Mod search completed, found {mods.Length} mods", "ModManager");
        }
        public void SetModsOrder(string[] order)
        {
            ModLogger.Log($"Setting mod loading order, total {order.Length} mods", "ModManager");
            sorter.SetModsOrder(order, modMap);
        }
        public void LoadModsAssemblies()
        {
            ModLogger.Log("Starting to load mod assemblies...", "ModManager");
            foreach (var mod in sorter.modOrder)
            {
                ModMetaData modData = modMap[mod];
                try
                {
                    assemblyLoader.LoadModAssembly(modData.assemblies);
                    ModLogger.Log($"Assembly loaded successfully: {mod}", "ModManager");
                }
                catch (Exception ex)
                {
                    AddModError(mod, ModErrorType.AssetLoading, $"Assembly loading failed: {ex.Message}", ex);
                }
            }
            ModLogger.Log("Mod assemblies loading completed", "ModManager");
        }
        public void LoadModsDefinition()
        {
            ModLogger.Log("Starting to load mod definition files...", "ModManager");
            XDocument definitionDocument = new XDocument(new XElement("Defs"));

            foreach (var mod in sorter.modOrder)
            {
                ModMetaData modData = modMap[mod];
                try
                {
                    definitionLoader.LoadDefinitions(modData.definitions, definitionDocument);
                    definitionPatcher.ApplyPatches(modData.patches, definitionDocument);
                    ModLogger.Log($"Definition files loaded successfully: {mod}", "ModManager");
                }
                catch (Exception ex)
                {
                    AddModError(mod, ModErrorType.AssetLoading, $"Definition files loading failed: {ex.Message}", ex);
                }
                // TODO Should each mod complete its own patch? Or merge first and then patch once?
            }

            deserializer.RegisterDeserializers();

            var definitions = deserializer.InstanceDefinitions(definitionDocument);
            DefinitionDatabase.SetDefinitions(definitions);

            loadingRecord.definitionDoc = definitionDocument;
            ModLogger.Log("Mod definition files loading completed", "ModManager");
        }
        public void LoadModsAssets()
        {
            ModLogger.Log("Starting to load mod assets...", "ModManager");
            assetsLoader.RegisterDeserializers();

            foreach (var mod in sorter.modOrder)
            {
                ModMetaData modData = modMap[mod];
                string modDirectory = modData.directory;

                assetsLoader.LoadAssets(modData.id, Path.Combine(modDirectory, ModStructure.Textures), modData.textures);
                assetsLoader.LoadAssets(modData.id, Path.Combine(modDirectory, ModStructure.Sounds), modData.sounds);
                assetsLoader.LoadAssets(modData.id, Path.Combine(modDirectory, ModStructure.Custom), modData.custom);
            }
            ModLogger.Log("Mod assets loading completed", "ModManager");
        }
        public void LoadModSettings()
        {
            ModLogger.Log("Starting to load mod settings...", "ModManager");
            modSettings.RegisterDeserializers();
            modSettings.LoadExistingSettings();

            foreach (var mod in sorter.modOrder)
            {
                ModMetaData modData = modMap[mod];
                modSettings.ReadSettings(modData.id);
            }
            ModLogger.Log("Mod settings loading completed", "ModManager");
        }
        public void ModsInitialization()
        {
            // Phase 1: Mod instantiation (use complete order)
            string[] allMods = sorter.modOrder;
            ModLogger.Log($"Starting mod instantiation phase, total {allMods.Length} mods", "ModManager");
            initializer.InstanceMod(allMods);

            ModLogger.Log($"Instantiation phase completed, checking error status...", "ModManager");
            LogErrorSummary("ModManager");

            // Phase 2: Mod initialization (use only mods without errors)
            string[] validMods = FilterValidMods(allMods);
            if (validMods.Length < allMods.Length)
            {
                ModLogger.LogWarning($"Skipping {allMods.Length - validMods.Length} mods with errors for initialization", "ModManager");
            }

            ModLogger.Log($"Starting mod initialization phase, total {validMods.Length} mods", "ModManager");
            initializer.InitializeMods(validMods);

            ModLogger.Log($"Initialization phase completed, checking error status...", "ModManager");
            LogErrorSummary("ModManager");
        }

        public void GameStart()
        {
            // Game start (use only mods without errors)
            string[] validMods = GetValidModOrder();
            if (validMods.Length < sorter.modOrder.Length)
            {
                ModLogger.LogWarning($"Skipping {sorter.modOrder.Length - validMods.Length} mods with errors for game start", "ModManager");
            }

            ModLogger.Log($"Starting game start phase, total {validMods.Length} mods", "ModManager");
            initializer.StartGame(validMods);

            ModLogger.Log($"Game start phase completed, checking error status...", "ModManager");
            LogErrorSummary("ModManager");
        }

        /// <summary>
        /// Output error summary
        /// </summary>
        private void LogErrorSummary(string tag = null)
        {
            if (errorRecords.Count > 0)
            {
                var errorGroups = errorRecords.GroupBy(e => e.ErrorType);
                foreach (var group in errorGroups)
                {
                    ModLogger.LogWarning($"  {group.Key} errors: {group.Count()} items", tag);
                }
            }
        }
        public void WriteModSettings()
        {
            foreach (var mod in sorter.modOrder)
            {
                ModMetaData modData = modMap[mod];
                modSettings.WriteSettings(modData.id);
            }
        }

        //
        public void UnloadMods()
        {
            // assemblyLoader.UnloadAssembly();

        }

        //
        public Dictionary<string, ModMetaData> GetModsMap()
        {
            return modMap;
        }
        public XDocument GetDefinitionDocument()
        {
            return loadingRecord.definitionDoc;
        }

        // Unified error management methods
        /// <summary>
        /// Add mod error record
        /// </summary>
        public void AddModError(string modId, ModErrorType errorType, string message, Exception exception = null)
        {
            errorModIds.Add(modId);
            errorRecords.Add(new ModError(modId, errorType, message, exception));
            ModLogger.LogError($"[{errorType}] {modId}: {message}", "ModManager");
        }

        /// <summary>
        /// Check if specified mod has errors
        /// </summary>
        public bool HasModError(string modId)
        {
            return errorModIds.Contains(modId);
        }

        /// <summary>
        /// Get all error records
        /// </summary>
        public IReadOnlyList<ModError> GetErrorRecords()
        {
            return errorRecords.AsReadOnly();
        }

        /// <summary>
        /// Get error mod IDs collection
        /// </summary>
        public IReadOnlyCollection<string> GetErrorModIds()
        {
            return errorModIds;
        }

        /// <summary>
        /// Get error records for specified mod
        /// </summary>
        public IEnumerable<ModError> GetModErrors(string modId)
        {
            return errorRecords.Where(e => e.ModId == modId);
        }

        /// <summary>
        /// Get error records by specified error type
        /// </summary>
        public IEnumerable<ModError> GetErrorsByType(ModErrorType errorType)
        {
            return errorRecords.Where(e => e.ErrorType == errorType);
        }

        /// <summary>
        /// Filter out mods with errors from given mod order
        /// </summary>
        private string[] FilterValidMods(string[] modOrder)
        {
            return modOrder.Where(modId => !HasModError(modId)).ToArray();
        }

        /// <summary>
        /// Get valid mod list (mods without errors)
        /// </summary>
        public string[] GetValidModOrder()
        {
            return FilterValidMods(sorter.modOrder);
        }

        /// <summary>
        /// Get invalid mod list (mods with errors)
        /// </summary>
        public string[] GetInvalidModOrder()
        {
            return sorter.modOrder.Where(modId => HasModError(modId)).ToArray();
        }

        /// <summary>
        /// Get mod loading statistics
        /// </summary>
        public (int total, int valid, int invalid) GetModLoadingStats()
        {
            int total = sorter.modOrder.Length;
            int invalid = errorModIds.Count;
            int valid = total - invalid;
            return (total, valid, invalid);
        }

        /// <summary>
        /// Clear all error records
        /// </summary>
        public void ClearErrors()
        {
            errorModIds.Clear();
            errorRecords.Clear();
        }
    }
}
