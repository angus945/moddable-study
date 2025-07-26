using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;
using UnityEngine;
using ModArchitecture;
using ModArchitecture.Definition;
using System.Reflection;
using System.Linq;
using ModArchitecture.Logger;

public class Test : MonoBehaviour
{
    public string[] modOrder = new string[0];

    [ShowInInspector]
    [DictionaryDrawerSettings(KeyLabel = "Type")]
    public Dictionary<Type, List<Definition>> definitions = new Dictionary<Type, List<Definition>>();

    [ShowInInspector]
    [DictionaryDrawerSettings(KeyLabel = "ID")]
    Dictionary<string, ModMetaData> modMap = new Dictionary<string, ModMetaData>();

    [ShowInInspector]
    [DictionaryDrawerSettings(KeyLabel = "ID")]
    Dictionary<string, object> assets = new Dictionary<string, object>();

    ModManager modManager;

    void Awake()
    {
        DefinitionDatabase.Clear();
        ModAssetsDatabase.Clear();

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
        ModInstancer initializer = new ModInstancer();
        ModAssetsLoader assetsLoader = new ModAssetsLoader();
        modManager = new ModManager(modFinder, modSorter, assemblyLoader, definitionLoader, patcher, deserializer, assetsLoader, initializer);

        modManager.FindMods();
        modManager.SetModsOrder(modOrder);
        modManager.LoadModsAssemblies();
        modManager.LoadModsDefinition();
        modManager.LoadModsAssets();
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

        Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        bool alreadyLoaded = loadedAssemblies.Any(a => a.FullName.Contains("ModA"));

        Debug.Log($"ModA 已被自動載入？：{alreadyLoaded}");
    }
    void OnDestroy()
    {
        if (modManager == null) return;

        modManager.UnloadMods();
        ModLogger.Log($"============= Game End =============");
    }

}
