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

        // 統一錯誤管理
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

            // 初始化需要 ModManager 參考的子系統
            this.assetsLoader = new ModAssetsLoader(this);
            this.initializer = new ModInstancer(this);
        }    // Initialization methods
        public void FindMods()
        {
            ModMetaData[] mods = finder.FindMods();
            foreach (var mod in mods)
            {
                if (!modMap.ContainsKey(mod.id))
                {
                    modMap[mod.id] = mod;
                }
            }
        }
        public void SetModsOrder(string[] order)
        {
            sorter.SetModsOrder(order, modMap);
        }
        public void LoadModsAssemblies()
        {
            foreach (var mod in sorter.modOrder)
            {
                ModMetaData modData = modMap[mod];
                assemblyLoader.LoadModAssembly(modData.assemblies);
            }
        }
        public void LoadModsDefinition()
        {
            XDocument definitionDocument = new XDocument(new XElement("Defs"));

            foreach (var mod in sorter.modOrder)
            {
                ModMetaData modData = modMap[mod];
                definitionLoader.LoadDefinitions(modData.definitions, definitionDocument);
                definitionPatcher.ApplyPatches(modData.patches, definitionDocument);
                // TODO 應該要每個 mod 做完自己的 patch? 還是先合併完最後再一次 patch?
            }

            deserializer.RegisterDeserializers();

            var definitions = deserializer.InstanceDefinitions(definitionDocument);
            DefinitionDatabase.SetDefinitions(definitions);

            loadingRecord.definitionDoc = definitionDocument;
        }
        public void LoadModsAssets()
        {
            assetsLoader.RegisterDeserializers();

            foreach (var mod in sorter.modOrder)
            {
                ModMetaData modData = modMap[mod];
                string modDirectory = modData.directory;

                assetsLoader.LoadAssets(modData.id, Path.Combine(modDirectory, ModStructure.Textures), modData.textures);
                assetsLoader.LoadAssets(modData.id, Path.Combine(modDirectory, ModStructure.Sounds), modData.sounds);
                assetsLoader.LoadAssets(modData.id, Path.Combine(modDirectory, ModStructure.Custom), modData.custom);
            }
        }
        public void LoadModSettings()
        {
            modSettings.RegisterDeserializers();
            modSettings.LoadExistingSettings();

            foreach (var mod in sorter.modOrder)
            {
                ModMetaData modData = modMap[mod];
                modSettings.ReadSettings(modData.id);
            }
        }
        public void ModsInitialization()
        {
            // 第一階段：模組實例化（使用完整的順序）
            string[] allMods = sorter.modOrder;
            initializer.InstanceMod(allMods);

            ModLogger.Log($"實例化階段完成，檢查錯誤狀態...");
            LogErrorSummary();

            // 第二階段：模組初始化（僅使用沒有錯誤的模組）
            string[] validMods = FilterValidMods(allMods);
            if (validMods.Length < allMods.Length)
            {
                ModLogger.LogWarning($"跳過 {allMods.Length - validMods.Length} 個有錯誤的模組進行初始化");
            }

            initializer.InitializeMods(validMods);

            ModLogger.Log($"初始化階段完成，檢查錯誤狀態...");
            LogErrorSummary();
        }

        public void GameStart()
        {
            // 遊戲啟動（僅使用沒有錯誤的模組）
            string[] validMods = GetValidModOrder();
            if (validMods.Length < sorter.modOrder.Length)
            {
                ModLogger.LogWarning($"跳過 {sorter.modOrder.Length - validMods.Length} 個有錯誤的模組進行遊戲啟動");
            }

            initializer.StartGame(validMods);

            ModLogger.Log($"遊戲啟動階段完成，檢查錯誤狀態...");
            LogErrorSummary();
        }

        /// <summary>
        /// 輸出錯誤摘要
        /// </summary>
        private void LogErrorSummary()
        {
            if (errorRecords.Count > 0)
            {
                var errorGroups = errorRecords.GroupBy(e => e.ErrorType);
                foreach (var group in errorGroups)
                {
                    ModLogger.LogWarning($"  {group.Key} 錯誤: {group.Count()} 個");
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

        // 統一錯誤管理方法
        /// <summary>
        /// 新增模組錯誤記錄
        /// </summary>
        public void AddModError(string modId, ModErrorType errorType, string message, Exception exception = null)
        {
            errorModIds.Add(modId);
            errorRecords.Add(new ModError(modId, errorType, message, exception));
            ModLogger.LogError($"[{errorType}] {modId}: {message}");
        }

        /// <summary>
        /// 檢查指定模組是否有錯誤
        /// </summary>
        public bool HasModError(string modId)
        {
            return errorModIds.Contains(modId);
        }

        /// <summary>
        /// 取得所有錯誤記錄
        /// </summary>
        public IReadOnlyList<ModError> GetErrorRecords()
        {
            return errorRecords.AsReadOnly();
        }

        /// <summary>
        /// 取得有錯誤的模組ID集合
        /// </summary>
        public IReadOnlyCollection<string> GetErrorModIds()
        {
            return errorModIds;
        }

        /// <summary>
        /// 取得指定模組的錯誤記錄
        /// </summary>
        public IEnumerable<ModError> GetModErrors(string modId)
        {
            return errorRecords.Where(e => e.ModId == modId);
        }

        /// <summary>
        /// 取得指定錯誤類型的錯誤記錄
        /// </summary>
        public IEnumerable<ModError> GetErrorsByType(ModErrorType errorType)
        {
            return errorRecords.Where(e => e.ErrorType == errorType);
        }

        /// <summary>
        /// 從給定的模組順序中過濾掉有錯誤的模組
        /// </summary>
        private string[] FilterValidMods(string[] modOrder)
        {
            return modOrder.Where(modId => !HasModError(modId)).ToArray();
        }

        /// <summary>
        /// 取得有效模組列表（沒有錯誤的模組）
        /// </summary>
        public string[] GetValidModOrder()
        {
            return FilterValidMods(sorter.modOrder);
        }

        /// <summary>
        /// 取得無效模組列表（有錯誤的模組）
        /// </summary>
        public string[] GetInvalidModOrder()
        {
            return sorter.modOrder.Where(modId => HasModError(modId)).ToArray();
        }

        /// <summary>
        /// 取得模組載入統計資訊
        /// </summary>
        public (int total, int valid, int invalid) GetModLoadingStats()
        {
            int total = sorter.modOrder.Length;
            int invalid = errorModIds.Count;
            int valid = total - invalid;
            return (total, valid, invalid);
        }

        /// <summary>
        /// 清除所有錯誤記錄
        /// </summary>
        public void ClearErrors()
        {
            errorModIds.Clear();
            errorRecords.Clear();
        }
    }
}
