using System.Xml.Linq;

namespace ModdableArchitecture.Definition.Deserializers
{
    public interface IDefinitionDeserializer
    {
        string HandlesNode { get; }
        Definition Deserialize(XElement element);
    }
}
