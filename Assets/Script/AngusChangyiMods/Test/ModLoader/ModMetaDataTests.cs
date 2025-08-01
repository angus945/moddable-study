using NUnit.Framework;
using System;
using AngusChangyiMods.Core;
using AngusChangyiMods.Core.ModLoader;

namespace AngusChangyiMods.Core.ModLoader.Tests
{
    [TestFixture]
    public class ModSorterTests
    {
        
    }
    
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
