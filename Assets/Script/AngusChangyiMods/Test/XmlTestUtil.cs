using System;
using System.IO;
using System.Xml.Linq;

namespace AngusChangyiMods.Core.Test
{
    public static class XmlTestUtil
    {
        public static string SaveToTempFile(XDocument doc)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), $"def_{Guid.NewGuid()}.xml");
            doc.Save(tempPath);
            return tempPath;
        }
    }
}