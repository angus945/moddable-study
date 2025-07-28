using System.Linq;
using System.Xml.Linq;

namespace AngusChangyiMods.Core.DefinitionProcessing.Test
{
    public class XMLUtil
    {
        public static XElement ParseFirstElement(string content)
        {
            XDocument document = XDocument.Parse(content);
            return document.Root.Elements().First(); // 正確：取 <Defs> 底下第一個子元素
        }

        public static XDocument DefinitionBase => new XDocument(new XElement(Def.Root));
    }
}