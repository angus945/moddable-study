using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AngusChangyiMods.Core;
using AngusChangyiMods.Logger;
using AngusChangyiMods.Core.ModLoader;

namespace AngusChangyiMods.Core.ModLoader.Tests
{
    public class FakeLogger : ILogger
    {
        public List<LogInfo> Logs = new();
        public void Log(LogInfo logInfo)
        {
            Logs.Add(logInfo);
        }
    }

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

        private string CreateAboutXml(string folder, string name, string id, string author, string desc, string[] versions)
        {
            string aboutDirectory = Path.Combine(folder, ModDirectory.AboutDir);
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
            string filePath = Path.Combine(folder, ModDirectory.About);
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
            ModPreloader preloader = new ModPreloader(new FakeLogger());

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
        public void CreateModMetadata_ShouldThrowWhenAboutXmlMissing()
        {
            // Arrange
            string modDir = Path.Combine(tempDir, "ModB");
            Directory.CreateDirectory(modDir);
            ModPreloader preloader = new ModPreloader(new FakeLogger());

            // Act & Assert
            var ex = Assert.Throws<FileNotFoundException>(() =>
                preloader.CreateModMetadata(modDir));

            StringAssert.Contains("About.xml not found", ex.Message);
        }
        
        [Test]
        public void PreloadAllMods_ShouldContinueOnIndividualModFailure()
        {
            // Arrange
            string validMod = Path.Combine(tempDir, "ValidMod");
            string invalidMod = Path.Combine(tempDir, "InvalidMod");
            Directory.CreateDirectory(validMod);
            Directory.CreateDirectory(invalidMod);

            CreateAboutXml(validMod, "GoodMod", "good.id", "Alice", "A valid mod", new[] { "1.4" });
            
            // InvalidMod 沒有建立 About.xml

            var fakeLogger = new FakeLogger();
            ModPreloader preloader = new ModPreloader(fakeLogger);

            // Act
            preloader.PreloadAllMods(new[] { tempDir });

            // Assert
            Assert.AreEqual(1, preloader.PreLoadedMods.Count);
            Assert.AreEqual("GoodMod", preloader.PreLoadedMods[0].Meta.Name);

            Assert.That(fakeLogger.Logs.Any(msg =>
                msg.Level == LogLevel.Warning && msg.Contains("InvalidMod")), "Expected warning log for invalid mod");

            Assert.That(fakeLogger.Logs.Any(msg =>
                msg.Level == LogLevel.Info && msg.Contains("GoodMod")), "Expected info log for valid mod");
        }
        
    }
}
