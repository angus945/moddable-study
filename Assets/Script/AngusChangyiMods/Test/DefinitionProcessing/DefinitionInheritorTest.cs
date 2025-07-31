using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace AngusChangyiMods.Core.DefinitionProcessing.Test
{
    [TestFixture]
    [TestOf(typeof(DefinitionInheritor))]
    public class DefinitionInheritorTest
    {
        [Test]
        [TestCase("Inheritance/PropertyCaseSource.xml", "Inheritance/PropertyCaseExpected.xml")]
        public void Test_01_ShouldInheritanceProperty(string source, string expected)
        {
            string message = InheritorTest(source, expected);
            
            Assert.That(message, Does.Contain("Inherited"), "Process message should indicate inheritance processing, but got: " + message);
        }
        
        [Test]
        [TestCase("Inheritance/ListSource.xml", "Inheritance/ListExpected.xml")]
        public void Test_02_ShouldInheritanceList(string source, string expected)
        {
            string message = InheritorTest(source, expected);
            
            Assert.That(message, Does.Contain("Inherited"), "Process message should indicate inheritance processing, but got: " + message);
        }
        
        [Test]
        [TestCase("Inheritance/MultiLevelSource.xml", "Inheritance/MultiLevelExpected.xml")]
        public void Test_03_ShouldProcessMultiLevelInheritance(string source, string expected)
        {
            string message = InheritorTest(source, expected);
            
            Assert.That(message, Does.Contain("Inherited"), "Process message should indicate inheritance processing, but got: " + message);
        }

        
        [Test]
        [TestCase("Inheritance/MultiParentSource.xml", "Inheritance/MultiParentExpected.xml")]
        public void Test_04_ShouldProcessMultiParentInheritance(string source, string expected)
        {
            string message = InheritorTest(source, expected);

            // Optional: Check for specific inheritance messages in processMessage
            Assert.That(message, Does.Contain("Inherited"), "Process message should indicate inheritance processing, but got: " + message);
        }
        
        [Test]
        [TestCase("Inheritance/ParentMissingSource.xml", "Inheritance/ParentMissingExpected.xml")]
        public void Test_05_ShouldBreakInheritanceOnMissingParent(string source, string expected)
        {
            string message = InheritorTest(source, expected);   
            
            // Optional: Check for specific inheritance messages in processMessage
            Assert.That(message, Does.Contain("has no valid parent definition"), "Process message should indicate missing parent definitions");
        }
        
        [Test]
        [TestCase("Inheritance/CircularInheritanceSource.xml", "Inheritance/CircularInheritanceExpected.xml")]
        public void Test_06_ShouldHandleCircularInheritance(string source, string expected)
        {
            string message = InheritorTest(source, expected);
            
            Assert.That(message, Does.Contain("circular inheritance detected"), "Process message should indicate circular inheritance detection");
        }
        
        
        static string InheritorTest(string source, string expected)
        {
            // Arrange
            XDocument sourceDoc = CaseReader.ReadXML(source);
            XDocument expectedDoc = CaseReader.ReadXML(expected);
            DefinitionInheritor inheritor = new DefinitionInheritor();

            // Act
            XDocument result = inheritor.ProcessInheritance(sourceDoc, out string message);

            // Assert
            string resultString = result.ToString();
            string expectedString = expectedDoc.ToString();
            Assert.IsNotNull(result, "Result should not be null");
            Assert.IsNotNull(result.Root, "Result should have a root element");
            Assert.IsTrue(result.Root.Elements().Any(), "Result should contain elements");
            Assert.AreEqual(expectedString, resultString, "Result should match expected XML structure, \nresult: \n" + resultString + "\nexpected: \n" + expectedString);

            // Optional: Check for specific inheritance messages in processMessage
            return message;
        }


    }
}