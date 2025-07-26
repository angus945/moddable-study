using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ModArchitecture.Logger;


namespace ModArchitecture
{
    public class ModDefinitionLoader
    {
        public void LoadDefinitions(string[] filePaths, XDocument xmlDocument)
        {
            ModLogger.Log($"Starting to load {filePaths.Length} definition files...", "DefinitionLoader");
            int successCount = 0;

            foreach (var filePath in filePaths)
            {
                if (LoadDefinition(filePath, xmlDocument))
                {
                    successCount++;
                }
            }

            ModLogger.Log($"Definition file loading completed, success: {successCount}/{filePaths.Length}", "DefinitionLoader");
        }
        public bool LoadDefinition(string filePath, XDocument xmlDocument)
        {
            if (!File.Exists(filePath))
            {
                ModLogger.LogWarning($"Definition file does not exist: {filePath}", "DefinitionLoader");
                return false;
            }

            try
            {
                ModLogger.Log($"Loading definition file: {Path.GetFileName(filePath)}", "DefinitionLoader");
                XDocument tempDoc = XDocument.Load(filePath);
                XElement root = tempDoc.Root;

                if (root == null || root.Name != "Defs")
                {
                    ModLogger.LogWarning($"Invalid definition file format: {filePath}", "DefinitionLoader");
                    return false;
                }

                int elementCount = 0;
                foreach (XElement element in root.Elements())
                {
                    RemoveExisting(xmlDocument, element);
                    xmlDocument.Root.Add(element);
                    elementCount++;

                    ModLogger.Log($"Merged definition: {element.Name} with defID: {element.Element("defID")?.Value}", "DefinitionLoader");
                }

                ModLogger.Log($"File {Path.GetFileName(filePath)} loaded successfully, merged {elementCount} definitions", "DefinitionLoader");
                return true;
            }
            catch (Exception ex)
            {
                ModLogger.LogError($"Failed to load definition file {filePath}: {ex.Message}", "DefinitionLoader");
                return false;
            }
        }
        void RemoveExisting(XDocument mergeDoc, XElement newElement)
        {
            string defID = newElement.Element("defID")?.Value;
            string defType = newElement.Name.LocalName;

            if (string.IsNullOrEmpty(defID)) return;

            XElement existing = mergeDoc.Root
                .Elements(defType)
                .FirstOrDefault(e => e.Element("defID")?.Value == defID);

            if (existing != null)
            {
                ModLogger.Log($"Overriding Definition: {defType} with defID: {defID}");
                existing.Remove();
            }
        }
    }
}