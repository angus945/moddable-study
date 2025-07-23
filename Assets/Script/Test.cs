using System.IO;
using System.Xml.Linq;
using UnityEngine;
using ModdableArchitecture;
using ModdableArchitecture.Utils;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;

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
        Debug.Log($"Final XML saved to {checkFilePath}");

        // Deserialize definitions
        DefinitionInstanter deserializer = new DefinitionInstanter(logger);
        definitions = deserializer.InstanceDefinitions(mergeDoc);

        DefinitionDatabase definitionDatabase = new DefinitionDatabase();
        definitionDatabase.AddDefinition(definitions);
    }
}
