using System;
using System.IO;
using System.Xml.Linq;
using AngusChangyiMods.Logger;

namespace AngusChangyiMods.Core.DefinitionProcessing
{
    public interface IDefinitionLoader
    {
        XDocument LoadDefinition(string loadPath, string sourceMod);
    }

    public class DefinitionLoader : IDefinitionLoader
    {
        public const string warningFileNotFound = "Definition file not found";
        public const string errorInvalidFormat = "Invalid definition file format";
        public const string infoSuccessfullyLoaded = "Successfully loaded definition from";
        public const string errorXmlParse = "XML parse error in";
        public const string errorUnexpected = "Unexpected error loading";
        
        private readonly ILogger logger;

        public DefinitionLoader(ILogger logger)
        {
            this.logger = logger;
        }

        public XDocument LoadDefinition(string loadPath, string sourceMod)
        {
            if (!File.Exists(loadPath))
            {
                logger.LogWarning($"{warningFileNotFound}: {loadPath}");
                return null;
            }

            try
            {
                XDocument doc = XDocument.Load(loadPath);

                if (doc.Root == null || doc.Root.Name != Def.Root)
                {
                    logger.LogError($"{errorInvalidFormat}: {loadPath}");
                    return null;
                }

                foreach (XElement defElement in doc.Root.Elements())
                {
                    defElement.Add(new XElement(Def.SourceFile, loadPath));
                    defElement.Add(new XElement(Def.SourceMod, sourceMod));
                }

                logger.LogInfo($"{infoSuccessfullyLoaded}: {loadPath}");
                return doc;
            }
            catch (System.Xml.XmlException ex)
            {
                logger.LogError($"{errorXmlParse} {loadPath}: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError($"{errorUnexpected} {loadPath}: {ex.Message}");
                return null;
            }
        }
    }

}