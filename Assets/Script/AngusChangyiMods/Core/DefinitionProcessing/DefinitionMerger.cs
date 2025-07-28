using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;


namespace AngusChangyiMods.Core.DefinitionProcessing
{
    public interface IDefinitionMerger
    {
        bool MergeDefinitions(XDocument source, XDocument mergeTo, out string errorMessage);
    }

    public class DefinitionMerger
    {
        public const string error_mergeTargetNull = "Merge Target is null";
        public const string error_sourceInvalid = "Invalid source document: {0}";
        public const string error_invalidDefinition = "Invalid definition {0}: {1} \n";
        public const string error_overrideDefinition = "Override Definition, Type {0}, DefName {1} \n";

        private IDefinitionVarifier verifier;

        public DefinitionMerger(IDefinitionVarifier verifier)
        {
            this.verifier = verifier;
        }

        public bool MergeDefinitions(XDocument source, XDocument mergeTo, out string errorMessage)
        {
            if (mergeTo == null)
            {
                errorMessage = error_mergeTargetNull;
                return false;
            }

            if (source == null || source.Root == null || source.Root.Name != Def.Root)
            {
                errorMessage = string.Format(error_sourceInvalid, source?.ToString() ?? "null");
                return false;
            }

            errorMessage = "";
            foreach (XElement element in source.Root.Elements())
            {
                if (verifier.VerifyDefinitions(element, out string verifyError))
                {
                    if (RemoveExisting(element, mergeTo, out string overrideType, out string overrideName))
                    {
                        errorMessage += string.Format(error_overrideDefinition, overrideType, overrideName);
                    }

                    mergeTo.Root.Add(element);
                }
                else
                {
                    errorMessage += string.Format(error_invalidDefinition, element.Name, verifyError);
                    continue;
                }
            }

            return true;
        }

        bool RemoveExisting(XElement source, XDocument removeTarget, out string overrideType, out string overrideName)
        {
            string defName = source.Element(Def.DefName)?.Value;
            string defType = source.Name.LocalName;

            if (string.IsNullOrEmpty(defName))
            {
                overrideType = defType;
                overrideName = defName;
                return false;
            }

            IEnumerable<XElement> existing = removeTarget.Root
                .Elements(defType)
                .Where(e => e.Element("defName")?.Value == defName)
                .ToList();

            foreach (XElement existingElement in existing)
            {
                existingElement.Remove();
            }

            overrideType = defType;
            overrideName = defName;
            return existing.Any();
        }
    }
}