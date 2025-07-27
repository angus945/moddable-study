using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AngusChangyiMods.Core
{
    public class ModDefinitionInheritor
    {
        ILogger logger;
        public ModDefinitionInheritor(ILogger logger)
        {
            this.logger = logger;
        }

        public XDocument ProcessInheritance(XDocument source)
        {
            var result = new XDocument(source);
            var root = result.Root;
            if (root == null) return result;

            // 建立 defID → 元素的查詢表（支援多型）
            Dictionary<string, XElement> defLookup = root.Elements()
                .Where(e => e.Element("defID") != null)
                .ToDictionary(e => e.Element("defID")!.Value, e => e);

            // 尋找有繼承關係的非抽象定義
            List<XElement> inheritedElements = root.Elements()
                .Where(e => e.Attribute("parent") != null && e.Attribute("isAbstract")?.Value != "true")
                .ToList();

            foreach (var element in inheritedElements)
            {
                string? parentId = element.Attribute("parent")?.Value;
                if (parentId == null || !defLookup.ContainsKey(parentId))
                    continue;

                // 深度複製繼承鏈
                var merged = DeepCloneAndMerge(parentId, defLookup, new HashSet<string>());
                if (merged == null) continue;

                // 將子元素的欄位覆蓋進 merged
                MergeElementData(merged, element);

                // 將合併結果替換原本的 element 內容
                element.ReplaceWith(merged);
            }

            // 移除 isAbstract=true 的定義
            root.Elements()
                .Where(e => e.Attribute("isAbstract")?.Value == "true")
                .ToList()
                .ForEach(e => e.Remove());

            return result;
        }

        private XElement DeepCloneAndMerge(string defId, Dictionary<string, XElement> defLookup, HashSet<string> visited)
        {
            if (visited.Contains(defId)) return null;
            visited.Add(defId);

            if (!defLookup.TryGetValue(defId, out var baseElement)) return null;

            var clone = new XElement(baseElement);
            string? parentId = baseElement.Attribute("parent")?.Value;
            if (parentId != null && defLookup.ContainsKey(parentId))
            {
                var parentClone = DeepCloneAndMerge(parentId, defLookup, visited);
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

                // 如果是列表（以 <tag> 為例） → 合併
                if (childNode.Name.LocalName == "tags" || IsListNode(childNode))
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
            baseElement.SetAttributeValue("parent", null);
            baseElement.SetAttributeValue("isAbstract", null);
        }

        private bool IsListNode(XElement node)
        {
            return node.Elements().Count() > 1 && node.Elements().All(e => e.Name == node.Elements().First().Name);
        }
    }
}
