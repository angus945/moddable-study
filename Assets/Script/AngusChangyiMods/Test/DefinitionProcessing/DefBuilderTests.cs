using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Script.AngusChangyiMods.Test.DefinitionProcessing.Old;

namespace AngusChangyiMods.Core.DefinitionProcessing.Test
{
    [TestFixture]
    public class DefBuilderTests
    {
        [Test]
        public void DefBuilder_ShouldBuild_ValidDefFile()
        {
            // Arrange
            string path = new DefBuilder()
                .WithDef<MockDefinition>("Test.Foo").Label("LabelFoo").Description("This is a test definition.")
                .AddProperty("stringProp", "Some string")
                .AddProperty("intProp", "123")
                .AddProperty("boolProp", "true")
                .ListProp("listProp", "A", "B")
                .Build();

            // Act
            XDocument doc = XDocument.Load(path);
            XElement def = doc.Root.Elements().First();

            // Assert
            Assert.AreEqual("Test.Foo", def.Element("defName")?.Value);
            Assert.AreEqual("LabelFoo", def.Element("label")?.Value);
            Assert.AreEqual("This is a test definition.", def.Element("description")?.Value);
            Assert.AreEqual("Some string", def.Element("stringProp")?.Value);
            Assert.AreEqual("123", def.Element("intProp")?.Value);
            Assert.AreEqual("true", def.Element("boolProp")?.Value);

            var listItems = def.Element("listProp")?.Elements("li").Select(li => li.Value).ToList();
            CollectionAssert.AreEqual(new[] { "A", "B" }, listItems);

            TestContext.WriteLine(doc.ToString());
        }

        [Test]
        public void DefBuilder_ShouldBuild_AbstractDefinition_WithExpectedAttributes()
        {
            // Arrange
            string path = new DefBuilder()
                .WithDef<MockDefinition>("Test.Abstract", isAbstract: true)
                .Label("Abstract Label")
                .Description("This is an abstract def")
                .Build();

            // Act
            XDocument doc = XDocument.Load(path);
            XElement def = doc.Root.Element("MockDefinition");

            // Assert
            Assert.IsNotNull(def);
            Assert.AreEqual("Test.Abstract", def.Element("defName")?.Value);
            Assert.AreEqual("Abstract Label", def.Element("label")?.Value);
            Assert.AreEqual("true", def.Attribute("abstract")?.Value);

            TestContext.WriteLine(doc.ToString());
        }

        [Test]
        public void DefBuilder_ShouldBuild_InheritedDefinition()
        {
            // Arrange
            var builder = new DefBuilder();

            builder.WithDef<MockDefinition>("Test.Abstract", isAbstract: true)
                .Label("Abstract Base");

            builder.WithDef<MockDefinition>("Test.Child")
                .InheritFrom("Test.Abstract")
                .Label("Child Label")
                .AddProperty("intProp", "42");

            string path = builder.Build();

            // Act
            XDocument doc = XDocument.Load(path);
            XElement child = doc.Root.Elements("MockDefinition")
                .FirstOrDefault(e => e.Element("defName")?.Value == "Test.Child");

            // Assert
            Assert.IsNotNull(child);
            Assert.AreEqual("Test.Abstract", child.Attribute("parent")?.Value);
            Assert.AreEqual("Child Label", child.Element("label")?.Value);
            Assert.AreEqual("42", child.Element("intProp")?.Value);

            TestContext.WriteLine(doc.ToString());
        }

        [Test]
        public void DefBuilder_ShouldBuildNestedTreeStructure()
        {
            // Arrange
            string path = new DefBuilder()
                .WithDef<MockDefinition>("Test.Advanced", isAbstract: true)
                .Label("高階定義")
                .InheritFrom("Test.Base")
                .Add(DefBuilder.Tree("stats").WithChildren(
                    DefBuilder.Tree("hp", 100),
                    DefBuilder.Tree("speed", 5.5),
                    DefBuilder.Tree("meta").WithChildren(
                        DefBuilder.Tree("tag", "elite"),
                        DefBuilder.Tree("level", 3)
                    )
                ))
                .Build();

            // Act
            var doc = XDocument.Load(path);
            var root = doc.Root.Element("MockDefinition");
            var stats = root.Element("stats");
            var meta = stats.Element("meta");

            // Assert
            TestContext.WriteLine(doc.ToString());

            Assert.IsNotNull(stats);
            Assert.AreEqual("100", stats.Element("hp")?.Value);
            Assert.AreEqual("5.5", stats.Element("speed")?.Value);
            Assert.AreEqual("elite", meta.Element("tag")?.Value);
            Assert.AreEqual("3", meta.Element("level")?.Value);
        }
    }
}