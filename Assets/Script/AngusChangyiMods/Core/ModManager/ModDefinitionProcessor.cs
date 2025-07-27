using System;
using System.Xml.Linq;

namespace AngusChangyiMods.Core
{

    public class ModDefinitionProcessor
    {
        ILogger logger;

        ModDefinitionLoader definitionLoader;
        ModDefinitionInheritor inheritor;
        ModDefinitionPatcher definitionPatcher;
        ModDefinitionDeserializer deserializer;

        XDocument mergeDocument;
        XDocument inheritanceDocument;
        XDocument patchDocument;

        public ModDefinitionProcessor(ILogger logger)
        {
            this.logger = logger;

            definitionLoader = new ModDefinitionLoader(logger);
            inheritor = new ModDefinitionInheritor(logger);
            definitionPatcher = new ModDefinitionPatcher();
            deserializer = new ModDefinitionDeserializer();

            mergeDocument = new XDocument(new XElement("Defs"));
            inheritanceDocument = new XDocument(new XElement("Defs"));
            patchDocument = new XDocument(new XElement("Defs"));
        }
        public void LoadDefinitions(string[] definitions)
        {
            int successCount = 0;

            foreach (var filePath in definitions)
            {
                if (definitionLoader.LoadDefinition(filePath, mergeDocument))
                {
                    successCount++;
                }
            }
        }
        public void ProcessInheritance()
        {
            inheritanceDocument = inheritor.ProcessInheritance(mergeDocument);

            patchDocument = new XDocument(inheritanceDocument);
        }
        public void ProcessPatches(string[] patches)
        {
            foreach (var patchPath in patches)
            {
                definitionPatcher.ApplyPatch(patchPath, patchDocument);
            }
        }

        public void GetProcessedDefinitions(out XDocument merge, out XDocument inheritance, out XDocument patch)
        {
            merge = mergeDocument;
            inheritance = inheritanceDocument;
            patch = patchDocument;
        }
    }
}
