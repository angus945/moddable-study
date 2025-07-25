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
    public List<string> modOrder = new List<string>();

    [ShowInInspector]
    [DictionaryDrawerSettings(KeyLabel = "Type")]
    public Dictionary<Type, List<Definition>> definitions = new Dictionary<Type, List<Definition>>();

    [ShowInInspector]
    [DictionaryDrawerSettings(KeyLabel = "ID")]
    Dictionary<string, ModMetaData> modMap = new Dictionary<string, ModMetaData>();

    void Awake()
    {
        DefinitionDatabase.Clear();

        ModLogger logger = new ModLogger(new UnityDebugLogger());
    }
    void Start()
    {
        //logger and mod manager setup

        ModFinder modFinder = new ModFinder($"{Application.streamingAssetsPath}/Mods/");
        ModSorter modSorter = new ModSorter();
        ModDefinitionLoader definitionLoader = new ModDefinitionLoader();
        ModDefinitionPatcher patcher = new ModDefinitionPatcher();
        ModDefinitionDeserializer deserializer = new ModDefinitionDeserializer();
        ModInitializer initializer = new ModInitializer();
        ModManager modManager = new ModManager(modFinder, modSorter, definitionLoader, patcher, deserializer, initializer);

        modManager.FindMods();
        modManager.SetModsOrder(modOrder);
        modManager.LoadModsAssemblies();
        modManager.LoadModsDefinition();
        modManager.ModsInitialization();

        // Inspector display
        modOrder = modSorter.modOrder;
        modMap = modManager.GetModsMap();

        string checkFilePath = $"{Application.dataPath}/Check.xml";
        XDocument mergeDoc = modManager.GetDefinitionDocument();
        File.WriteAllText(checkFilePath, mergeDoc.PrintAsString());
        Debug.Log(modManager.GetDefinitionDocument().PrintAsString());


        definitions = DefinitionDatabase.GetDefinitions();
        LogDefinitions(definitions);

        Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        bool alreadyLoaded = loadedAssemblies.Any(a => a.FullName.Contains("ModA"));

        Debug.Log($"ModA 已被自動載入？：{alreadyLoaded}");
    }

    void LogDefinitions(Dictionary<Type, List<Definition>> definitions)
    {
        ModLogger.Log("--- Instantiated Definitions ---");
        foreach (var kvp in definitions)
        {
            ModLogger.Log($"Type: {kvp.Key.Name} ({kvp.Value.Count} instances)");
            foreach (var def in kvp.Value)
            {
                if (def is ThingDef thingDef)
                {
                    ModLogger.Log($"  - defID: {thingDef.defID}, Label: {thingDef.label}, Damage: {thingDef.damage}, Tags: {(thingDef.tags != null ? string.Join(", ", thingDef.tags) : "None")}, Weapon Range: {thingDef.weaponProps?.range}");
                }
                else if (def is CharacterDef charDef)
                {
                    ModLogger.Log($"  - defID: {charDef.defID}, Label: {charDef.label}, Health: {charDef.health}, Speed: {charDef.speed}");
                }
                else
                {
                    ModLogger.Log($"  - defID: {def.defID}, Label: {def.label}");
                }
            }
        }
        ModLogger.Log("--------------------------------");
    }
}
