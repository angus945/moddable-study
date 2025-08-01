using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AngusChangyiMods.Core.Test;

namespace AngusChangyiMods.Core.ModLoader.Tests
{
    [TestFixture]
    public class ModPreloaderTests
    {
        private string tempDir;

        [SetUp]
        public void SetUp()
        {
            tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(tempDir, true);
        }

        private string CreateAboutXml(string folder, string name, string id, string author, string desc,
            string[] versions)
        {
            string aboutDirectory = Path.Combine(folder, ModPath.AboutDir);
            Directory.CreateDirectory(aboutDirectory);

            var doc = new XDocument(
                new XElement(Mod.Root,
                    new XElement(Mod.Name, name),
                    new XElement(Mod.PackageId, id),
                    new XElement(Mod.Author, author),
                    new XElement(Mod.Description, desc),
                    new XElement(Mod.SupportedVersions,
                        versions.Select(v => new XElement(Mod.Li, v))
                    )
                )
            );
            string filePath = Path.Combine(folder, ModPath.AboutFile);
            doc.Save(filePath);
            return filePath;
        }

        [Test]
        public void CreateModMetadata_ShouldLoadValidXml()
        {
            // Arrange
            string modDir = Path.Combine(tempDir, "ModA");
            Directory.CreateDirectory(modDir);

            CreateAboutXml(modDir, "CoolMod", "cool.mod", "Alice", "Test mod", new[] { "1.4", "1.5" });
            ModPreloader preloader = new ModPreloader(new MockLogger());

            // Act
            ModMetaData meta = preloader.CreateModMetadata(modDir);

            // Assert
            Assert.AreEqual("CoolMod", meta.Name);
            Assert.AreEqual("cool.mod", meta.PackageId);
            Assert.AreEqual("Alice", meta.Author);
            Assert.AreEqual("Test mod", meta.Description);
            Assert.AreEqual(modDir, meta.RootDirectory);
            CollectionAssert.AreEqual(new[] { "1.4", "1.5" }, meta.SupportedVersions);
        }

        [Test]
        public void CreateModMetadata_ShouldSetErrorWhenAboutXmlMissing()
        {
            // Arrange
            string modDir = Path.Combine(tempDir, "ModB");
            Directory.CreateDirectory(modDir);
            ModPreloader preloader = new ModPreloader(new MockLogger());

            // Act
            ModMetaData meta = preloader.CreateModMetadata(modDir);

            // Assert
            Assert.IsTrue(meta.HasError);
            Assert.That(meta.ErrorReason.StartsWith(ModPreloader.errorAboutNotFound));
        }

        [Test]
        public void CreateModMetadata_ShouldSetErrorWhenXMLParsingFails()
        {
            // Arrange
            string modFolder = Path.Combine(tempDir, "ModC");
            string aboutFolder = Path.Combine(modFolder, "About");
            Directory.CreateDirectory(aboutFolder);

            string aboutXmlPath = Path.Combine(aboutFolder, "About.xml");
            File.WriteAllText(aboutXmlPath, "<ModMeta><Broken"); // 故意寫入損毀的 XML

            ModPreloader preloader = new ModPreloader(new MockLogger());

            // Act
            ModMetaData meta = preloader.CreateModMetadata(modFolder);

            // Assert
            Assert.IsTrue(meta.HasError, "Expected metadata to have error due to XML parsing failure");
            Assert.That(meta.ErrorReason.StartsWith(ModPreloader.errorXmlParsingFailed));
        }

        [Test]
        public void PreloadAllMods_ShouldContinueOnIndividualModFailure()
        {
            // Arrange
            string validMod1 = Path.Combine(tempDir, "1_ValidMod");
            string invalidMod = Path.Combine(tempDir, "2_InvalidMod");
            string validMod2 = Path.Combine(tempDir, "3_ValidMod");
            Directory.CreateDirectory(validMod1);
            Directory.CreateDirectory(validMod2);
            Directory.CreateDirectory(invalidMod);

            CreateAboutXml(validMod1, "GoodMod1", "good1.id", "Alice", "A valid mod", new[] { "1.4" });
            // Skip invalid mod creation to simulate missing About.xml
            CreateAboutXml(validMod2, "GoodMod2", "good2.id", "Bob", "Another valid mod", new[] { "1.5" });

            var fakeLogger = new MockLogger();
            ModPreloader preloader = new ModPreloader(fakeLogger);

            // Act
            preloader.PreloadAllMods(new[] { tempDir });

            // Assert
            ModMetaData goodModMeta1 = preloader.PreLoadedMods[0].Meta;
            ModMetaData invalidModMeta = preloader.PreLoadedMods[1].Meta;
            ModMetaData goodModMeta2 = preloader.PreLoadedMods[2].Meta;

            Assert.AreEqual(3, preloader.PreLoadedMods.Count);
            Assert.IsFalse(goodModMeta1.HasError, "Expected GoodMod to be valid");
            Assert.IsTrue(invalidModMeta.HasError, "Expected InvalidMod to be invalid");
            Assert.IsFalse(goodModMeta2.HasError, "Expected GoodMod2 to be valid");
        }

        [Test]
        public void PreloadAllMods_ShouldSetErrorWhenIdRepeated()
        {
            // Arrange
            string modDir1 = Path.Combine(tempDir, "Mod1");
            string modDir2 = Path.Combine(tempDir, "Mod2");
            Directory.CreateDirectory(modDir1);
            Directory.CreateDirectory(modDir2);

            CreateAboutXml(modDir1, "ModOne", "duplicate.id", "Alice", "First mod", new[] { "1.4" });
            CreateAboutXml(modDir2, "ModTwo", "duplicate.id", "Bob", "Second mod with same ID", new[] { "1.5" });

            var fakeLogger = new MockLogger();
            ModPreloader preloader = new ModPreloader(fakeLogger);

            // Act
            preloader.PreloadAllMods(new[] { tempDir });

            // Assert
            Assert.AreEqual(2, preloader.PreLoadedMods.Count);
            Assert.IsTrue(preloader.PreLoadedMods[0].Meta.HasError, "Expected Mod1 to have error due to duplicate ID");
            Assert.IsTrue(preloader.PreLoadedMods[1].Meta.HasError, "Expected Mod2 to have error due to duplicate ID");
            Assert.That(preloader.PreLoadedMods[0].Meta.ErrorReason.StartsWith(ModPreloader.packageIdDuplicateError),
                "Expected error reason to start with packageIdDuplicateError");
            Assert.That(preloader.PreLoadedMods[1].Meta.ErrorReason.StartsWith(ModPreloader.packageIdDuplicateError),
                "Expected error reason to start with packageIdDuplicateError");
        }
    }
}