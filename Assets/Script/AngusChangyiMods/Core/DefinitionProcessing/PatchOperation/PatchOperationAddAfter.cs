using System.Xml.Linq;
using System.Xml.XPath;

namespace AngusChangyiMods.Core.DefinitionProcessing.PatchOperation
{
    public class PatchOperationAddAfter : IPatchOperation
    {
        public string xpath;
        public XElement value;

        public void Apply(XDocument doc)
        {
            foreach (XElement element in doc.XPathSelectElements(xpath))
            {
                element.AddAfterSelf(new XElement(value));
            }
        }
    }
}
