using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace AngusChangyiMods.Core.Test
{
    public static class DefinitionTestCases
    {
        private static string Directory => "Script/AngusChangyiMods/Test/TestCaseSources/";

        private static string Read(string fileName)
        {
            string path = Path.Combine(Application.dataPath, Directory, fileName);
            return File.ReadAllText(path);
        }
        static string GetPath(string fileName)
        {
            return Path.Combine(Application.dataPath, Directory, fileName);
        }

        public static IEnumerable EmptyDefinitionsPath
        {
            get { yield return new TestCaseData(GetPath("EmptyDefinitions.xml")); }
        }
        public static IEnumerable IllegalFormatPath
        {
            get { yield return new TestCaseData(GetPath("IllegalFormat.xml")); }
        }
        public static IEnumerable SimpleCasePath
        {
            get { yield return new TestCaseData(GetPath("SimpleCase.xml")); }
        }
        public static IEnumerable ComplexCasePath
        {
            get { yield return new TestCaseData(GetPath("ComplexCase.xml")); }
        }
        
        public static IEnumerable SimpleCase
        {
            get { yield return new TestCaseData(Read("SimpleCase.xml")); }
        }

       
    }
}
