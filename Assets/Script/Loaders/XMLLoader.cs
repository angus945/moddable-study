using System.IO;
using System.Linq;
using System.Xml.Linq;
using ModdableArchitecture.Loaders;

using ModdableArchitecture.Utils;

public class XMLLoader : IXMLLoader
{
    private readonly ILogger _logger;

    public XMLLoader(ILogger logger = null)
    {
        _logger = logger ?? new NullLogger();
    }

    public XDocument LoadAll(string directoryPath)
    {
        XDocument mergeDoc = new XDocument(new XElement("Defs"));
        foreach (string file in Directory.GetFiles(directoryPath, "*.xml", SearchOption.AllDirectories))
        {
            if (Path.GetDirectoryName(file)?.EndsWith("Patch") == true) continue;

            _logger.Log($"Loading XML file: {file}");
            XDocument tempDoc = XDocument.Load(file);
            XElement root = tempDoc.Root;

            if (root == null || root.Name != "Defs") continue;

            foreach (XElement element in root.Elements())
            {
                RemoveExisting(mergeDoc, element);
                mergeDoc.Root.Add(element);

                _logger.Log($"Merge element: {element.Name} with defID: {element.Element("defID")?.Value}");
            }
        }

        return mergeDoc;
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
            _logger.Log($"Overriding definition: {defType} with defID: {defID}");
            existing.Remove();
        }
    }
}
