using System.Xml.Linq;

namespace ModArchitecture
{
    public interface IPatchOperation
    {
        void Apply(XDocument doc);
    }
}
