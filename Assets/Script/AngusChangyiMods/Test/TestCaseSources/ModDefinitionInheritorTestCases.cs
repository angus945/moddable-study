using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace ModInfrastructure.Test
{
    public static class ModDefinitionInheritorTestCases
    {
        private static string DataPath => Path.Combine(Application.dataPath, "Script/AngusChangyiMods/Test/TestData/ModDefinitionInheritor");

        private static string Read(string fileName) => File.ReadAllText(Path.Combine(DataPath, fileName));

        public static IEnumerable SingleInheritance
        {
            get { yield return new TestCaseData(Read("SingleInheritance.xml")); }
        }

        public static IEnumerable DeepInheritance
        {
            get { yield return new TestCaseData(Read("DeepInheritance.xml")); }
        }

        public static IEnumerable AbstractDefs
        {
            get { yield return new TestCaseData(Read("AbstractDefs.xml")); }
        }

        public static IEnumerable TagLists
        {
            get { yield return new TestCaseData(Read("TagLists.xml")); }
        }

        public static IEnumerable MissingParent
        {
            get { yield return new TestCaseData(Read("MissingParent.xml")); }
        }
    }
}
