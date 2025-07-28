using System;
using System.IO;
using System.Xml.Linq;

namespace AngusChangyiMods.Core.DefinitionProcessing
{
    public interface IDefinitionLoader
    {
        bool LoadDefinition(string loadPath, out XDocument loaded, out string errorMessage);
    }

    public class DefinitionLoader : IDefinitionLoader
    {
        public bool LoadDefinition(string loadPath, out XDocument loaded, out string errorMessage)
        {
            loaded = null;
            errorMessage = null;

            if (!File.Exists(loadPath))
            {
                errorMessage = $"Definition file not found: {loadPath}";
                return false;
            }

            try
            {
                XDocument doc = XDocument.Load(loadPath);

                if (doc.Root == null || doc.Root.Name != Def.Root)
                {
                    errorMessage = $"Invalid definition file format: {loadPath}";
                    return false;
                }

                loaded = doc;
                return true;
            }
            catch (System.Xml.XmlException ex)
            {
                errorMessage = $"XML parse error: {ex.Message}";
                return false;
            }
        }
    }

}