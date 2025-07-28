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

        public static IEnumerable<TestCaseData> PathCase(string fileName)
        {
            yield return new TestCaseData(GetFullPath(fileName));
        }
        public static IEnumerable<TestCaseData> ContentCase(string fileName)
        {
            yield return new TestCaseData(ReadFile(fileName));
        }
    }
    
    public static class DefProcessingCase_Loader
    {
        public static IEnumerable SimpleCase       => CaseReader.PathCase("Common/SimpleCase.xml");
        public static IEnumerable EmptyDefinitions => CaseReader.PathCase("Loader/EmptyDefinitions.xml");
        public static IEnumerable IllegalFormat    => CaseReader.PathCase("Loader/IllegalFormat.xml");
        public static IEnumerable ComplexCase      => CaseReader. PathCase("Loader/ComplexCase.xml");
    }

    public static class DefProcessingCase_Varifier
    {
        public static IEnumerable SimpleCase      => CaseReader.ContentCase("Common/SimpleCase.xml");
        public static IEnumerable LostDefName     => CaseReader.ContentCase("Varifier/LostDefName.xml");
        public static IEnumerable IllegalDefName1 => CaseReader.ContentCase("Varifier/IllegalDefName1.xml");
        public static IEnumerable IllegalDefName2 => CaseReader. ContentCase("Varifier/IllegalDefName2.xml");
        public static IEnumerable IllegalDefName3 => CaseReader. ContentCase("Varifier/IllegalDefName3.xml");
    }

    public static class DefProcessingCase_Merger
    {
        public static IEnumerable MergeCase => ContentCase("Merger/MergeExpected.xml", "Merger/MergeSource1.xml", "Merger/MergeSource2.xml");
        public static IEnumerable OverrideCase => ContentCase("Merger/OverrideExpected.xml", "Merger/OverrideSource1.xml", "Merger/OverrideSource2.xml");
        public static IEnumerable IllegalCase => ContentCase("Merger/IllegalExpected.xml", "Merger/IllegalSource1.xml", "Merger/IllegalSource2.xml");
        
        public static IEnumerable<TestCaseData> ContentCase(string expectedFileName, params string[] sourceFileNames)
        {
            XDocument expected = (CaseReader.ReadXML(expectedFileName));
            XDocument[] sources = new XDocument[sourceFileNames.Length];
            for (int i = 0; i < sourceFileNames.Length; i++)
            {
                sources[i] = (CaseReader.ReadXML(sourceFileNames[i]));
            }
            yield return new TestCaseData(sources, expected);
        }
    }
}
