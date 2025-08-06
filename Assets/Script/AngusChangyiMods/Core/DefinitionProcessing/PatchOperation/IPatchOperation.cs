using System.Xml.Linq;

namespace AngusChangyiMods.Core.DefinitionProcessing.PatchOperation
{
    public interface IPatchOperation
    {
        void Apply(XDocument doc);
    }
}
