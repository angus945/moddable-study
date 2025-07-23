using System.Xml.Linq;
using UnityEngine;

public static class XDocumentExtensions
{
    public static string PrintAsString(this XDocument doc)
    {
        if (doc == null || doc.Root == null)
        {            
            return "Document or root is null.";
        }

        return doc.ToString(SaveOptions.OmitDuplicateNamespaces);
    }
}
