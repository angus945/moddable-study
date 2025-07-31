using System;
using System.Xml.Linq;

namespace AngusChangyiMods.Core.DefinitionProcessing
{
    public interface IDefinitionVarifier
    {
        bool VerifyDefinitions(XElement element);
    }

    public class DefinitionVarifier : IDefinitionVarifier
    {
        private readonly ILogger logger;

        public DefinitionVarifier(ILogger logger)
        {
            this.logger = logger;
        }
        
        public bool VerifyDefinitions(XElement element)
        {
            XElement defNameElement = element.Element(Def.DefName);

            if (defNameElement == null)
            {
                logger.LogError("Definition element is missing the required attribute 'defName'.", "DefinitionVarifier");
                return false;
            }

            string defname = defNameElement.Value;
            if (!System.Text.RegularExpressions.Regex.IsMatch(defname, Def.DefNamePattern))
            {
                logger.LogError($"defName '{defname}' is in illegal format, must be in the format OOO.OOO, and can only contain alphanumeric characters, with the first character being an alphabet letter.", "DefinitionVarifier");
                return false;
            }

            return true;
        }
    }
}