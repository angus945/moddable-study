using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace AngusChangyiMods.Core.DefinitionProcessing.PatchOperation.Test
{
    [TestFixture]
    public class PatchBuilderTests
    {
        [Test]
        public void ShouldBuild_AddAfterOperation_WithNestedValue()
        {
            // Arrange
            var builder = new PatchBuilder()
                .WithPatch<PatchOperationAddAfter>()
                    .XPath("/root/target")
                    .Value(
                        TreeNode.Tree("damage", "100"),
                        TreeNode.Tree("cooldown", "3.5")
                    );

            // Act
            XDocument doc = builder.Build();
            var op = doc.Root.Element(XPatch.Operation);

            // Assert
            TestContext.WriteLine(doc.ToString());

            Assert.AreEqual(nameof(PatchOperationAddAfter), op?.Attribute(XPatch.Class)?.Value);
            Assert.AreEqual("/root/target", op?.Element(XPatch.XPath)?.Value);
            Assert.AreEqual("damage", op?.Element(XPatch.Value)?.Element("damage")?.Name.LocalName);
            Assert.AreEqual("100", op?.Element(XPatch.Value)?.Element("damage")?.Value);
        }

        [Test]
        public void ShouldBuild_InsertOperation_WithXElementValue()
        {
            // Arrange
            var builder = new PatchBuilder()
                .WithPatch<PatchOperationInsert>()
                    .XPath("/root/another")
                    .Value(new XElement("newElement", "abc"));

            // Act
            XDocument doc = builder.Build();
            var op = doc.Root.Element(XPatch.Operation);

            // Assert
            TestContext.WriteLine(doc.ToString());

            Assert.AreEqual(nameof(PatchOperationInsert), op?.Attribute(XPatch.Class)?.Value);
            Assert.AreEqual("/root/another", op?.Element(XPatch.XPath)?.Value);
            Assert.AreEqual("abc", op?.Element(XPatch.Value)?.Element("newElement")?.Value);
        }

        [Test]
        public void ShouldBuild_MultipleOperations()
        {
            // Arrange
            var builder = new PatchBuilder()
                .WithPatch<PatchOperationInsert>()
                    .XPath("/root/one")
                    .Value(new XElement("item", "A"))
                .WithPatch<PatchOperationReplace>()
                    .XPath("/root/two")
                    .Value(new XElement("item", "B"));

            // Act
            XDocument doc = builder.Build();
            var ops = doc.Root.Elements(XPatch.Operation).ToList();

            // Assert
            Assert.AreEqual(2, ops.Count);

            Assert.AreEqual(nameof(PatchOperationInsert), ops[0].Attribute(XPatch.Class)?.Value);
            Assert.AreEqual("/root/one", ops[0].Element(XPatch.XPath)?.Value);
            Assert.AreEqual("A", ops[0].Element(XPatch.Value)?.Element("item")?.Value);

            Assert.AreEqual(nameof(PatchOperationReplace), ops[1].Attribute(XPatch.Class)?.Value);
            Assert.AreEqual("/root/two", ops[1].Element(XPatch.XPath)?.Value);
            Assert.AreEqual("B", ops[1].Element(XPatch.Value)?.Element("item")?.Value);

            TestContext.WriteLine(doc.ToString());
        }


        [Test]
        public void ShouldBuild_DeeplyNestedValueStructure()
        {
            // Arrange
            var builder = new PatchBuilder()
                .WithPatch<PatchOperationAddAfter>()
                    .XPath("/game/stats")
                    .Value(
                        TreeNode.Tree("combat").WithChildren(
                            TreeNode.Tree("hp", "120"),
                            TreeNode.Tree("armor", "steel"),
                            TreeNode.Tree("weapons").WithChildren(
                                TreeNode.Tree("primary", "sword"),
                                TreeNode.Tree("secondary", "bow")
                            )
                        )
                    );

            // Act
            XDocument doc = builder.Build();
            var op = doc.Root.Element(XPatch.Operation);

            // Assert
            var value = op.Element(XPatch.Value);
            var combat = value?.Element("combat");
            var weapons = combat?.Element("weapons");

            Assert.AreEqual("120", combat?.Element("hp")?.Value);
            Assert.AreEqual("steel", combat?.Element("armor")?.Value);
            Assert.AreEqual("sword", weapons?.Element("primary")?.Value);
            Assert.AreEqual("bow", weapons?.Element("secondary")?.Value);

            TestContext.WriteLine(doc.ToString());
        }

    }
}
