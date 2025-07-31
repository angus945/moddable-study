using System;
using NUnit.Framework;

namespace AngusChangyiMods.Core.ModLoader.Tests
{
    [TestFixture]
    public class ModValidatorTests
    {
        [Test]
        public void ShouldNotSetMetaDataErrorWhenMetadataLegal()
        {
            // Arrange
            var meta = new ModMetaData("name", "id.id", "author", "desc", "dir");

            // Act
            ModValidator.Validate(meta);

            // Assert
            Assert.IsFalse(meta.HasError);
        }
        
        [TestCase("1invalid.id")]
        [TestCase("mod@wrong")]
        [TestCase("justid")]
        [TestCase(".startDot")]
        public void ShouldSetMetaDataErrorWhenPackageIdIllegal(string badId)
        {
            // Arrange
            var meta = new ModMetaData("Test", badId, "auth", "desc", "root");
            
            // Act 
            ModValidator.Validate(meta);
            
            // Assert
            Assert.IsTrue(meta.HasError);
            Assert.AreEqual(ModValidator.packageIdFormatError, meta.ErrorReason);
        }

        [TestCase(null, "UnitTest.PackageID", "TestAuthor", "TestDescription", "dir", ModValidator.nameEmptyError)]
        [TestCase("TestName", null, "TestAuthor", "TestDescription", "dir", ModValidator.packageIdEmptyError)]
        [TestCase("TestName", "UnitTest.PackageID", null, "TestDescription", "dir", ModValidator.authorEmptyError)]
        [TestCase("TestName", "UnitTest.PackageID", "TestAuthor", null, "dir", ModValidator.descriptionEmptyError)]
        [TestCase("TestName", "UnitTest.PackageID", "TestAuthor", "TestDescription", null, ModValidator.rootDirectoryEmptyError)]
        public void ShouldSetMetaDataErrorWhenMetadataPropertyIsNull(string name, string id, string author, string desc, string dir, string expectedMessage)
        {
            // Arrange
            var meta = new ModMetaData(name, id, author, desc, dir);

            // Act
            ModValidator.Validate(meta);

            // Assert
            Assert.IsTrue(meta.HasError);
            Assert.AreEqual(expectedMessage, meta.ErrorReason);
        }
    }
    
    

}