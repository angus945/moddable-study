
using System;
using System.Xml.Linq;
using NUnit.Framework;

namespace AngusChangyiMods.Core.DefinitionProcessing.PatchOperation.Test
{

    [TestFixture]
    public class PatchOperationFactoryTest
    {
        [Test]
        public void ShouldCreateAddAfterOperationWithCorrectFields()
        {
            // Arrange
            var xml = XElement.Parse(@"
            <Operation Class='PatchOperationAddAfter'>
                <xpath>/root/element</xpath>
                <value><newElement>Inserted</newElement></value>
            </Operation>");

            var factory = new PatchOperationFactory();

            // Act
            var operation = factory.CreateOperation(xml);

            // Assert
            Assert.IsInstanceOf<PatchOperationAddAfter>(operation);
            var casted = (PatchOperationAddAfter)operation;
            Assert.AreEqual("/root/element", casted.xpath);
            Assert.AreEqual("newElement", casted.value.Name.LocalName);
            Assert.AreEqual("Inserted", casted.value.Value);
        }

        [Test]
        public void ShouldThrowIfClassMissing()
        {
            var xml = XElement.Parse(@"
            <Operation>
                <xpath>/root</xpath>
                <value><el>V</el></value>
            </Operation>");

            var factory = new PatchOperationFactory();

            Assert.Throws<ArgumentException>(() => factory.CreateOperation(xml));
        }

        [Test]
        public void ShouldThrowIfClassNotFound()
        {
            var xml = XElement.Parse(@"
            <Operation Class='NotExistClass'>
                <xpath>/root</xpath>
                <value><el>V</el></value>
            </Operation>");

            var factory = new PatchOperationFactory();

            var ex = Assert.Throws<ArgumentException>(() => factory.CreateOperation(xml));
            StringAssert.Contains("Could not find a patch operation class", ex.Message);
        }
    }
}
