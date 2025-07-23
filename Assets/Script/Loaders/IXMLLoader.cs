using System.Xml.Linq;

namespace ModdableArchitecture.Loaders
{
    public interface IXMLLoader
    {
        XDocument LoadAll(string directoryPath);
    }
}
