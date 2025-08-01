﻿using System;
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
                new XElement(Def.DefName, defName)
            );

            if (isAbstract)
            {
                current.SetAttributeValue(Def.IsAbstract, "true");
            }

            defs.Add(current);
            return this;
        }

        public DefBuilder InheritFrom(string parent)
        {
            current?.SetAttributeValue(Def.Parent, parent);
            return this;
        }

        public DefBuilder Label(string label)
        {
            current?.Add(new XElement(Def.Label, label));
            return this;
        }

        public DefBuilder Description(string description)
        {
            current?.Add(new XElement(Def.Description, description));
            return this;
        }

        public DefBuilder AddProperty(string name, string value)
        {
            current?.Add(new XElement(name, value));
            return this;
        }

        public DefBuilder ListProp(string name, params string[] items)
        {
            var list = new XElement(name, items.Select(i => new XElement("li", i)));
            current?.Add(list);
            return this;
        }

        public DefBuilder Add(XElement element)
        {
            current?.Add(element);
            return this;
        }

        public DefBuilder Add(TreeNode node)
        {
            current?.Add(ToXElement(node));
            return this;
        }

        public string Build()
        {
            var doc = new XDocument(new XElement(Def.Root, defs));
            string tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xml");
            doc.Save(tempPath);
            return tempPath;
        }

        // ==========================
        // 巢狀結構支援
        // ==========================

        public class TreeNode
        {
            public string Name { get; private set; }
            public object Content { get; private set; }
            public List<TreeNode> Children { get; private set; }

            public TreeNode(string name, object content = null)
            {
                Name = name;
                Content = content;
                Children = new List<TreeNode>();
            }

            public TreeNode WithChildren(params TreeNode[] children)
            {
                Children.AddRange(children);
                return this;
            }
        }


        public static TreeNode Tree(string name, object content = null)
        {
            return new TreeNode(name, content);
        }


        public static XElement ToXElement(TreeNode node)
        {
            var element = new XElement(node.Name);

            foreach (var child in node.Children)
            {
                element.Add(ToXElement(child));
            }

            if (node.Content != null && node.Children.Count == 0)
            {
                element.Value = node.Content.ToString();
            }

            return element;
        }


    }
}
