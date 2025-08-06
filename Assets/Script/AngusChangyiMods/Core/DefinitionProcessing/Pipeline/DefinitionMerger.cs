using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AngusChangyiMods.Logger;

namespace AngusChangyiMods.Core.DefinitionProcessing
{
    public interface IDefinitionMerger
    {
        bool MergeDefinitions(XDocument source, XDocument mergeTo);
    }

    public class DefinitionMerger : IDefinitionMerger
    {
        private readonly IDefinitionVarifier verifier;
        private readonly ILogger logger;

        public DefinitionMerger(IDefinitionVarifier verifier, ILogger logger)
        {
            this.verifier = verifier;
            this.logger = logger;
        }

        public bool MergeDefinitions(XDocument source, XDocument mergeTo)
        {
            if (mergeTo == null)
            {
                logger.LogError("Merge target document is null");
                return false;
            }

            if (source == null || source.Root == null || source.Root.Name != XDef.Root)
            {
                logger.LogError($"Invalid source document: {source?.ToString() ?? "null"}");
                return false;
            }

            bool hasErrors = false;
            int processedCount = 0;
            int overrideCount = 0;

            foreach (XElement element in source.Root.Elements())
            {
                if (verifier.VerifyDefinitions(element))
                {
                    if (RemoveExisting(element, mergeTo, out string overrideType, out string overrideName))
                    {
                        logger.LogWarning($"Override Definition, Type {overrideType}, DefName {overrideName}");
                        overrideCount++;
                    }

                    mergeTo.Root.Add(element);
                    processedCount++;
                }
                else
                {
                    logger.LogError($"Invalid definition {element.Name}: verification failed");
                    hasErrors = true;
                }
            }

            logger.LogInfo($"Merge completed: {processedCount} definitions processed, {overrideCount} overrides");
            return !hasErrors;
        }

        private bool RemoveExisting(XElement source, XDocument removeTarget, out string overrideType, out string overrideName)
        {
            string defName = source.Element(XDef.DefName)?.Value;
            string defType = source.Name.LocalName;

            if (string.IsNullOrEmpty(defName))
            {
                overrideType = defType;
                overrideName = defName ?? "Unknown";
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