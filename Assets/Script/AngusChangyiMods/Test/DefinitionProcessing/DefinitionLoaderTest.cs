using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using AngusChangyiMods.Core.DefinitionProcessing;

namespace AngusChangyiMods.Core.DefinitionProcessing.Test
{
[TestFixture]
[TestOf(typeof(DefinitionLoader))]
public class DefinitionLoaderTest
{
    [Test]
    [TestCaseSource(typeof(DefProcessingCase_Loader), nameof(DefProcessingCase_Loader.SimpleCase))]
    [TestCaseSource(typeof(DefProcessingCase_Loader), nameof(DefProcessingCase_Loader.ComplexCase))]
    public void Test_01_ShouldLoadDefinitions(string path)
    {
        // Arrange
        DefinitionLoader loader = new DefinitionLoader();
        bool result = loader.LoadDefinition(path, out XDocument definitions, out string error);

        // Assert
        Assert.IsTrue(result, "Loader should succeed on valid path");
        Assert.IsNull(error, "Error message should be null for valid load");
        Assert.IsNotNull(definitions, "Definitions should not be null");
        Assert.IsTrue(definitions.Root.Elements().Any(), "Definitions should contain elements");

        // Optional: Compare with expected content
        string expected = XDocument.Load(path).ToString(SaveOptions.DisableFormatting);
        string actual = definitions.ToString(SaveOptions.DisableFormatting);
        Assert.AreEqual(expected, actual, "Loaded content mismatch");
    }

    [Test]
    public void Test_02_ShouldReturnFalseForInvalidPath()
    {
        // Arrange
        DefinitionLoader loader = new DefinitionLoader();
        string invalidPath = "invalid/path/to/definitions.xml";

        // Act
        bool result = loader.LoadDefinition(invalidPath, out XDocument doc, out string error);

        // Assert
        Assert.IsFalse(result, "Loader should fail on invalid path");
        Assert.IsNull(doc, "Document should be null on failure");
        Assert.That(error, Does.Contain("not found"), "Error message should indicate file not found");
    }

    [Test]
    [TestCaseSource(typeof(DefProcessingCase_Loader), nameof(DefProcessingCase_Loader.EmptyDefinitions))]
    public void Test_03_ShouldReturnFalseForEmptyDefinitions(string path)
    {
        // Arrange
        DefinitionLoader loader = new DefinitionLoader();

        // Act
        bool result = loader.LoadDefinition(path, out XDocument doc, out string error);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(doc);
        Assert.That(error, Does.Contain("XML parse error").Or.Contain("Root element is missing"));
    }


    [Test]
    [TestCaseSource(typeof(DefProcessingCase_Loader), nameof(DefProcessingCase_Loader.IllegalFormat))]
    public void Test_04_ShouldReturnFalseForIllegalFormat(string path)
    {
        // Arrange
        DefinitionLoader loader = new DefinitionLoader();

        // Act
        bool result = loader.LoadDefinition(path, out XDocument doc, out string error);

        // Assert
        Assert.IsFalse(result, "Loader should fail on illegal XML format");
        Assert.IsNull(doc);
    }
}
}