using System;
using System.IO;
using System.Xml.Linq;
using AngusChangyiMods.Logger;

namespace AngusChangyiMods.Core.DefinitionProcessing
{
    public interface IDefinitionLoader
    {
        XDocument LoadDefinition(string loadPath);
    }

    public class DefinitionLoader : IDefinitionLoader
    {
        private readonly ILogger logger;

        public DefinitionLoader(ILogger logger)
        {
            this.logger = logger;
        }

        public XDocument LoadDefinition(string loadPath)
        {
            if (!File.Exists(loadPath))
            {
                logger.LogWarning($"Definition file not found: {loadPath}");
                return null;
            }

            try
            {
                XDocument doc = XDocument.Load(loadPath);

                if (doc.Root == null || doc.Root.Name != Def.Root)
                {
                    logger.LogError($"Invalid definition file format: {loadPath}");
                    return null;
                }

                logger.LogInfo($"Successfully loaded definition from: {loadPath}");
                return doc;
            }
            catch (System.Xml.XmlException ex)
            {
                logger.LogError($"XML parse error in {loadPath}: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError($"Unexpected error loading {loadPath}: {ex.Message}");
                return null;
            }
        }
    }

}