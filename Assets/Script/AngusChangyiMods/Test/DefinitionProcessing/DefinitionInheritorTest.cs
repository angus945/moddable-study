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
        [TestCaseSource(typeof(DefProcessingCase_Inheritance), nameof(DefProcessingCase_Inheritance.PropertyCase))]
        public void Test_01_ShouldInheritanceProperty(XDocument source, XDocument expected)
        {
            string message = InheritorTest(source, expected);
            
            Assert.That(message, Does.Contain("Inherited"), "Process message should indicate inheritance processing, but got: " + message);
        }
        
        [Test]
        [TestCaseSource(typeof(DefProcessingCase_Inheritance), nameof(DefProcessingCase_Inheritance.ListCase))]
        public void Test_01_ShouldInheritanceList(XDocument source, XDocument expected)
        {
            string message = InheritorTest(source, expected);
            
            Assert.That(message, Does.Contain("Inherited"), "Process message should indicate inheritance processing, but got: " + message);
        }
        
        [Test]
        [TestCaseSource(typeof(DefProcessingCase_Inheritance), nameof(DefProcessingCase_Inheritance.MultiLevelCase))]
        public void Test_02_ShouldProcessMultiLevelInheritance(XDocument source, XDocument expected)
        {
            string message = InheritorTest(source, expected);
            
            Assert.That(InheritorTest(source, expected), Does.Contain("Inherited"), "Process message should indicate inheritance processing, but got: " + message);
        }

        
        [Test]
        [TestCaseSource(typeof(DefProcessingCase_Inheritance), nameof(DefProcessingCase_Inheritance.MultiParentCase))]
        public void Test_03_ShouldProcessMultiParentInheritance(XDocument source, XDocument expected)
        {
            string message = InheritorTest(source, expected);

            // Optional: Check for specific inheritance messages in processMessage
            Assert.That(message, Does.Contain("Inherited"), "Process message should indicate inheritance processing, but got: " + message);
        }
        
        [Test]
        [TestCaseSource(typeof(DefProcessingCase_Inheritance), nameof(DefProcessingCase_Inheritance.MissingParentCase))]
        public void Test_04_ShouldBreakInheritanceOnMissingParent(XDocument source, XDocument expected)
        {
            string message = InheritorTest(source, expected);   
            
            // Optional: Check for specific inheritance messages in processMessage
            Assert.That(message, Does.Contain("has no valid parent definition"), "Process message should indicate missing parent definitions");
        }
        
        [Test]
        [TestCaseSource(typeof(DefProcessingCase_Inheritance), nameof(DefProcessingCase_Inheritance.CircularInheritanceCase))]
        public void Test_05_ShouldHandleCircularInheritance(XDocument source, XDocument expected)
        {
            string message = InheritorTest(source, expected);
            
            Assert.That(message, Does.Contain("circular inheritance detected"), "Process message should indicate circular inheritance detection");
        }
        
        
        static string InheritorTest(XDocument source, XDocument expected)
        {
            // Arrange
            DefinitionInheritor inheritor = new DefinitionInheritor();

            // Act
            XDocument result = inheritor.ProcessInheritance(source, out string message);

            // Assert
            string resultString = result.ToString();
            string expectedString = expected.ToString();
            Assert.IsNotNull(result, "Result should not be null");
            Assert.IsNotNull(result.Root, "Result should have a root element");
            Assert.IsTrue(result.Root.Elements().Any(), "Result should contain elements");
            Assert.AreEqual(expectedString, resultString, "Result should match expected XML structure, \nresult: \n" + resultString + "\nexpected: \n" + expectedString);

            // Optional: Check for specific inheritance messages in processMessage
            return message;
        }


    }
}