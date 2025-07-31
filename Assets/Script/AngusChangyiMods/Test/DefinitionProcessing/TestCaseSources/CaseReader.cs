using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using UnityEngine;

namespace AngusChangyiMods.Core.DefinitionProcessing.Test
{
    public static class CaseReader
    {
        private static readonly string Directory = "Script/AngusChangyiMods/Test/TestCaseSources/";

        public static string GetFullPath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("File name cannot be null or empty", nameof(fileName));
            
            return Path.Combine(Application.dataPath, Directory, fileName);
        }
        public static string ReadFile(string fileName)
        {
            if (!File.Exists(GetFullPath(fileName)))
                throw new FileNotFoundException($"Test case file not found: {fileName}");
            
            return File.ReadAllText(GetFullPath(fileName));
        }
        public static XDocument ReadXML(string fileName)
        {
            if (!File.Exists(GetFullPath(fileName)))
                throw new FileNotFoundException($"Test case file not found: {fileName}");
            
            return XDocument.Load(GetFullPath(fileName));
        }
    }
}
