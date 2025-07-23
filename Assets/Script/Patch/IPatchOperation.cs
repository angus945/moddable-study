using System.Xml.Linq;

namespace ModdableArchitecture
{
    public interface IPatchOperation
    {
        void Apply(XDocument doc);
    }
}
