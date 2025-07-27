using System;
using System.Xml.Linq;

namespace AngusChangyiMods.Core
{
    public class ModDefinitionProcessor
    {
        ILogger logger;

        ModDefinitionLoader definitionLoader;
        // ModDefinitionInheritor inheritor;
        ModDefinitionPatcher definitionPatcher;
        ModDefinitionDeserializer deserializer;

        XDocument mergeDocument;
        XDocument inheritanceDocument;
        XDocument patchDocument;

        public ModDefinitionProcessor(ILogger logger)
        {
            this.logger = logger;

            definitionLoader = new ModDefinitionLoader(logger);
            // inheritor = new ModDefinitionInheritor();
            definitionPatcher = new ModDefinitionPatcher();
            deserializer = new ModDefinitionDeserializer();

            mergeDocument = new XDocument(new XElement("Defs"));
            inheritanceDocument = new XDocument(new XElement("Defs"));
            patchDocument = new XDocument(new XElement("Defs"));
        }

        // public void LoadModsDefinition()
        // {
        //     ModLogger.Log("Starting to load mod definition files...", "ModManager");
        //     XDocument definitionDocument = new XDocument(new XElement("Defs"));

        //     foreach (var mod in sorter.modOrder)
        //     {
        //         ModMetaData modData = modMap[mod];
        //         try
        //         {
        //             definitionPatcher.ApplyPatches(modData.patches, definitionDocument);
        //             ModLogger.Log($"Definition files loaded successfully: {mod}", "ModManager");
        //         }
        //         catch (Exception ex)
        //         {
        //             AddModError(mod, ModErrorType.AssetLoading, $"Definition files loading failed: {ex.Message}", ex);
        //         }
        //         // TODO Should each mod complete its own patch? Or merge first and then patch once?
        //     }

        //     deserializer.RegisterDeserializers();

        //     var definitions = deserializer.InstanceDefinitions(definitionDocument);

        //     // Process inheritance for all definitions
        //     ModLogger.Log("Starting definition inheritance processing...", "ModManager");
        //     inheritor.RegisterInheritors();
        //     var processedDefinitions = inheritor.ProcessInheritance(definitions);
        //     ModLogger.Log("Definition inheritance processing completed", "ModManager");

        //     DefinitionDatabase.SetDefinitions(processedDefinitions);

        //     loadingRecord.definitionDoc = definitionDocument;
        //     ModLogger.Log("Mod definition files loading completed", "ModManager");
        // }

        public void LoadDefinitions(string[] definitions)
        {
            logger.Log($"Starting to load {definitions.Length} definition files...", "DefinitionLoader");
            int successCount = 0;

            foreach (var filePath in definitions)
            {
                if (definitionLoader.LoadDefinition(filePath, mergeDocument))
                {
                    successCount++;
                }
            }

            logger.Log($"Definition file loading completed, success: {successCount}/{definitions.Length}", "DefinitionLoader");
        }
    }
}
