using System;
using System.IO;
using System.Xml.Linq;

namespace AngusChangyiMods.Core.DefinitionProcessing
{
    public class DefinitionLoader
    {
        public XDocument LoadDefinition(string loadPath)
        {
            if (!File.Exists((loadPath)))
            {
                throw new FileNotFoundException($"Definition file not found: {loadPath}");
            }
            
            try
            {
                XDocument doc = XDocument.Load(loadPath);
                
                if (doc.Root == null || doc.Root.Name != "Defs")
                {
                    throw new InvalidOperationException($"Invalid definition file format: {loadPath}");
                }
                return doc;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load definition file {loadPath}: {ex.Message}", ex);
            }

            return null;
        }
    }
}