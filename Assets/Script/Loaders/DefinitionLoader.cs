using System.IO;
using System.Linq;
using System.Xml.Linq;

using ModdableArchitecture.Utils;

public class DefinitionLoader
{
    private readonly ILogger _logger;

    public DefinitionLoader(ILogger logger)
    {
        _logger = logger;
    }

    public void LoadDirectory(string directoryPath, XDocument xmlDocument)
    {
        if (!Directory.Exists(directoryPath))
        {
            _logger.LogWarning($"Definition directory does not exist: {directoryPath}");
            return;
        }

        foreach (string file in Directory.GetFiles(directoryPath, "*.xml", SearchOption.AllDirectories))
        {
            _logger.Log($"Loading XML file: {file}");
            XDocument tempDoc = XDocument.Load(file);
            XElement root = tempDoc.Root;

            if (root == null || root.Name != "Defs") continue;

            foreach (XElement element in root.Elements())
            {
                RemoveExisting(xmlDocument, element);
                xmlDocument.Root.Add(element);

                _logger.Log($"Merge Definition: {element.Name} with defID: {element.Element("defID")?.Value}");
            }
        }
    }
    void RemoveExisting(XDocument mergeDoc, XElement newElement)
    {
        string defID = newElement.Element("defID")?.Value;
        string defType = newElement.Name.LocalName;

        if (string.IsNullOrEmpty(defID)) return;

        XElement? existing = mergeDoc.Root
            .Elements(defType)
            .FirstOrDefault(e => e.Element("defID")?.Value == defID);

        if (existing != null)
        {
            _logger.Log($"Overriding Definition: {defType} with defID: {defID}");
            existing.Remove();
        }
    }
}
