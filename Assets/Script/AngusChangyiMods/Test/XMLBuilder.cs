using System.Xml.Linq;

namespace AngusChangyiMods.Core.Test
{
    public static class XMLBuilder
    {
        public static XDocument CreateSimpleDocument(string rootName, params (string name, string value)[] elements)
        {
            var root = new XElement(rootName);
            foreach (var (name, value) in elements)
            {
                root.Add(new XElement(name, value));
            }
            return new XDocument(root);
        }

        public static XDocument CreateNestedDocument(string rootName, string childName, params (string name, string value)[] childElements)
        {
            var root = new XElement(rootName);
            var child = new XElement(childName);

            foreach (var (name, value) in childElements)
            {
                child.Add(new XElement(name, value));
            }

            root.Add(child);
            return new XDocument(root);
        }

        public static XDocument CreateComplexDocument(string rootName,
            string groupName = null,
            params (string elementName, string id, string content)[] items)
        {
            var root = new XElement(rootName);

            if (!string.IsNullOrEmpty(groupName))
            {
                var group = new XElement(groupName);

                foreach (var (elementName, id, content) in items)
                {
                    var element = new XElement(elementName);
                    if (!string.IsNullOrEmpty(id))
                    {
                        element.Add(new XElement("defID", id));
                    }
                    if (!string.IsNullOrEmpty(content))
                    {
                        element.Add(new XElement("element", content));
                    }
                    group.Add(element);
                }

                root.Add(group);
            }

            return new XDocument(root);
        }
    }
}
