using System.Xml.Linq;
using ModdableArchitecture.Definition;
using ModdableArchitecture.Definition.Deserializers;

public class CharacterDefDeserializer : DefinitionDeserializerBase<CharacterDef>
{
    public override string HandlesNode => "CharacterDef";

    protected override void DeserializeSpecific(XElement element, CharacterDef def)
    {
        int.TryParse(element.Element("health")?.Value, out def.health);
        int.TryParse(element.Element("speed")?.Value, out def.speed);
    }
}

