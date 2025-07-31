using NUnit.Framework;
using System;
using AngusChangyiMods.Core;

namespace AngusChangyiMods.Tests
{
    [TestFixture]
    public class ModMetaDataTests
    {
        private const string TestName = "Test Mod";
        private const string TestPackageId = "test.mod.package";
        private const string TestAuthor = "Tester";
        private const string TestDescription = "This is a test mod.";
        private const string TestRootDir = "C:/Mods/TestMod";

        [Test]
        public void Constructor_ShouldInitializeAllFields()
        {
            // Arrange & Act
            var meta = new ModMetaData(TestName, TestPackageId, TestAuthor, TestDescription, TestRootDir);

            // Assert
            Assert.AreEqual(TestName, meta.Name);
            Assert.AreEqual(TestPackageId, meta.PackageId);
            Assert.AreEqual(TestAuthor, meta.Author);
            Assert.AreEqual(TestDescription, meta.Description);
            Assert.AreEqual(TestRootDir, meta.RootDirectory);
            Assert.IsNotNull(meta.SupportedVersions);
            Assert.IsEmpty(meta.SupportedVersions);
        }

        [TestCase(null,     Mod.PackageId, Mod.Author, Mod.Description, "dir", Mod.Name)]
        [TestCase(Mod.Name, null,          Mod.Author, Mod.Description, "dir", Mod.PackageId)]
        [TestCase(Mod.Name, Mod.PackageId, null,       Mod.Description, "dir", Mod.Author)]
        [TestCase(Mod.Name, Mod.PackageId, Mod.Author, null,            "dir", Mod.Description)]
        [TestCase(Mod.Name, Mod.PackageId, Mod.Author, Mod.Description, null,  "rootDirectory")]
        public void Constructor_ShouldThrowArgumentNullException(string name, string id, string author, string desc, string dir, string expectedParam)
        {
            // Arrange & Act
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() =>
                new ModMetaData(name, id, author, desc, dir));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo(expectedParam));
        }

        [Test]
        public void ToString_ShouldReturnExpectedFormat()
        {
            // Arrange & Act
            var meta = new ModMetaData(TestName, TestPackageId, TestAuthor, TestDescription, TestRootDir);
            string result = meta.ToString();

            // Assert
            Assert.AreEqual("Test Mod (test.mod.package) by Tester", result);
        }
    }
}
