using System.Xml.Linq;
using System.Xml.XPath;

namespace AngusChangyiMods.Core.DefinitionProcessing.PatchOperation
{
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
