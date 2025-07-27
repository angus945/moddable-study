using System.Xml.Linq;

namespace AngusChangyiMods.Core
{
    public interface IDefinitionDeserializer
    {
        string HandlesNode { get; }
        DefBase Deserialize(XElement element);
    }
}
