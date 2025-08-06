using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace AngusChangyiMods.Core
{

    public class DefBuilder
    {
        private readonly List<XElement> defs = new List<XElement>();
        private XElement current;

        public DefBuilder WithDef<T>(string defName, bool isAbstract = false) where T : DefBase
        {
            current = new XElement(typeof(T).Name,
                new XElement(XDef.DefName, defName)
            );

            if (isAbstract)
            {
                current.SetAttributeValue(XDef.IsAbstract, "true");
            }

            defs.Add(current);
            return this;
        }

        public DefBuilder InheritFrom(string parent)
        {
            current?.SetAttributeValue(XDef.Parent, parent);
            return this;
        }

        public DefBuilder Label(string label)
        {
            current?.Add(new XElement(XDef.Label, label));
            return this;
        }

        public DefBuilder Description(string description)
        {
            current?.Add(new XElement(XDef.Description, description));
            return this;
        }

        public DefBuilder AddProperty(string name, string value)
        {
            current?.Add(new XElement(name, value));
            return this;
        }

        public DefBuilder AddList(string name, params string[] items)
        {
            var list = new XElement(name, items.Select(i => new XElement("li", i)));
            current?.Add(list);
            return this;
        }

        public DefBuilder AddNested(TreeNode node)
        {
            current?.Add(TreeNode.ToXElement(node));
            return this;
        }

        public DefBuilder AddComponent<T>() where T : ComponentProperty
        {
            if (current == null)
                throw new InvalidOperationException("Must call WithDef<T>() before adding components.");

            XElement comps = current.Element(XDef.Components);
            if (comps == null)
            {
                comps = new XElement(XDef.Components);
                current.Add(comps);
            }

            EnsureNoDuplicateClass(comps, typeof(T).FullName, "Component");

            XElement li = new XElement(XDef.Li);
            li.Add(new XElement(XDef.Class, typeof(T).FullName));
            comps.Add(li);

            return this;
        }
        public DefBuilder AddComponent<T>(params TreeNode[] content) where T : ComponentProperty
        {
            if (current == null)
                throw new InvalidOperationException("Must call WithDef<T>() before adding components.");

            if (content == null || content.Length == 0)
                throw new ArgumentException("Component content cannot be null or empty.");

            var comps = current.Element(XDef.Components);
            if (comps == null)
            {
                comps = new XElement(XDef.Components);
                current.Add(comps);
            }

            EnsureNoDuplicateClass(comps, typeof(T).FullName, "Component");

            var li = new XElement(XDef.Li);
            li.SetAttributeValue(XDef.Class, typeof(T).FullName);
            foreach (var node in content)
            {
                li.Add(TreeNode.ToXElement(node));
            }

            comps.Add(li);
            return this;
        }
        public DefBuilder AddExtension<T>(params TreeNode[] content) where T : DefExtension
        {
            if (current == null)
                throw new InvalidOperationException("Must call WithDef<T>() before adding extensions.");

            if (content == null || content.Length == 0)
                throw new ArgumentException("Extension content cannot be null or empty.");

            var modExts = current.Element(XDef.Extensions);
            if (modExts == null)
            {
                modExts = new XElement(XDef.Extensions);
                current.Add(modExts);
            }

            EnsureNoDuplicateClass(modExts, typeof(T).FullName, "Extension");

            var li = new XElement(XDef.Li);
            li.SetAttributeValue(XDef.Class, typeof(T).FullName);
            foreach (var node in content)
            {
                li.Add(TreeNode.ToXElement(node));
            }

            modExts.Add(li);
            return this;
        }
        private void EnsureNoDuplicateClass(XElement parent, string className, string context)
        {
            bool exists = parent.Elements(XDef.Li)
                .Any(e => e.Attribute(XDef.Class)?.Value == className);

            if (exists)
            {
                throw new InvalidOperationException($"{context} of type '{className}' already added.");
            }
        }

        public XDocument Build()
        {
            return new XDocument(new XElement(XDef.Root, defs));
        }

        // ==========================
        // 巢狀結構支援
        // ==========================




    }
}
