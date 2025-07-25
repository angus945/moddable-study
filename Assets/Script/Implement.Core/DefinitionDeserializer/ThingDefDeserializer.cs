using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ModArchitecture.Definition.Deserializers;

public class ThingDefDeserializer : DefinitionDeserializerBase<ThingDef>
{
    public override string HandlesNode => "ThingDef";

    protected override void DeserializeSpecific(XElement element, ThingDef def)
    {
        int.TryParse(element.Element("damage")?.Value, out def.damage);
        int.TryParse(element.Element("stack")?.Value, out def.stack);

        def.tags = element.Element("tags")?.Elements("tag").Select(e => e.Value).ToList() ?? new List<string>();

        XElement weaponPropsElement = element.Element("weaponProps");
        if (weaponPropsElement != null)
        {
            def.weaponProps = new WeaponProperties();
            def.weaponProps.type = weaponPropsElement.Element("type")?.Value;
            int.TryParse(weaponPropsElement.Element("damage")?.Value, out def.weaponProps.damage);
            float.TryParse(weaponPropsElement.Element("range")?.Value, out def.weaponProps.range);
        }
    }
}

