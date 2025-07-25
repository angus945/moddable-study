using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using ModArchitecture.Utils;



namespace ModArchitecture
{
    public class ModDefinitionPatcher
    {
        ILogger logger;

        private readonly PatchOperationFactory factory = new PatchOperationFactory();

        public ModDefinitionPatcher(ILogger logger)
        {
            this.logger = logger;
        }
        public void ApplyPatches(string[] patchPaths, XDocument xmlDocument)
        {
            foreach (string patchPath in patchPaths)
            {
                ApplyPatch(patchPath, xmlDocument);
            }
        }
        public void ApplyPatch(string filePath, XDocument xmlDocument)
        {
            XDocument patchDoc = XDocument.Load(filePath);
            foreach (XElement operationElement in patchDoc.Root.Elements("Operation"))
            {
                try
                {
                    IPatchOperation operation = factory.CreateOperation(operationElement);
                    operation.Apply(xmlDocument);
                }
                catch (System.Exception ex)
                {
                    logger.LogError($"Failed to create patch operation from {filePath}: {ex.Message}");
                }
            }
        }
    }
}