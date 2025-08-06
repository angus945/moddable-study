using System;
using System.Collections.Generic;
using System.Xml.Linq;
using AngusChangyiMods.Core;
using AngusChangyiMods.Core.DefinitionProcessing;
using AngusChangyiMods.Core.DefinitionProcessing.PatchOperation;

namespace AngusChangyiMods.Core
{
    public class PatchBuilder
    {
        private readonly List<XElement> operations = new List<XElement>();
        private XElement current;

        public PatchBuilder WithPatch<T>() where T : IPatchOperation
        {
            current = new XElement(XPatch.Operation);
            current.SetAttributeValue(XPatch.Class, typeof(T).Name);
            operations.Add(current);
            return this; // 關鍵：保持回傳自己，不要建立新的 PatchBuilder
        }

        public PatchBuilder XPath(string xpath)
        {
            if (current == null) throw new InvalidOperationException("Call WithPatch<T>() first.");
            current.Add(new XElement(XPatch.XPath, xpath));
            return this;
        }

        public PatchBuilder Value(XElement element)
        {
            if (current == null) throw new InvalidOperationException("Call WithPatch<T>() first.");
            var wrapper = new XElement(XPatch.Value);
            wrapper.Add(new XElement(element));
            current.Add(wrapper);
            return this;
        }

        public PatchBuilder Value(params TreeNode[] children)
        {
            if (current == null) throw new InvalidOperationException("Call WithPatch<T>() first.");
            var wrapper = new XElement(XPatch.Value);
            foreach (var child in children)
            {
                wrapper.Add(TreeNode.ToXElement(child));
            }
            current.Add(wrapper);
            return this;
        }

        public XDocument Build()
        {
            return new XDocument(new XElement(XPatch.Root, operations));
        }

        // ... 省略 TreeNode 同上
    }


}
