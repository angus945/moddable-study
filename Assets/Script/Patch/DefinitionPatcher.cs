using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using ModdableArchitecture.Utils;



namespace ModdableArchitecture
{
    public class DefinitionPatcher
    {
        ILogger logger;

        private readonly PatchOperationFactory factory = new PatchOperationFactory();

        public DefinitionPatcher(ILogger logger)
        {
            this.logger = logger;
        }
        public void ApplyPatches(string directory, XDocument xmlDocument)
        {
            if (!Directory.Exists(directory))
            {
                logger.LogWarning($"Patch directory does not exist: {directory}");
                return;
            }

            List<IPatchOperation> operations = new List<IPatchOperation>();

            foreach (string file in Directory.GetFiles(directory, "*.xml", SearchOption.AllDirectories))
            {
                XDocument patchDoc = XDocument.Load(file);
                foreach (XElement operationElement in patchDoc.Root.Elements("Operation"))
                {
                    try
                    {
                        IPatchOperation operation = factory.CreateOperation(operationElement);
                        operations.Add(operation);
                    }
                    catch (System.Exception ex)
                    {
                        logger.LogError($"Failed to create patch operation from {file}: {ex.Message}");
                    }
                }
            }

            foreach (IPatchOperation operation in operations)
            {
                operation.Apply(xmlDocument);
            }
        }

    }
}