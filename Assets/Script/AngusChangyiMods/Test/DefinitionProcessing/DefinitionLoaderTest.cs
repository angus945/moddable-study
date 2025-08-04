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
        private DefinitionLoader loader;
        private MockLogger logger;

        [SetUp]
        public void SetUp()
        {
            logger = new MockLogger();
            loader = new DefinitionLoader(logger);
        }

        [Test]
        public void ShouldLoadValidDefinition_WithSourceInfo()
        {
            // Arrange
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
            string fakePath = Path.Combine(Path.GetTempPath(), "missing_" + Guid.NewGuid() + ".xml");

            // Act
            var result = loader.LoadDefinition(fakePath, "Test");

            // Assert
            TestContext.WriteLine(result?.ToString() ?? "null");
            Assert.IsNull(result);
            Assert.That(logger.Logs[0].Message, Does.Contain(DefinitionLoader.warningFileNotFound));
        }

        [Test]
        public void ShouldFailWhenRootIsEmpty()
        {
            // Arrange
            string path = XmlTestUtil.SaveToTempFile(new XDocument(new XElement(Def.Root)));

            // Act
            var result = loader.LoadDefinition(path, "Test");

            // Assert
            TestContext.WriteLine(result.ToString());
            Assert.IsNotNull(result);
            Assert.IsEmpty(result.Root.Elements());
        }

        [Test]
        public void ShouldFailWhenRootIsMissingOrWrong()
        {
            // Arrange
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
