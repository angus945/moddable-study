using System.Xml.Linq;
using NUnit.Framework;
using AngusChangyiMods.Core;

namespace ModInfrastructure.Test.PatchOperation
{
    [TestFixture]
    public class Test_PatchOperationAddAfter
    {
        [Test]
        public void Test_01_PatchOperationAddAfter_ShouldAddElementAfterTarget()
        {
            // Arrange
            var xml = @"<root><element>Test</element></root>";
            var doc = XDocument.Parse(xml);
            var operation = new PatchOperationAddAfter
            {
                xpath = "/root/element",
                value = new XElement("newElement", "New Value")
            };

            // Act
            operation.Apply(doc);

            // Assert
            var result = doc.ToString(SaveOptions.DisableFormatting);
            var expected = "<root><element>Test</element><newElement>New Value</newElement></root>";
            StringAssert.Contains(expected, result);
        }

        [Test]
        public void Test_02_PatchOperationAddAfter_ShouldNotAddElementIfTargetNotFound()
        {
            // Arrange
            var xml = @"<root><element>Test</element></root>";
            var doc = XDocument.Parse(xml);
            var operation = new PatchOperationAddAfter
            {
                xpath = "/root/nonexistent",
                value = new XElement("newElement", "New Value")
            };

            // Act
            operation.Apply(doc);

            // Assert
            var result = doc.ToString(SaveOptions.DisableFormatting);
            var expected = "<root><element>Test</element></root>";
            StringAssert.Contains(expected, result);
        }

        [Test]
        public void Test_03_PatchOperationAddAfter_ShouldModifyMultipleTimes()
        {
            // Arrange
            var xml = @"<root><element>Test</element></root>";
            var doc = XDocument.Parse(xml);
            var operationFirst = new PatchOperationAddAfter
            {
                xpath = "/root/element",
                value = new XElement("newElement", "New Value")
            };
            var operationSecond = new PatchOperationAddAfter
            {
                xpath = "/root/element",
                value = new XElement("newElement", "Another Value")
            };

            // Act
            operationFirst.Apply(doc);
            operationSecond.Apply(doc); // Apply again to add another element

            // Assert
            var result = doc.ToString(SaveOptions.DisableFormatting);
            var expected = "<root><element>Test</element><newElement>Another Value</newElement><newElement>New Value</newElement></root>";
            StringAssert.Contains(expected, result);
        }

        [Test]
        public void Test_04_PatchOperationAddAfter_ShouldAddElementWithDefID()
        {
            // Arrange
            var xml = @"
            <root>
                <things>
                    <thing>
                        <defID>TestDef</defID>
                        <element>Test</element>
                    </thing>
                    <thing>
                        <defID>AnotherDef</defID>
                        <element>Another Test</element>
                    </thing>
                </things>
            </root>";
            var doc = XDocument.Parse(xml);
            var operation = new PatchOperationAddAfter
            {
                xpath = "/root/things/thing[defID='TestDef']/element",
                value = new XElement("newElement", "New Value")
            };

            // Act
            operation.Apply(doc);

            // Assert
            var result = doc.ToString();
            var expected = XDocument.Parse(@"
            <root>
                <things>
                    <thing>
                        <defID>TestDef</defID>
                        <element>Test</element>
                        <newElement>New Value</newElement>
                    </thing>
                    <thing>
                        <defID>AnotherDef</defID>
                        <element>Another Test</element>
                    </thing>
                </things>
            </root>").ToString();
            StringAssert.Contains(expected, result);
        }
    }
}