using System.IO;
using System.Xml.Linq;
using UnityEngine;
using ModdableArchitecture;
using ModdableArchitecture.Utils;
using System.Collections.Generic;

public class Test : MonoBehaviour
{
    public List<string> modOrder = new List<string>();

    void Start()
    {
        // 1. Setup the logger and the core loader
        ModdableArchitecture.Utils.ILogger logger = new UnityDebugLogger();

        ModManager modManager = new ModManager(logger);
        modManager.LoadModsInfo($"{Application.streamingAssetsPath}/Mods/");
        modManager.SetModsOrder(modOrder);
        modOrder = modManager.modIDs;

        //
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

        // 5. Save the final result for verification
        string checkFilePath = $"{Application.dataPath}/Check.xml";
        File.WriteAllText(checkFilePath, mergeDoc.PrintAsString());
        Debug.Log($"Final XML saved to {checkFilePath}");
    }
}
