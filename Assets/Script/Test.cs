using System.IO;
using System.Xml.Linq;
using UnityEngine;
using ModdableArchitecture;
using ModdableArchitecture.Utils;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;
using ModdableArchitecture.Definition;

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
    }
    void Start()
    {
        //logger and mod manager setup
        ModdableArchitecture.Utils.ILogger logger = new UnityDebugLogger();

        ModFinder modFinder = new ModFinder(logger, $"{Application.streamingAssetsPath}/Mods/");
        ModSorter modSorter = new ModSorter();
        ModDefinitionLoader definitionLoader = new ModDefinitionLoader(logger);
        ModDefinitionPatcher patcher = new ModDefinitionPatcher(logger);
        ModDefinitionDeserializer deserializer = new ModDefinitionDeserializer(logger);
        ModManager modManager = new ModManager(modFinder, modSorter, definitionLoader, patcher, deserializer);

        modManager.FindMods();
        modManager.SetModsOrder(modOrder);
        modManager.LoadModsDefinition();

        // Inspector display
        modOrder = modSorter.modOrder;
        modMap = modManager.GetModsMap();

        string checkFilePath = $"{Application.dataPath}/Check.xml";
        XDocument mergeDoc = modManager.GetDefinitionDocument();
        File.WriteAllText(checkFilePath, mergeDoc.PrintAsString());
        Debug.Log(modManager.GetDefinitionDocument().PrintAsString());


        definitions = DefinitionDatabase.GetDefinitions();
        LogDefinitions(definitions, logger);
    }

    void LogDefinitions(Dictionary<Type, List<Definition>> definitions, ModdableArchitecture.Utils.ILogger logger)
    {
        logger.Log("--- Instantiated Definitions ---");
        foreach (var kvp in definitions)
        {
            logger.Log($"Type: {kvp.Key.Name} ({kvp.Value.Count} instances)");
            foreach (var def in kvp.Value)
            {
                if (def is ThingDef thingDef)
                {
                    logger.Log($"  - defID: {thingDef.defID}, Label: {thingDef.label}, Damage: {thingDef.damage}, Tags: {(thingDef.tags != null ? string.Join(", ", thingDef.tags) : "None")}, Weapon Range: {thingDef.weaponProps?.range}");
                }
                else if (def is CharacterDef charDef)
                {
                    logger.Log($"  - defID: {charDef.defID}, Label: {charDef.label}, Health: {charDef.health}, Speed: {charDef.speed}");
                }
                else
                {
                    logger.Log($"  - defID: {def.defID}, Label: {def.label}");
                }
            }
        }
        logger.Log("--------------------------------");
    }
}
