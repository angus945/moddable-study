using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;


namespace AngusChangyiMods.Core.DefinitionProcessing
{
    public class DefinitionMerger
    {
        public void MergeDefinitions(XDocument source, XDocument mergeTo)
        {
            if (mergeTo == null)
                throw new ArgumentNullException($"Merge to is null");

            if (source == null || source.Root == null || source.Root.Name != "Defs")
                throw new ArgumentException($"Invalid definition document: {source?.Root?.Name}");
            
            foreach (XElement element in source.Elements())
            {
                RemoveExisting(element, mergeTo);
                
                mergeTo.Root.Add(element);
            }            
        }
        
        void RemoveExisting(XElement source, XDocument removeTarget)
        {
            string defName = source.Element("defID")?.Value;
            string defType = source.Name.LocalName;

            if (string.IsNullOrEmpty(defName)) return;

            IEnumerable<XElement> existing = removeTarget.Root
                .Elements(defType)
                .Where(e => e.Element("defName")?.Value == defName);
            
            foreach(XElement existingElement in existing)
            {
                existingElement.Remove();
            }
        }
    }
}