using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public static class XDocumentExtensions
{
    public static string PrintAsString(this XDocument doc)
    {
        if (doc == null || doc.Root == null)
        {
            Debug.Log("Document or root is null.");
            return null;
        }

        string xmlString = doc.ToString(SaveOptions.OmitDuplicateNamespaces);
        Debug.Log(xmlString);
        return xmlString;
    }
}

public class XMLLoader
{
    // public void Load(string filePath)
    // {
    //     XDocument doc = XDocument.Load(filePath);
    //     XElement root = doc.Root;
    //     foreach (XElement element in root.Elements())
    //     {
    //         string defType = element.Name.LocalName;
    //         Debug.Log($"Loading definition of type: {defType}");
    //     }
    // }
    public XDocument LoadAll(string directoryPath)
    {
        XDocument mergeDoc = new XDocument(new XElement("Defs"));
        foreach (string file in Directory.GetFiles(directoryPath, "*.xml", SearchOption.AllDirectories))
        {
            XDocument tempDoc = XDocument.Load(file);
            XElement root = tempDoc.Root;
            Debug.Log($"Loading XML file: {file}");

            if (root == null || root.Name != "Defs") continue;

            foreach (XElement element in root.Elements())
            {
                RemoveExisting(mergeDoc, element);

                mergeDoc.Root.Add(element);
            }
        }

        return mergeDoc;
    }

    // Overridden
    void RemoveExisting(XDocument mergeDoc, XElement newElement)
    {
        string defID = newElement.Element("defID")?.Value;
        string defType = newElement.Name.LocalName;
        XElement? existing = mergeDoc.Root
            .Elements(defType)
            .FirstOrDefault(e => e.Element("defID")?.Value == defID);

        if (existing != null)
        {
            existing.Remove();
            Debug.Log($"Removed existing definition: {defType} with defID: {defID}");
        }
        else
        {
            Debug.Log($"No existing definition found for: {defType} with defID: {defID}");
        }
    }

    public XDocument Patch(string directoryPath, XDocument doc)
    {
        List<PatchOperation> operations = new List<PatchOperation>();

        foreach (string file in Directory.GetFiles(directoryPath, "*.xml", SearchOption.AllDirectories))
        {
            if (Path.GetDirectoryName(file)?.EndsWith("Patch") != true) continue;

            XDocument patchDoc = XDocument.Load(file);
            foreach (XElement operationElement in patchDoc.Root.Elements("Operation"))
            {
                string className = operationElement.Attribute("Class")?.Value;
                string xpath = operationElement.Element("xpath")?.Value;
                XElement valueElement = operationElement.Element("value")?.FirstNode as XElement;

                if (className == "PatchOperationReplace")
                {
                    PatchOperationReplace operation = new PatchOperationReplace
                    {
                        xpath = xpath,
                        value = valueElement
                    };
                    operations.Add(operation);
                }
                else if (className == "PatchOperationAdd")
                {
                    PatchOperationAdd operation = new PatchOperationAdd
                    {
                        xpath = xpath,
                        value = valueElement,
                        prepend = operationElement.Element("prepend")?.Value == "true"
                    };
                    operations.Add(operation);
                }
            }
        }

        foreach (PatchOperation operation in operations)
        {
            operation.Apply(doc);
        }

        return doc;
    }
}