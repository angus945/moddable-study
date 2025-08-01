using System.Collections.Generic;
using NUnit.Framework;
using Script.AngusChangyiMods.Core.Util;

namespace AngusChangyiMods.Core.SaveLoad.Test
{
    [TestFixture]
    public class TypeFileNameConverterTests
    {
        class DummyClass
        {
            
        }

        class Outer
        {
            public class Inner
            {
            }
        }
        
        [Test]
        public void ShouldGenerateSimpleFileName()
        {
            string fileName = TypeFileNameConverter.GetFileNameForType(typeof(DummyClass), ".xml");
    
            Assert.That(fileName, Does.Contain("DummyClass"));
            Assert.That(fileName, Does.EndWith(".xml"));
            
        }


        [Test]
        public void ShouldGenerateFileName_ForNestedClass()
        {
            string fileName = TypeFileNameConverter.GetFileNameForType(typeof(Outer.Inner), ".xml");
            Assert.That(fileName, Does.Contain("Outer.Inner"));
            Assert.That(fileName, Does.EndWith(".xml"));

        }

        [Test]
        public void ShouldGenerateFileName_ForGenericType()
        {
            string fileName = TypeFileNameConverter.GetFileNameForType(typeof(List<string>), ".xml");
            Assert.That(fileName, Does.Contain("System.Collections.Generic.List"));
            Assert.That(fileName, Does.Contain("[System.String]"));
            Assert.That(fileName, Does.EndWith(".xml"));
        }

        [Test]
        public void ShouldSanitizeInvalidChars()
        {
            var fileName = TypeFileNameConverter.GetFileNameForType(typeof(Dictionary<int, string>), ".xml");
            Assert.That(fileName, Does.Not.Contain("<"));
            Assert.That(fileName, Does.Not.Contain(">"));
            Assert.That(fileName, Does.Not.Contain(":"));
            Assert.That(fileName, Does.EndWith(".xml"));
        }
    }
}