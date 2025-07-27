using System.Xml.Linq;
using AngusChangyiMods.Core;
using NUnit.Framework;

namespace Script.AngusChangyiMods.Test.DefinitionProcessing.Old.PatchOperation
{

    [TestFixture]
    public class Test_PatchOperationInsert
    {
        [Test]
        public void Test_01_PatchOperationInsert_ShouldInsertElement()
        {
            // Arrange
            var xml = @"<root><element>Test</element></root>";
            var doc = XDocument.Parse(xml);
            var operation = new PatchOperationInsert
            {
                xpath = "/root",
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
        public void Test_02_PatchOperationInsert_ShouldInsertInElement()
        {
            // Arrange
            var xml = @"<root><element>Test</element></root>";
            var doc = XDocument.Parse(xml);
            var operation = new PatchOperationInsert
            {
                xpath = "/root",
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
        public void Test_03_PatchOperationInsert_ShouldInsertMultipleElements()
        {
            // Arrange
            var xml = @"<root><element>Test</element></root>";
            var doc = XDocument.Parse(xml);
            var operationA = new PatchOperationInsert
            {
                xpath = "/root",
                value = new XElement("newElement", "New Value")
            };
            var operationB = new PatchOperationInsert
            {
                xpath = "/root",
                value = new XElement("newElement", "Another Value")
            };

            // Act
            operationA.Apply(doc);
            operationB.Apply(doc); // Apply again to insert another element

            // Assert
            var result = doc.ToString(SaveOptions.DisableFormatting);
            var expected = "<root><element>Test</element><newElement>New Value</newElement><newElement>Another Value</newElement></root>";
            StringAssert.Contains(expected, result);
        }

        [Test]
        public void Test_04_PatchOperationInsert_ShouldNotInsertIfTargetNotFound()
        {
            // Arrange
            var xml = @"<root><element>Test</element></root>";
            var doc = XDocument.Parse(xml);
            var operation = new PatchOperationInsert
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
        public void Test_05_PatchOperationInsert_ShouldInsertByDefID()
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
            var operation = new PatchOperationInsert
            {
                xpath = "/root/things/thing[defID='TestDef']",
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