using System;
using System.Collections.Generic;
using System.Xml;

namespace AngusChangyiMods.Core
{
    public class ModSettingContext : IModSettingContext
    {
        public readonly string modId;

        public XmlDocument doc { get; private set; }
        XmlElement root;

        public ModSettingContext(string modId, XmlDocument doc)
        {
            this.modId = modId;

            this.doc = doc;
            this.root = doc.DocumentElement;
        }

        public void ReadValue<T>(string key, ref T value, T defaultValue)
        {
            XmlNode node = root.SelectSingleNode(key);
            if (node != null)
            {
                value = (T)Convert.ChangeType(node.InnerText, typeof(T));
            }
            else
            {
                value = defaultValue;
            }
        }
        public void ReadCollection<T>(string key, List<T> collection, List<T> defaultValue)
        {
            if (collection == null)
            {
                collection = new List<T>();
            }
            if (defaultValue == null)
            {
                defaultValue = new List<T>();
            }

            collection.Clear();

            XmlNode listNode = root.SelectSingleNode(key);
            if (listNode != null)
            {
                foreach (XmlNode itemNode in listNode.ChildNodes)
                {
                    if (itemNode.Name == "li")
                    {
                        T item = (T)Convert.ChangeType(itemNode.InnerText, typeof(T));
                        collection.Add(item);
                    }
                }
            }
            else
            {
                collection.AddRange(defaultValue);
            }
        }
        public void WriteValue<T>(string key, T value)
        {
            if (value == null) return;

            XmlNode existingNode = root.SelectSingleNode(key);
            if (existingNode != null)
            {
                existingNode.InnerText = value.ToString();
            }
            else
            {
                XmlElement element = doc.CreateElement(key);
                element.InnerText = value.ToString();
                root.AppendChild(element);
            }
        }
        public void WriteCollection<T>(string key, List<T> collection)
        {
            if (collection == null || collection.Count == 0)
            {
                return;
            }

            XmlNode existingListNode = root.SelectSingleNode(key);
            if (existingListNode != null)
            {
                existingListNode.RemoveAll();

                foreach (var item in collection)
                {
                    XmlElement itemNode = doc.CreateElement("li");
                    itemNode.InnerText = item.ToString();
                    existingListNode.AppendChild(itemNode);
                }
            }
            else
            {
                XmlElement listNode = doc.CreateElement(key);
                foreach (var item in collection)
                {
                    XmlElement itemNode = doc.CreateElement("li");
                    itemNode.InnerText = item.ToString();
                    listNode.AppendChild(itemNode);
                }

                root.AppendChild(listNode);
            }
        }
    }
}

