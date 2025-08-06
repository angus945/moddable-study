using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AngusChangyiMods.Logger;

namespace AngusChangyiMods.Core
{
    public interface IDefinitionInheritor
    {
        List<XElement> ProcessInheritance(XDocument source);
    }

    public class DefinitionInheritor : IDefinitionInheritor
    {
        public const string errorCircularReference = "Circular reference detected in definition inheritance.";
        public const string errorParentNotFound = "Parent definition not found for inheritance.";
        public const string errorMergeFailed = "Failed to merge parent definition during inheritance.";

        struct DefLookUpKey
        {
            public string defname;
            public string type;

            public DefLookUpKey(string defname, string type)
            {
                this.defname = defname;
                this.type = type;
            }

            public override string ToString()
            {
                return $"({type}){defname}";
            }
        }

        private readonly ILogger logger;

        private Dictionary<DefLookUpKey, XElement> defLookup;
        // private List<XElement> inheritedElements;

        public DefinitionInheritor(ILogger logger)
        {
            this.logger = logger;
        }

        public List<XElement> ProcessInheritance(XDocument source)
        {
            XDocument result = new XDocument(source);
            XElement root = result.Root;
            List<XElement> inheritedElements = new List<XElement>();

            defLookup = CreateDefLookUp(root);

            foreach (XElement element in root.Elements())
            {
                if (element.Attribute(XDef.IsAbstract)?.Value == "true") continue;

                if (DeepCloneAndMerge(element, new HashSet<DefLookUpKey>(), out XElement inheritedElement))
                {
                    inheritedElements.Add(inheritedElement);
                }
            }

            return inheritedElements;
        }

        private static Dictionary<DefLookUpKey, XElement> CreateDefLookUp(XElement root)
        {
            return root.Elements().ToDictionary((e) => ConvertToKey(e), e => e);
        }
        private static DefLookUpKey ConvertToKey(XElement e)
        {
            string defName = e.Element(XDef.DefName)?.Value;
            string type = e.Name.LocalName;
            return new DefLookUpKey(defName, type);
        }
        private static bool GetParentLookUpKey(XElement e, out DefLookUpKey key)
        {
            string parentName = e.Attribute(XDef.Parent)?.Value;
            string type = e.Name.LocalName;
            if (string.IsNullOrEmpty(parentName))
            {
                key = default;
                return false;
            }
            else
            {
                key = new DefLookUpKey(parentName, type);
                return true;
            }
        }

        private bool DeepCloneAndMerge(XElement child, HashSet<DefLookUpKey> visited, out XElement inherited)
        {
            DefLookUpKey defKey = ConvertToKey(child);

            if (visited.Contains(defKey))
            {
                logger.LogError($"{errorCircularReference} '{defKey}'.");
                inherited = null;
                return false;
            }

            visited.Add(defKey);

            if (GetParentLookUpKey(child, out var parentKey))
            {
                if (defLookup.TryGetValue(parentKey, out XElement parent))
                {
                    if (DeepCloneAndMerge(parent, visited, out XElement inheritedParent))
                    {
                        MergeElementData(inheritedParent, child);
                        inherited = inheritedParent;
                        return true;
                    }
                    else
                    {
                        logger.LogError($"{errorMergeFailed} '{parentKey}' for '{defKey}'.");
                        inherited = null;
                        return false;
                    }
                }
                else
                {
                    logger.LogWarning($"{errorParentNotFound} '{parentKey}' for '{defKey}'.");
                    inherited = null;
                    return false;
                }
            }
            else
            {
                inherited = new XElement(child);
                return true;
            }
        }

        private void MergeElementData(XElement baseElement, XElement childElement)
        {
            foreach (var childNode in childElement.Elements())
            {
                var baseNode = baseElement.Element(childNode.Name);

                // 特殊處理 components 和 extensions 的合併
                if (childNode.Name == XDef.Components || childNode.Name == XDef.Extensions)
                {
                    MergeComponentsOrExtensions(baseElement, baseNode, childNode);
                }
                // 如果是列表（以 <tag> 為例） → 合併
                else if (IsListNode(childNode))
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
            baseElement.SetAttributeValue(XDef.Parent, null);
            baseElement.SetAttributeValue(XDef.IsAbstract, null);
        }

        private void MergeComponentsOrExtensions(XElement baseElement, XElement baseNode, XElement childNode)
        {
            // 如果父級沒有這個節點，直接添加子級的
            if (baseNode == null)
            {
                baseElement.Add(new XElement(childNode));
                return;
            }

            // 創建合併後的節點
            var mergedNode = new XElement(childNode.Name);

            // 收集所有 li 元素，按照 class 屬性分組
            var baseItems = baseNode.Elements(XDef.Li).ToList();
            var childItems = childNode.Elements(XDef.Li).ToList();

            // 建立一個以 class 為鍵的字典來避免重複
            var mergedItems = new Dictionary<string, XElement>();

            // 先添加父級的項目
            foreach (var item in baseItems)
            {
                var classValue = GetClassValue(item);
                if (!string.IsNullOrEmpty(classValue))
                {
                    mergedItems[classValue] = new XElement(item);
                }
            }

            // 再添加子級的項目（會覆蓋同 class 的父級項目）
            foreach (var item in childItems)
            {
                var classValue = GetClassValue(item);
                if (!string.IsNullOrEmpty(classValue))
                {
                    mergedItems[classValue] = new XElement(item);
                }
            }

            // 將合併後的項目添加到節點中
            foreach (var item in mergedItems.Values)
            {
                mergedNode.Add(item);
            }

            // 替換原有節點
            baseNode.Remove();
            baseElement.Add(mergedNode);
        }

        private static string GetClassValue(XElement item)
        {
            // 嘗試從 class 屬性獲取
            var classAttr = item.Attribute(XDef.Class);
            if (classAttr != null)
                return classAttr.Value;

            // 嘗試從 <compClass> 子元素獲取（component 可能的格式）
            var classElement = item.Element("compClass");
            if (classElement != null)
                return classElement.Value;

            // 對於 components，還可能在 <class> 子元素中
            var classChildElement = item.Element(XDef.Class);
            if (classChildElement != null)
                return classChildElement.Value;

            return null;
        }

        private bool IsListNode(XElement node)
        {
            return node.Elements().Count() > 1 && node.Elements().All(e => e.Name == node.Elements().First().Name);
        }
    }
}
