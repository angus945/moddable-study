using System.Xml.Linq;
using AngusChangyiMods.Core;

namespace Angus
{
    /// <summary>
    /// Deserializer for CharacterDef definitions.
    /// </summary>
    /// <remarks>
    /// This class handles the deserialization of CharacterDef XML elements into CharacterDef objects.
    /// </remarks>
    public class CharacterDefDeserializer : DefinitionDeserializerBase<CharacterDef>
    {
        public override string HandlesNode => "CharacterDef";

        protected override void DeserializeSpecific(XElement element, CharacterDef def)
        {
            int.TryParse(element.Element("health")?.Value, out def.health);
            int.TryParse(element.Element("speed")?.Value, out def.speed);
        }
    }

}