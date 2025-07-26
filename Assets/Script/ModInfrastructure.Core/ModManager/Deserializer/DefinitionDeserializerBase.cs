using System.Xml.Linq;

namespace ModArchitecture.Definition.Deserializers
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

            XAttribute parentAttr = element.Attribute("inheritsFrom");
            if (parentAttr != null)
            {
                def.inheritsFrom = parentAttr.Value;
            }

            XAttribute abstractAttr = element.Attribute("isAbstract");
            if (abstractAttr != null)
            {
                def.IsAbstract = bool.Parse(abstractAttr.Value);
            }
        }

        protected abstract void DeserializeSpecific(XElement element, T def);
    }
}
