using System;
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
                .AddList("listProp", "A", "B")
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
                .AddNested(DefBuilder.Tree("stats").WithChildren(
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
        
        [Test]
        public void DefBuilder_ShouldBuildExtension()
        {
            // Arrange
            string path = new DefBuilder()
                .WithDef<MockDefinition>("Test.WithExtension")
                .Label("擴充範例")
                .AddExtension<MockExtension>(
                    DefBuilder.Tree("canSwim", "true"),
                    DefBuilder.Tree("extraDamage", "5")
                )
                .Build();

            // Act
            var doc = XDocument.Load(path);
            var def = doc.Root.Element("MockDefinition");
            var extensions = def.Element(Def.Extensions);
            var li = extensions?.Element(Def.Li);

            // Assert
            TestContext.WriteLine(doc.ToString());
            
            Assert.IsNotNull(extensions);
            Assert.AreEqual(typeof(MockExtension).FullName, li?.Attribute(Def.Class)?.Value);
            Assert.AreEqual("true", li.Element("canSwim")?.Value);
            Assert.AreEqual("5", li.Element("extraDamage")?.Value);
        }
        
        [Test]
        public void DefBuilder_ShouldBuildComponent()
        {
            // Arrange
            string path = new DefBuilder()
                .WithDef<MockDefinition>("Test.WithComponent")
                .Label("組件範例")
                .AddComponent<MockComponent>(
                    DefBuilder.Tree("range", "12"),
                    DefBuilder.Tree("cooldown", "3.5")
                )
                .Build();

            // Act
            var doc = XDocument.Load(path);
            var def = doc.Root.Element(nameof(MockDefinition));
            var comps = def.Element(Def.Components);
            var li = comps?.Element(Def.Li);

            // Assert
            Assert.IsNotNull(comps);
            Assert.AreEqual(typeof(MockComponent).FullName, li?.Attribute(Def.Class)?.Value);
            Assert.AreEqual("12", li.Element("range")?.Value);
            Assert.AreEqual("3.5", li.Element("cooldown")?.Value);

            TestContext.WriteLine(doc.ToString());
        }
        
        [Test]
        public void DefBuilder_ShouldBuildComponent_WithoutParameters()
        {
            // Arrange
            string path = new DefBuilder()
                .WithDef<MockDefinition>("Test.NoParams")
                .Label("無參數元件")
                .AddComponent<MockComponent>()
                .Build();

            // Act
            var doc = XDocument.Load(path);
            var def = doc.Root.Element(nameof(MockDefinition));
            var comps = def.Element(Def.Components);
            var li = comps?.Element(Def.Li);
            var compClass = li?.Element(Def.Class)?.Value;

            // Assert
            Assert.IsNotNull(comps);
            Assert.AreEqual(typeof(MockComponent).FullName, compClass);

            TestContext.WriteLine(doc.ToString());
        }

        [Test]
        public void DefBuilder_ShouldBuildMultipleCompsAndExtensions()
        {
            // Arrange
            string path = new DefBuilder()
                .WithDef<MockDefinition>("Test.Multi")
                .Label("複合測試")

                // --- Components ---
                .AddComponent<MockComponent>(
                    DefBuilder.Tree("range", "10"),
                    DefBuilder.Tree("cooldown", "2.5")
                )
                .AddComponent<AnotherMockComponent>() // 沒有參數，使用 compClass

                // --- Extensions ---
                .AddExtension<MockExtension>(
                    DefBuilder.Tree("canSwim", "true"),
                    DefBuilder.Tree("bonusSpeed", "0.2")
                )
                .AddExtension<AnotherMockExtension>(
                    DefBuilder.Tree("tag", "beast"),
                    DefBuilder.Tree("danger", "high")
                )

                .Build();

            // Act
            var doc = XDocument.Load(path);
            var def = doc.Root.Element(nameof(MockDefinition));

            var comps = def.Element(Def.Components);
            var compList = comps?.Elements(Def.Li).ToList();

            var exts = def.Element(Def.Extensions);
            var extList = exts?.Elements(Def.Li).ToList();

            // Assert
            Assert.AreEqual(2, compList?.Count);
            Assert.AreEqual(typeof(MockComponent).FullName, compList[0].Attribute(Def.Class)?.Value);
            Assert.AreEqual(typeof(AnotherMockComponent).FullName, compList[1].Element(Def.Class)?.Value);

            Assert.AreEqual(2, extList?.Count);
            Assert.AreEqual(typeof(MockExtension).FullName, extList[0].Attribute(Def.Class)?.Value);
            Assert.AreEqual("true", extList[0].Element("canSwim")?.Value);
            Assert.AreEqual("0.2", extList[0].Element("bonusSpeed")?.Value);

            Assert.AreEqual(typeof(AnotherMockExtension).FullName, extList[1].Attribute(Def.Class)?.Value);
            Assert.AreEqual("beast", extList[1].Element("tag")?.Value);
            Assert.AreEqual("high", extList[1].Element("danger")?.Value);

            TestContext.WriteLine(doc.ToString());
        }

        [Test]
        public void DefBuilder_ShouldThrow_When_AddComponentWithoutDef()
        {
            var builder = new DefBuilder();

            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                builder.AddComponent<MockComponent>(
                    DefBuilder.Tree("value", "1")
                );
            });

            StringAssert.Contains("WithDef", ex.Message);
        }
        
        [Test]
        public void DefBuilder_ShouldThrow_When_ComponentClassDuplicated()
        {
            var builder = new DefBuilder()
                .WithDef<MockDefinition>("Test.Duplicated");

            builder.AddComponent<MockComponent>(
                DefBuilder.Tree("range", "10")
            );

            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                builder.AddComponent<MockComponent>(
                    DefBuilder.Tree("cooldown", "5")
                );
            });

            StringAssert.Contains("already added", ex.Message);
        }

        [Test]
        public void DefBuilder_ShouldThrow_When_ExtensionClassDuplicated()
        {
            var builder = new DefBuilder()
                .WithDef<MockDefinition>("Test.Duplicated");

            builder.AddExtension<MockExtension>(
                DefBuilder.Tree("range", "10")
            );

            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                builder.AddExtension<MockExtension>(
                    DefBuilder.Tree("cooldown", "5")
                );
            });

            StringAssert.Contains("already added", ex.Message);
        }



    }
}