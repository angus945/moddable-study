using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using AngusChangyiMods.Core.DefinitionProcessing;
using AngusChangyiMods.Core.Test;
using AngusChangyiMods.Logger;

namespace AngusChangyiMods.Core.DefinitionProcessing.Test
{
    [TestFixture]
    [TestOf(typeof(DefinitionLoader))]
    public class DefinitionLoaderTest
    {
        // [Test]
        // [TestCase("Common/SimpleCase.xml", "Loader/SimpleCase_Expected.xml")]
        // [TestCase("Loader/ComplexCase.xml", "Loader/ComplexCase_Expected.xml")]
        // public void Test_01_ShouldLoadDefinitions(string loadFilePath, string expectedFilePath)
        // {
        //     // Arrange
        //     MockLogger logger = new MockLogger();
        //     DefinitionLoader loader = new DefinitionLoader(logger);
        //     string loadPath = CaseReader.GetFullPath(loadFilePath);
        //     
        //     
        //     // Act
        //     XDocument def = loader.LoadDefinition(loadPath, "TestMod");
        //
        //     // Assert
        //     Assert.IsNotNull(def);
        //     string expected = CaseReader.ReadXML(expectedFilePath).ToString();    
        //     string actual = def.ToString();
        //     Assert.AreEqual(expected, actual, $"Loaded content mismatch, expected: \n {expectedFilePath}, actual: \n {loadFilePath}");
        //
        //     Assert.That(logger.Logs[0].Message, Does.StartWith(DefinitionLoader.infoSuccessfullyLoaded));
        // }

        // [Test]
        // public void Test_02_ShouldReturnFalseForInvalidPath()
        // {
        //     // Arrange
        //     DefinitionLoader loader = new DefinitionLoader();
        //     string invalidPath = "invalid/path/to/definitions.xml";
        //
        //     // Act
        //     bool result = loader.LoadDefinition(invalidPath, out XDocument doc, out string error);
        //
        //     // Assert
        //     Assert.IsFalse(result, "Loader should fail on invalid path");
        //     Assert.IsNull(doc, "Document should be null on failure");
        //     Assert.That(error, Does.Contain("not found"), "Error message should indicate file not found");
        // }
        //
        // [Test]
        // [TestCase("Loader/EmptyDefinitions.xml")]
        // public void Test_03_ShouldReturnFalseForEmptyDefinitions(string fileName)
        // {
        //     // Arrange
        //     DefinitionLoader loader = new DefinitionLoader();
        //     string path = CaseReader.GetFullPath(fileName);
        //
        //     // Act
        //     bool result = loader.LoadDefinition(path, out XDocument doc, out string error);
        //
        //     // Assert
        //     Assert.IsFalse(result);
        //     Assert.IsNull(doc);
        //     Assert.That(error, Does.Contain("XML parse error").Or.Contain("Root element is missing"));
        // }
        //
        //
        // [Test]
        // [TestCase("Loader/IllegalFormat.xml")]
        // public void Test_04_ShouldReturnFalseForIllegalFormat(string fileName)
        // {
        //     // Arrange
        //     DefinitionLoader loader = new DefinitionLoader();
        //     string path = CaseReader.GetFullPath(fileName);
        //
        //     // Act
        //     bool result = loader.LoadDefinition(path, out XDocument doc, out string error);
        //
        //     // Assert
        //     Assert.IsFalse(result, "Loader should fail on illegal XML format");
        //     Assert.IsNull(doc);
        // }
    }
}