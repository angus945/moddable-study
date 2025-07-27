using System.Xml.Linq;
using System.Xml.XPath;

namespace AngusChangyiMods.Core
{
    public class PatchOperationAddAfter : IPatchOperation
    {
        public string xpath;
        public XElement value;
        public bool prepend = false;

        public void Apply(XDocument doc)
        {
            foreach (XElement element in doc.XPathSelectElements(xpath))
            {
                element.AddAfterSelf(new XElement(value));
            }
        }
    }

    public class PatchOperationInsert : IPatchOperation
    {
        public string xpath;
        public XElement value;

        public void Apply(XDocument doc)
        {
            var targetElement = doc.XPathSelectElement(xpath);

            if (targetElement != null)
            {
                targetElement.Add(value);
            }
        }
    }
}
