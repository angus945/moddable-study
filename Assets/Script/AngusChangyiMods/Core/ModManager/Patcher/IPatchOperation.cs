using System.Xml.Linq;

namespace AngusChangyiMods.Core
{
    public interface IPatchOperation
    {
        void Apply(XDocument doc);
    }
}
