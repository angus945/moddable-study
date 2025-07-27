using System.Xml.Linq;
using NUnit.Framework;
using AngusChangyiMods.Core;

namespace ModInfrastructure.Test.PatchOperation
{
    [TestFixture]
    public class Test_PatchOperationReplace
    {
        [Test]
        public void Test_01_PatchOperationReplace_ShouldReplaceElement()
        {
            // Arrange
            var xml = @"<root><element>Old Value</element></root>";
            var doc = XDocument.Parse(xml);
            var operation = new PatchOperationReplace
            {
                xpath = "/root/element",
                value = new XElement("element", "New Value")
            };

            // Act
            operation.Apply(doc);

            // Assert
            var result = doc.ToString(SaveOptions.DisableFormatting);
            var expected = "<root><element>New Value</element></root>";
            StringAssert.Contains(expected, result);
        }

        [Test]
        public void Test_02_PatchOperationReplace_ShouldNotReplaceIfTargetNotFound()
        {
            // Arrange
            var xml = @"<root><element>Old Value</element></root>";
            var doc = XDocument.Parse(xml);
            var operation = new PatchOperationReplace
            {
                xpath = "/root/nonexistent",
                value = new XElement("element", "New Value")
            };

            // Act
            operation.Apply(doc);

            // Assert
            var result = doc.ToString(SaveOptions.DisableFormatting);
            var expected = "<root><element>Old Value</element></root>";
            StringAssert.Contains(expected, result);
        }

        [Test]
        public void Test_03_PatchOperationReplace_ShouldReplaceMultipleTimes()
        {
            // Arrange
            var xml = @"<root><element>Old Value</element></root>";
            var doc = XDocument.Parse(xml);
            var operationFirst = new PatchOperationReplace
            {
                xpath = "/root/element",
                value = new XElement("element", "New Value")
            };
            var operationSecond = new PatchOperationReplace
            {
                xpath = "/root/element",
                value = new XElement("element", "Another Value")
            };

            // Act
            operationFirst.Apply(doc);
            operationSecond.Apply(doc); // Apply again to replace again

            // Assert
            var result = doc.ToString(SaveOptions.DisableFormatting);
            var expected = "<root><element>Another Value</element></root>";
            StringAssert.Contains(expected, result);
        }

        [Test]
        public void Test_04_PatchOperationReplace_ShouldReplaceComplexElement()
        {
            // Arrange
            var xml = @"<root><element><subElement>Old Value</subElement></element></root>";
            var doc = XDocument.Parse(xml);
            var operation = new PatchOperationReplace
            {
                xpath = "/root/element/subElement",
                value = new XElement("subElement", "New Value")
            };

            // Act
            operation.Apply(doc);

            // Assert
            var result = doc.ToString(SaveOptions.DisableFormatting);
            var expected = "<root><element><subElement>New Value</subElement></element></root>";
            StringAssert.Contains(expected, result);
        }

        [Test]
        public void Test_05_PatchOperationReplace_ShouldReplaceByDefID()
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
            var operation = new PatchOperationReplace
            {
                xpath = "/root/things/thing[defID='TestDef']/element",
                value = new XElement("element", "New Value")
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
                        <element>New Value</element>
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