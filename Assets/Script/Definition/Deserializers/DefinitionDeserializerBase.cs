using System.Xml.Linq;

namespace ModdableArchitecture.Definition.Deserializers
{
    public abstract class DefinitionDeserializerBase<T> : IDefinitionDeserializer where T : Definition, new()
    {
        public abstract string HandlesNode { get; }

        public Definition Deserialize(XElement element)
        {
            T def = new T();
            DeserializeCommon(element, def);
            DeserializeSpecific(element, def);
            return def;
        }

        protected virtual void DeserializeCommon(XElement element, T def)
        {
            def.defID = element.Element("defID")?.Value;
            def.label = element.Element("label")?.Value;
            def.description = element.Element("description")?.Value;
        }

        protected abstract void DeserializeSpecific(XElement element, T def);
    }
}
