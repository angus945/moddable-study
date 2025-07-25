using System.IO;
using System.Linq;
using System.Xml.Linq;

using ModdableArchitecture.Utils;

public class ModDefinitionLoader
{
    private readonly ILogger _logger;

    public ModDefinitionLoader(ILogger logger)
    {
        _logger = logger;
    }

    public void LoadDefinitions(string[] filePaths, XDocument xmlDocument)
    {
        foreach (var filePath in filePaths)
        {
            LoadDefinition(filePath, xmlDocument);
        }
    }
    public void LoadDefinition(string filePath, XDocument xmlDocument)
    {
        if (!File.Exists(filePath))
        {
            _logger.LogWarning($"Definition file does not exist: {filePath}");
            return;
        }

        _logger.Log($"Loading XML file: {filePath}");
        XDocument tempDoc = XDocument.Load(filePath);
        XElement root = tempDoc.Root;

        if (root == null || root.Name != "Defs") return;

        foreach (XElement element in root.Elements())
        {
            RemoveExisting(xmlDocument, element);
            xmlDocument.Root.Add(element);

            _logger.Log($"Merge Definition: {element.Name} with defID: {element.Element("defID")?.Value}");
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
            _logger.Log($"Overriding Definition: {defType} with defID: {defID}");
            existing.Remove();
        }
    }
}