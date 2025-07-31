using System;
using NUnit.Framework;

namespace AngusChangyiMods.Core.ModLoader.Tests
{
    [TestFixture]
    public class ModValidatorTests
    {
        [TestCase("1invalid.id")]
        [TestCase("mod@wrong")]
        [TestCase("justid")]
        [TestCase(".startDot")]
        public void Validate_ShouldThrowIfPackageIdInvalid(string badId)
        {
            var meta = new ModMetaData("Test", badId, "auth", "desc", "root");
            var ex = Assert.Throws<FormatException>(() => ModValidator.Validate(meta));
            StringAssert.Contains("Invalid packageId format", ex.Message);
        }
        
        [TestCase(null,       "UnitTest.PackageID", "TestAuthor", "TestDescription", "dir", Mod.Name)]
        [TestCase("TestName", null,                 "TestAuthor", "TestDescription", "dir", Mod.PackageId)]
        [TestCase("TestName", "UnitTest.PackageID", null,         "TestDescription", "dir", Mod.Author)]
        [TestCase("TestName", "UnitTest.PackageID", "TestAuthor", null,              "dir", Mod.Description)]
        [TestCase("TestName", "UnitTest.PackageID", "TestAuthor", "TestDescription", null,  "RootDirectory")]
        public void Constructor_ShouldThrowArgumentNullException(string name, string id, string author, string desc, string dir, string expectedParam)
        {
            // Arrange & Act
            var meta = new ModMetaData(name, id, author, desc, dir);
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => ModValidator.Validate(meta));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo(expectedParam));
        }
    }
    
    

}