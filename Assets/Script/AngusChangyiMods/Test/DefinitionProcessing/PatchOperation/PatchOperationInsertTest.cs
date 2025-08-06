using System.Xml.Linq;
using AngusChangyiMods.Core;
using AngusChangyiMods.Core.DefinitionProcessing.PatchOperation;
using NUnit.Framework;

namespace AngusChangyiMods.Core.DefinitionProcessing.PatchOperation.Test
{

    [TestFixture]
    public class PatchOperationInsertTest
    {
        [Test]
        public void Test_01_PatchOperationInsert_ShouldInsertElement()
        {
            // Arrange
            var doc = new XDocument(new XElement("root", new XElement("element", "Test")));
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
            var doc = new XDocument(new XElement("root", new XElement("element", "Test")));
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
            var doc = new XDocument(new XElement("root", new XElement("element", "Test")));
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
            var doc = new XDocument(new XElement("root", new XElement("element", "Test")));
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
            var doc = new XDocument(
                new XElement("root",
                    new XElement("things",
                        new XElement("thing",
                            new XElement("defID", "TestDef"),
                            new XElement("element", "Test")
                        ),
                        new XElement("thing",
                            new XElement("defID", "AnotherDef"),
                            new XElement("element", "Another Test")
                        )
                    )
                )
            );
            var operation = new PatchOperationInsert
            {
                xpath = "/root/things/thing[defID='TestDef']",
                value = new XElement("newElement", "New Value")
            };

            // Act
            operation.Apply(doc);

            // Assert
            var result = doc.ToString();
            var expected = new XDocument(
                new XElement("root",
                    new XElement("things",
                        new XElement("thing",
                            new XElement("defID", "TestDef"),
                            new XElement("element", "Test"),
                            new XElement("newElement", "New Value")
                        ),
                        new XElement("thing",
                            new XElement("defID", "AnotherDef"),
                            new XElement("element", "Another Test")
                        )
                    )
                )
            ).ToString();
            StringAssert.Contains(expected, result);
        }
    }
}