using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Reflection;
using System.Linq;
using AngusChangyiMods;
using AngusChangyiMods.Core;
using AngusChangyiMods.Core.Utils;
using AngusChangyiMods.Unity;

public class Test : MonoBehaviour
{
    public string[] modOrder = new string[0];

    [ShowInInspector]
    [DictionaryDrawerSettings(KeyLabel = "Type")]
    public Dictionary<Type, List<DefBase>> definitions = new Dictionary<Type, List<DefBase>>();

    [ShowInInspector]
    [DictionaryDrawerSettings(KeyLabel = "ID")]
    Dictionary<string, ModMetaData> modMap = new Dictionary<string, ModMetaData>();

    [ShowInInspector]
    [DictionaryDrawerSettings(KeyLabel = "ID")]
    Dictionary<string, object> assets = new Dictionary<string, object>();

    [ShowInInspector]
    [DictionaryDrawerSettings(KeyLabel = "ID")]
    Dictionary<string, IModeSettings> settings = new Dictionary<string, IModeSettings>();

    [ShowInInspector]
    [ListDrawerSettings(ShowFoldout = true)]
    [PropertySpace(SpaceBefore = 10)]
    [InfoBox("統一錯誤管理 - 顯示所有模組載入過程中的錯誤", InfoMessageType.Info)]
    List<ModError> modErrors = new List<ModError>();

    [ShowInInspector]
    [PropertySpace(SpaceBefore = 5)]
    [InfoBox("有錯誤的模組ID列表", InfoMessageType.Warning)]
    List<string> errorModIds = new List<string>();

    [ShowInInspector]
    [PropertySpace(SpaceBefore = 5)]
    [InfoBox("有效的模組載入順序", InfoMessageType.Info)]
    List<string> validModOrder = new List<string>();

    [ShowInInspector]
    [PropertySpace(SpaceBefore = 5)]
    [InfoBox("無效的模組列表", InfoMessageType.Error)]
    List<string> invalidModOrder = new List<string>();

    ModManager modManager;

    void Awake()
    {
        DefinitionDatabase.Clear();
        ModAssetsDatabase.Clear();
        ReflectionUtils.ClearCache();

        ModLogger logger = new ModLogger(new UnityDebugLogger());
    }
    void Start()
    {
        //logger and mod manager setup

        ModFinder modFinder = new ModFinder($"{Application.streamingAssetsPath}/Mods/");
        ModSorter modSorter = new ModSorter();
        ModAssemblyLoader assemblyLoader = new ModAssemblyLoader();
        ModDefinitionLoader definitionLoader = new ModDefinitionLoader();
        ModDefinitionPatcher patcher = new ModDefinitionPatcher();
        ModDefinitionDeserializer deserializer = new ModDefinitionDeserializer();
        ModSettings modSettings = new ModSettings($"{Application.persistentDataPath}/ModSettings/");
        modManager = new ModManager(modFinder, modSorter, assemblyLoader, definitionLoader, patcher, deserializer, modSettings);

        modManager.FindMods();
        modManager.SetModsOrder(modOrder);
        modManager.LoadModsAssemblies();
        modManager.LoadModsDefinition();
        modManager.LoadModsAssets();
        modManager.LoadModSettings();
        modManager.ModsInitialization();

        ModLogger.Log($"============= Game Start =============");
        modManager.GameStart();

        // Inspector display
        ModLogger.Log($"============= Debug Print in Test.cs =============");
        modOrder = modSorter.modOrder;
        modMap = modManager.GetModsMap();

        string checkFilePath = $"{Application.dataPath}/Check.xml";
        XDocument mergeDoc = modManager.GetDefinitionDocument();
        File.WriteAllText(checkFilePath, mergeDoc.PrintAsString());
        Debug.Log(modManager.GetDefinitionDocument().PrintAsString());

        definitions = DefinitionDatabase.GetDefinitions();
        assets = ModAssetsDatabase.GetAssets();
        settings = modSettings.GetSettings();

        // 更新錯誤管理顯示
        modErrors = modManager.GetErrorRecords().ToList();
        errorModIds = modManager.GetErrorModIds().ToList();
        validModOrder = modManager.GetValidModOrder().ToList();
        invalidModOrder = modManager.GetInvalidModOrder().ToList();

        // 輸出詳細的載入統計
        var (total, valid, invalid) = modManager.GetModLoadingStats();
        ModLogger.Log($"============= 模組載入統計 =============");
        ModLogger.Log($"總模組數: {total}");
        ModLogger.Log($"成功載入: {valid}");
        ModLogger.Log($"載入失敗: {invalid}");

        if (invalid > 0)
        {
            string[] invalidMods = modManager.GetInvalidModOrder();
            ModLogger.LogWarning($"失敗的模組: [{string.Join(", ", invalidMods)}]");
        }

        string[] validMods = modManager.GetValidModOrder();
        ModLogger.Log($"有效的模組順序: [{string.Join(", ", validMods)}]");

        // 輸出錯誤統計
        if (modErrors.Count > 0)
        {
            ModLogger.LogWarning($"載入過程中發現 {modErrors.Count} 個錯誤，影響 {errorModIds.Count} 個模組");
            foreach (var errorGroup in modErrors.GroupBy(e => e.ErrorType))
            {
                ModLogger.LogWarning($"  {errorGroup.Key}: {errorGroup.Count()} 個錯誤");
            }
        }
        else
        {
            ModLogger.Log("所有模組載入成功，無錯誤記錄");
        }
        Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        bool alreadyLoaded = loadedAssemblies.Any(a => a.FullName.Contains("ModA"));

        Debug.Log($"ModA 已被自動載入？：{alreadyLoaded}");
    }

    [Button("Test ReflectionUtils Cache Performance")]
    private void TestReflectionCachePerformance()
    {
        ModLogger.Log("============= Testing ReflectionUtils Cache Performance =============");

        // Clear cache first
        AngusChangyiMods.Core.Utils.ReflectionUtils.ClearCache();
        ModLogger.Log("Cache cleared");

        // Test first call (should initialize cache)
        var watch = System.Diagnostics.Stopwatch.StartNew();
        var definitions1 = AngusChangyiMods.Core.Utils.ReflectionUtils.GetTypesAssignableFrom<DefBase>();
        watch.Stop();
        ModLogger.Log($"First call (with cache initialization): {watch.ElapsedMilliseconds}ms, found {definitions1.Count()} types");

        // Test second call (should use cache)
        watch.Restart();
        var definitions2 = AngusChangyiMods.Core.Utils.ReflectionUtils.GetTypesAssignableFrom<DefBase>();
        watch.Stop();
        ModLogger.Log($"Second call (from cache): {watch.ElapsedMilliseconds}ms, found {definitions2.Count()} types");

        // Test third call with different parameters (should compute and cache)
        watch.Restart();
        var definitions3 = AngusChangyiMods.Core.Utils.ReflectionUtils.GetTypesAssignableFrom<DefBase>(includeAbstract: true);
        watch.Stop();
        ModLogger.Log($"Third call (different params, new cache entry): {watch.ElapsedMilliseconds}ms, found {definitions3.Count()} types");

        // Test fourth call with same parameters as third (should use cache)
        watch.Restart();
        var definitions4 = AngusChangyiMods.Core.Utils.ReflectionUtils.GetTypesAssignableFrom<DefBase>(includeAbstract: true);
        watch.Stop();
        ModLogger.Log($"Fourth call (same params as third, from cache): {watch.ElapsedMilliseconds}ms, found {definitions4.Count()} types");

        // Test FindTypeByName
        watch.Restart();
        var testType1 = AngusChangyiMods.Core.Utils.ReflectionUtils.FindTypeByName("TestDefinition");
        watch.Stop();
        ModLogger.Log($"FindTypeByName first call: {watch.ElapsedMilliseconds}ms, found: {testType1?.Name ?? "null"}");

        watch.Restart();
        var testType2 = AngusChangyiMods.Core.Utils.ReflectionUtils.FindTypeByName("TestDefinition");
        watch.Stop();
        ModLogger.Log($"FindTypeByName second call (cached): {watch.ElapsedMilliseconds}ms, found: {testType2?.Name ?? "null"}");

        ModLogger.Log("============= Cache Performance Test Complete =============");
    }

    void OnDestroy()
    {
        if (modManager == null) return;

        modManager.WriteModSettings();
        modManager.UnloadMods();
        ModLogger.Log($"============= Game End =============");
    }

}
