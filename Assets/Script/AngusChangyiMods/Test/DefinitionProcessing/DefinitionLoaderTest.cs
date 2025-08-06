using System;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using AngusChangyiMods.Core.Test;

namespace AngusChangyiMods.Core.DefinitionProcessing.Test
{
    [TestFixture]
    [TestOf(typeof(XMLLoader))]
    public class XMLLoaderTest
    {

        [Test]
        public void ShouldLoadValidDefinition_WithSourceInfo()
        {
            // Arrange
            var logger = new MockLogger();
            var loader = new XMLLoader(logger, XDef.Root);
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
                .AddProperty(XDef.SourceFile, inputPath)
                .AddProperty(XDef.SourceMod, sourceMod)
                .Build();

            // Act
            XDocument result = loader.LoadXML(inputPath, sourceMod);

            // Assert
            TestContext.WriteLine(result.ToString());

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedDoc.ToString(), result.ToString());
            Assert.That(logger.Logs[0].Message, Does.Contain(XMLLoader.infoSuccessfullyLoaded));
        }

        [Test]
        public void ShouldFailWhenFileNotFound()
        {
            // Arrange
            var logger = new MockLogger();
            var loader = new XMLLoader(logger, XDef.Root);
            string fakePath = Path.Combine(Path.GetTempPath(), "missing_" + Guid.NewGuid() + ".xml");

            // Act
            var result = loader.LoadXML(fakePath, "Test");

            // Assert
            TestContext.WriteLine(result?.ToString() ?? "null");
            Assert.IsNull(result);
            Assert.That(logger.Logs[0].Message, Does.Contain(XMLLoader.warningFileNotFound));
        }

        [Test]
        public void ShouldFailWhenFileIsEmpty()
        {
            // Arrange
            MockLogger logger = new MockLogger();
            XMLLoader loader = new XMLLoader(logger, XDef.Root);
            string path = Path.GetTempFileName();
            File.WriteAllText(path, string.Empty);

            // Act
            XDocument result = loader.LoadXML(path, "Test");

            // Assert
            TestContext.WriteLine(result?.ToString() ?? "null");
            Assert.IsNull(result);
            Assert.That(logger.Logs[0].Message, Does.Contain(XMLLoader.errorXmlParse),
                "Should log XML parse error for empty file, expected:" + XMLLoader.errorXmlParse + "but: " + logger.Logs[0].Message);
        }

        [Test]
        public void ShouldFailWhenRootIsMissingOrWrong()
        {
            // Arrange
            MockLogger logger = new MockLogger();
            XMLLoader loader = new XMLLoader(logger, XDef.Root);
            string wrongRootXml = "<banana><item/></banana>";
            string path = Path.GetTempFileName();
            File.WriteAllText(path, wrongRootXml);

            // Act
            var result = loader.LoadXML(path, "Test");

            // Assert
            TestContext.WriteLine(result?.ToString() ?? "null");
            Assert.IsNull(result);
            Assert.That(logger.Logs[0].Message, Does.Contain(XMLLoader.errorInvalidFormat));
        }

        [Test]
        public void ShouldFailWhenXmlIsMalformed()
        {
            // Arrange
            MockLogger logger = new MockLogger();
            XMLLoader loader = new XMLLoader(logger, XDef.Root);
            string malformedXml = "<Defs><bad></Defs>"; // missing closing tag
            string path = Path.GetTempFileName();
            File.WriteAllText(path, malformedXml);

            // Act
            var result = loader.LoadXML(path, "Test");

            // Assert
            TestContext.WriteLine(result?.ToString() ?? "null");
            Assert.IsNull(result);
            Assert.That(logger.Logs[0].Message, Does.Contain(XMLLoader.errorXmlParse));
        }
    }
}
