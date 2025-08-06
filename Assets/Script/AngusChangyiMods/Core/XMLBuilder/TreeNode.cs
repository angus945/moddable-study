using System.Collections.Generic;
using System.Xml.Linq;

namespace AngusChangyiMods.Core
{
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
