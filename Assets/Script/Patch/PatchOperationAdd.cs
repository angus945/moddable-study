using System.Xml.Linq;
using System.Xml.XPath;

namespace ModdableArchitecture
{
    public class PatchOperationAdd : IPatchOperation
    {
        public string xpath;
        public XElement value;
        public bool prepend = false;

        public void Apply(XDocument doc)
        {
            foreach (XElement element in doc.XPathSelectElements(xpath))
            {
                if (prepend)
                {
                    element.AddFirst(new XElement(value));
                }
                else
                {
                    element.AddAfterSelf(new XElement(value));
                }
            }
        }
    }
}
