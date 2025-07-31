using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AngusChangyiMods.Core
{
    public interface IDefinitionInheritor
    {
        XDocument ProcessInheritance(XDocument source);
    }
    
    public class DefinitionInheritor : IDefinitionInheritor
    {
        private readonly ILogger logger;
        
        private Dictionary<string, XElement> defLookup;
        private List<XElement> inheritedElements;

        public DefinitionInheritor(ILogger logger)
        {
            this.logger = logger;
        }
        
        public XDocument ProcessInheritance(XDocument source)
        {
            var result = new XDocument(source);
            var root = result.Root;
            
            if (root == null)
            {
                logger.LogError("Source document has no root element.", "DefinitionInheritor");
                return result;
            }

            defLookup = CreateDefLookUp(root);
            inheritedElements = ListInheritedElements(root);

            logger.Log($"Processing {inheritedElements.Count} inherited elements.", "DefinitionInheritor");

            foreach (var element in inheritedElements)
            {
                string parentId = element.Attribute(Def.Parent)?.Value;
                string elementName = element.Element(Def.DefName)?.Value ?? "Unknown";
                
                if (parentId == null || !defLookup.ContainsKey(parentId))
                {
                    logger.LogWarning($"Element '{elementName}' has no valid parent definition '{parentId}'.", "DefinitionInheritor");
                    continue;
                }

                var merged = DeepCloneAndMerge(parentId, new HashSet<string>());
                if (merged == null)
                {
                    logger.LogError($"Failed to merge definition for '{elementName}' with parent '{parentId}'. Possible circular reference.", "DefinitionInheritor");
                    continue;
                }

                MergeElementData(merged, element);
                element.ReplaceWith(merged);
                logger.Log($"Successfully inherited '{elementName}' from '{parentId}'.", "DefinitionInheritor");
            }

            RemoveAbstractDefinition(root);
            logger.Log("Definition inheritance processing completed.", "DefinitionInheritor");

            return result;
        }

        private static Dictionary<string, XElement> CreateDefLookUp(XElement root)
        {
            return root.Elements()
                .Where(e => e.Element(Def.DefName) != null)
                .ToDictionary(e => e.Element(Def.DefName)!.Value, e => e);
        }
        
        private static List<XElement> ListInheritedElements(XElement root)
        {
            return root.Elements()
                .Where(e => e.Attribute(Def.Parent) != null && e.Attribute(Def.IsAbstract)?.Value != "true")
                .ToList();
        }

        private XElement DeepCloneAndMerge(string defId, HashSet<string> visited)
        {
            if (visited.Contains(defId)) 
            {
                logger.LogError($"Circular reference detected for definition '{defId}'.", "DefinitionInheritor");
                return null;
            }
            
            visited.Add(defId);

            if (!defLookup.TryGetValue(defId, out var baseElement)) 
            {
                logger.LogError($"Definition '{defId}' not found in lookup.", "DefinitionInheritor");
                return null;
            }

            var clone = new XElement(baseElement);
            string parentId = baseElement.Attribute(Def.Parent)?.Value;
            if (parentId != null && defLookup.ContainsKey(parentId))
            {
                var parentClone = DeepCloneAndMerge(parentId, visited);
                if (parentClone != null)
                {
                    MergeElementData(parentClone, clone);
                    return parentClone;
                }
            }

            return clone;
        }

        private void MergeElementData(XElement baseElement, XElement childElement)
        {
            foreach (var childNode in childElement.Elements())
            {
                var baseNode = baseElement.Element(childNode.Name);

                // 如果是列表（以 <tag> 為例�� → 合併
                if (IsListNode(childNode))
                {
                    var mergedList = new XElement(childNode.Name);
                    var baseTags = baseNode?.Elements().Select(e => e.Value).ToHashSet() ?? new HashSet<string>();
                    var childTags = childNode.Elements().Select(e => e.Value);

                    foreach (var tag in baseTags.Union(childTags))
                        mergedList.Add(new XElement(childNode.Elements().First().Name, tag));

                    baseNode?.Remove();
                    baseElement.Add(mergedList);
                }
                else
                {
                    // 一般欄位直接覆蓋
                    baseNode?.Remove();
                    baseElement.Add(new XElement(childNode));
                }
            }

            // 移除 baseElement 上的繼承與抽象屬性
            baseElement.SetAttributeValue(Def.Parent, null);
            baseElement.SetAttributeValue(Def.IsAbstract, null);
        }
        
        private static void RemoveAbstractDefinition(XElement root)
        {
            root.Elements()
                .Where(e => e.Attribute(Def.IsAbstract)?.Value == "true")
                .ToList()
                .ForEach(e => e.Remove());
        }

        private bool IsListNode(XElement node)
        {
            return node.Elements().Count() > 1 && node.Elements().All(e => e.Name == node.Elements().First().Name);
        }
    }
}
