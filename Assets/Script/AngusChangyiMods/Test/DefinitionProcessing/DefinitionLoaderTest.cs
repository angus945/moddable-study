using System;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using AngusChangyiMods.Core.DefinitionProcessing;
using AngusChangyiMods.Core.Test;

namespace AngusChangyiMods.Core.DefinitionProcessing.Test
{
    [TestFixture]
    [TestOf(typeof(DefinitionLoader))]
    public class DefinitionLoaderTest
    {

        [Test]
        [TestCaseSource(typeof(DefinitionTestCases), nameof(DefinitionTestCases.SimpleCasePath))]
        [TestCaseSource(typeof(DefinitionTestCases), nameof(DefinitionTestCases.ComplexCasePath))]
        public void Test_01_ShouldLoadDefinitions(string path)
        {
            // Arrange
            DefinitionLoader loader = new DefinitionLoader();
            XDocument expected = XDocument.Load(path); 
            
            // Act
            XDocument definitions = loader.LoadDefinition(path);
            
            // Assert
            string content = definitions.ToString(SaveOptions.DisableFormatting);
            string expectedContent = expected.ToString(SaveOptions.DisableFormatting);
            Assert.IsNotNull(definitions, "Definitions should not be null");
            Assert.IsTrue(definitions.Root.Elements().Any(), "Definitions should contain elements");
            Assert.AreEqual(expectedContent, content, "Loaded definitions do not match expected content");
        }
        
        [Test]
        public void Test_02_ShouldThrowExceptionForInvalidPath()
        {
            // Arrange
            DefinitionLoader loader = new DefinitionLoader();
            string invalidPath = "invalid/path/to/definitions.xml";
            
            // Act & Assert
            Assert.Throws<System.IO.FileNotFoundException>(() => loader.LoadDefinition(invalidPath), 
                "Loading from an invalid path should throw FileNotFoundException");
        }

        [Test]
        [TestCaseSource(typeof(DefinitionTestCases), nameof(DefinitionTestCases.EmptyDefinitionsPath))]
        public void Test_03_ShouldHandleEmptyDefinitions(string path)
        {
            // Arrange
            DefinitionLoader loader = new DefinitionLoader();
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => loader.LoadDefinition(path), 
                "Loading an empty definitions file should throw InvalidOperationException");
        }
        
        [Test]
        [TestCaseSource(typeof(DefinitionTestCases), nameof(DefinitionTestCases.IllegalFormatPath))]
        public void Test_04_ShouldThrowExceptionForIllegalFormat(string path)
        {
            // Arrange
            DefinitionLoader loader = new DefinitionLoader();
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => loader.LoadDefinition(path), 
                "Loading an illegally formatted XML should throw XmlException");
        }
        
    }
}