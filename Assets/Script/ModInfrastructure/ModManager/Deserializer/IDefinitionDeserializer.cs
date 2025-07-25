using System.Xml.Linq;

namespace ModArchitecture.Definition.Deserializers
{
    public interface IDefinitionDeserializer
    {
        string HandlesNode { get; }
        Definition Deserialize(XElement element);
    }
}
