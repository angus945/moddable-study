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

    void Start()
    {
        //logger and mod manager setup
        ModdableArchitecture.Utils.ILogger logger = new UnityDebugLogger();

        ModManager modManager = new ModManager(logger);
        modManager.LoadModsInfo($"{Application.streamingAssetsPath}/Mods/");
        modManager.SetModsOrder(modOrder);
        modOrder = modManager.modIDs;

        // definition load
        DefinitionLoader definitionLoader = new DefinitionLoader(logger);
        DefinitionPatcher patchService = new DefinitionPatcher(logger);

        XDocument mergeDoc = new XDocument(new XElement("Defs"));
        foreach (string modId in modManager.modIDs)
        {
            string modPath = modManager.GetModDirectory(modId);
            string defPath = Path.Combine(modPath, ModStructure.Defs);
            definitionLoader.LoadDirectory(defPath, mergeDoc);

            string patchPath = Path.Combine(modPath, ModStructure.Patches);
            patchService.ApplyPatches(patchPath, mergeDoc);
        }
        // TODO 應該要每個 mod 做完自己的 patch? 還是先合併完最後再一次 patch?

        // Save the final result for verification
        string checkFilePath = $"{Application.dataPath}/Check.xml";
        File.WriteAllText(checkFilePath, mergeDoc.PrintAsString());
        logger.Log($"Final XML saved to {checkFilePath}");

        // Deserialize definitions
        DefinitionInstanter deserializer = new DefinitionInstanter(logger);
        definitions = deserializer.InstanceDefinitions(mergeDoc);

        DefinitionDatabase definitionDatabase = new DefinitionDatabase();
        definitionDatabase.AddDefinition(definitions);

        // Log instantiated definitions for verification
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
