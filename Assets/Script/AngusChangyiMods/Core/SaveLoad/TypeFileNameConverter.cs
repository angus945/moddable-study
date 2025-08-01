using System;
using System.IO;
using System.Linq;

namespace Script.AngusChangyiMods.Core.Util
{
    public static class TypeFileNameConverter
    {
        public static string GetFileNameForType(Type type, string fileExtension)
        {
            string baseName;

            if (type.IsGenericType)
            {
                string typeName = type.GetGenericTypeDefinition().FullName;
                string argNames = string.Join(",", type.GetGenericArguments().Select(t => t.FullName));
                baseName = $"{typeName}[{argNames}]";
            }
            else
            {
                baseName = type.FullName;
            }

            // 修正 nested class 表示法與非法字元
            baseName = baseName.Replace('+', '.');

            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                baseName = baseName.Replace(c, '_');
            }

            return baseName + fileExtension;
        }
    }


}