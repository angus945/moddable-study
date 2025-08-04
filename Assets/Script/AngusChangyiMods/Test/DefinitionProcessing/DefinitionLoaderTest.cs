using System;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using AngusChangyiMods.Core.Test;

namespace AngusChangyiMods.Core.DefinitionProcessing.Test
{
    [TestFixture]
    [TestOf(typeof(DefinitionLoader))]
    public class DefinitionLoaderTest
    {

        [Test]
        public void ShouldLoadValidDefinition_WithSourceInfo()
        {
            // Arrange
            var logger = new MockLogger();
            var loader = new DefinitionLoader(logger);
            string sourceMod = "TestMod";

            XDocument inputDoc = new DefBuilder()
                .WithDef<MockDefinition>("Test.Example")
                .Label("範例")
                .Description("測試用")
                .AddProperty("someValue", "123")
                .Build();

            string inputPath = XmlTestUtil.SaveToTempFile(inputDoc);

            XDocument expectedDoc = new DefBuilder()
                .WithDef<MockDefinition>("Test.Example")
                .Label("範例")
                .Description("測試用")
                .AddProperty("someValue", "123")
                .AddProperty(Def.SourceFile, inputPath)
                .AddProperty(Def.SourceMod, sourceMod)
                .Build();

            // Act
            XDocument result = loader.LoadDefinition(inputPath, sourceMod);

            // Assert
            TestContext.WriteLine(result.ToString());

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedDoc.ToString(), result.ToString());
            Assert.That(logger.Logs[0].Message, Does.Contain(DefinitionLoader.infoSuccessfullyLoaded));
        }

        [Test]
        public void ShouldFailWhenFileNotFound()
        {
            // Arrange
            var logger = new MockLogger();
            var loader = new DefinitionLoader(logger);
            string fakePath = Path.Combine(Path.GetTempPath(), "missing_" + Guid.NewGuid() + ".xml");

            // Act
            var result = loader.LoadDefinition(fakePath, "Test");

            // Assert
            TestContext.WriteLine(result?.ToString() ?? "null");
            Assert.IsNull(result);
            Assert.That(logger.Logs[0].Message, Does.Contain(DefinitionLoader.warningFileNotFound));
        }

        [Test]
        public void ShouldFailWhenFileIsEmpty()
        {
            // Arrange
            MockLogger logger = new MockLogger();
            DefinitionLoader loader = new DefinitionLoader(logger);
            string path = Path.GetTempFileName();
            File.WriteAllText(path, string.Empty);

            // Act
            XDocument result = loader.LoadDefinition(path, "Test");

            // Assert
            TestContext.WriteLine(result?.ToString()?? "null");
            Assert.IsNull(result);
            Assert.That(logger.Logs[0].Message, Does.Contain(DefinitionLoader.errorXmlParse), 
                "Should log XML parse error for empty file, expected:" + DefinitionLoader.errorXmlParse + "but: " + logger.Logs[0].Message);
        }

        [Test]
        public void ShouldFailWhenRootIsMissingOrWrong()
        {
            // Arrange
            MockLogger logger = new MockLogger();
            DefinitionLoader loader = new DefinitionLoader(logger);
            string wrongRootXml = "<banana><item/></banana>";
            string path = Path.GetTempFileName();
            File.WriteAllText(path, wrongRootXml);

            // Act
            var result = loader.LoadDefinition(path, "Test");

            // Assert
            TestContext.WriteLine(result?.ToString() ?? "null");
            Assert.IsNull(result);
            Assert.That(logger.Logs[0].Message, Does.Contain(DefinitionLoader.errorInvalidFormat));
        }

        [Test]
        public void ShouldFailWhenXmlIsMalformed()
        {
            // Arrange
            MockLogger logger = new MockLogger();
            DefinitionLoader loader = new DefinitionLoader(logger);
            string malformedXml = "<Defs><bad></Defs>"; // missing closing tag
            string path = Path.GetTempFileName();
            File.WriteAllText(path, malformedXml);

            // Act
            var result = loader.LoadDefinition(path, "Test");

            // Assert
            TestContext.WriteLine(result?.ToString() ?? "null");
            Assert.IsNull(result);
            Assert.That(logger.Logs[0].Message, Does.Contain(DefinitionLoader.errorXmlParse));
        }
    }
}
