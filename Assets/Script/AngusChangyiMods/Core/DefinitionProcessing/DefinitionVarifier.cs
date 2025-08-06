using System;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using AngusChangyiMods.Logger;

namespace AngusChangyiMods.Core.DefinitionProcessing
{
    public interface IDefinitionVarifier
    {
        bool VerifyDefinitions(XElement element);
    }

    public class DefinitionVarifier : IDefinitionVarifier
    {
        // === Error Messages ===
        public const string error_lostDefName = "Definition element is missing the required attribute 'defName'.";
        public const string error_illegalDefName = "defName is in illegal format, must be in the format OOO.OOO, and can only contain alphanumeric characters, with the first character being an alphabet letter.";
        public const string error_illegalParent = "parent is in illegal format, must match defName pattern.";
        public const string error_componentMissingClass = "Component <li> must have either 'Class' attribute or <compClass> child.";
        public const string error_extensionMissingClass = "Extension <li> is missing required 'Class' attribute.";

        private readonly ILogger logger;

        public DefinitionVarifier(ILogger logger)
        {
            this.logger = logger;
        }

        public bool VerifyDefinitions(XElement element)
        {
            // defName 驗證
            XElement defNameElement = element.Element(Def.DefName);
            if (defNameElement == null)
            {
                logger.LogError(error_lostDefName);
                return false;
            }

            string defname = defNameElement.Value;
            if (!Regex.IsMatch(defname, Def.DefNamePattern))
            {
                logger.LogError($"{error_illegalDefName} Got: '{defname}'");
                return false;
            }

            // parent 驗證
            XAttribute parentAttr = element.Attribute(Def.Parent);
            if (parentAttr != null && !Regex.IsMatch(parentAttr.Value, Def.DefNamePattern))
            {
                logger.LogError($"{error_illegalParent} Got: '{parentAttr.Value}'");
                return false;
            }

            // Component 驗證
            XElement comps = element.Element(Def.Components);
            if (comps != null)
            {
                foreach (XElement li in comps.Elements(Def.Li))
                {
                    bool hasClassAttr = li.Attribute(Def.Class) != null;
                    bool hasCompClassElement = li.Element("compClass") != null;

                    if (!hasClassAttr && !hasCompClassElement)
                    {
                        logger.LogError(error_componentMissingClass);
                        return false;
                    }
                }
            }

            // Extension 驗證
            XElement exts = element.Element(Def.Extensions);
            if (exts != null)
            {
                foreach (XElement li in exts.Elements(Def.Li))
                {
                    if (li.Attribute(Def.Class) == null)
                    {
                        logger.LogError(error_extensionMissingClass);
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
