using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using AngusChangyiMods.Core.DefinitionProcessing.PatchOperation;
using AngusChangyiMods.Logger;


namespace AngusChangyiMods.Core.DefinitionProcessing
{
    public interface IDefinitionPatcher
    {
        public void ApplyPatch(string filePath, XDocument xmlDocument);
    }
    public class ModDefinitionPatcher : IDefinitionPatcher
    {
        private readonly PatchOperationFactory factory = new PatchOperationFactory();

        ILogger logger;
        IXMLoader patchLoader;

        public ModDefinitionPatcher(ILogger logger, IXMLoader patchLoader)
        {
            this.logger = logger;
            this.patchLoader = patchLoader;
        }

        public void ApplyPatch(string filePath, XDocument xmlDocument)
        {
            XDocument patchDoc = patchLoader.LoadXML(filePath, XPatch.Root);
            foreach (XElement operationElement in patchDoc.Root.Elements(XPatch.Operation))
            {
                try
                {
                    IPatchOperation operation = factory.CreateOperation(operationElement);
                    operation.Apply(xmlDocument);
                }
                catch (System.Exception ex)
                {
                    logger.LogError($"Failed to create patch operation from {filePath}, failed patch: {operationElement}, \n {ex.Message}");
                }
            }
        }


    }
}