using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace ModdableArchitecture
{
    public class PatchService
    {
        private readonly PatchOperationFactory factory = new PatchOperationFactory();

        public void ApplyPatches(string directoryPath, XDocument doc)
        {
            List<IPatchOperation> operations = new List<IPatchOperation>();

            foreach (string file in Directory.GetFiles(directoryPath, "*.xml", SearchOption.AllDirectories))
            {
                if (Path.GetDirectoryName(file)?.EndsWith("Patch") != true) continue;

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
                        UnityEngine.Debug.LogError($"Failed to create patch operation from {file}: {ex.Message}");
                    }
                }
            }

            foreach (IPatchOperation operation in operations)
            {
                operation.Apply(doc);
            }
        }
    }
}
