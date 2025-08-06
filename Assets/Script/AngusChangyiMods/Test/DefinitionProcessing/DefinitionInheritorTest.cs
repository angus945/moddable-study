using System;
using System.Linq;
using System.Xml.Linq;
using AngusChangyiMods.Core.Test;
using AngusChangyiMods.Logger;
using NUnit.Framework;

namespace AngusChangyiMods.Core.DefinitionProcessing.Test
{
    [TestFixture]
    [TestOf(typeof(DefinitionInheritor))]
    public class DefinitionInheritorTest
    {
        [Test]
        public void Should_InheritPropertiesCorrectly()
        {
            // Arrange
            var logger = new MockLogger();
            var inheritor = new DefinitionInheritor(logger);

            XDocument source = new DefBuilder()
                .WithDef<MockDefinition>("Base.Example", isAbstract: true)
                .Label("基礎範例")
                .Description("這是基礎定義")
                .AddProperty("defaultValue", "42")

                .WithDef<MockDefinition>("Child.Example")
                .InheritFrom("Base.Example")
                .Label("子級範例")
                .AddProperty("childValue", "100")
                .Build();

            // Act
            var result = inheritor.ProcessInheritance(source);

            // Assert
            TestContext.WriteLine($"source: \n{source} \n result: \n{string.Join("\n", result.Select(e => e.ToString()))} \n");
            Assert.AreEqual(1, result.Count); // 只有非抽象的會被處理

            var childDef = result[0];
            Assert.AreEqual("Child.Example", childDef.Element(Def.DefName)?.Value);
            Assert.AreEqual("子級範例", childDef.Element(Def.Label)?.Value);
            Assert.AreEqual("這是基礎定義", childDef.Element(Def.Description)?.Value); // 繼承自父級
            Assert.AreEqual("42", childDef.Element("defaultValue")?.Value); // 繼承自父級
            Assert.AreEqual("100", childDef.Element("childValue")?.Value); // 子級自有

            Assert.IsNull(childDef.Attribute(Def.Parent)); // 繼承屬性應被移除
            Assert.IsNull(childDef.Attribute(Def.IsAbstract));
        }

        [Test]
        public void Should_MergeListValues()
        {
            // Arrange
            var logger = new MockLogger();
            var inheritor = new DefinitionInheritor(logger);

            XDocument source = new DefBuilder()
                .WithDef<MockDefinition>("Base.Tagged", isAbstract: true)
                .AddList("tags", "common", "base")

                .WithDef<MockDefinition>("Child.Tagged")
                .InheritFrom("Base.Tagged")
                .AddList("tags", "child", "specific", "common") // 包含重複項目
                .Build();

            // Act
            var result = inheritor.ProcessInheritance(source);

            // Assert
            TestContext.WriteLine($"source: \n{source} \n result: \n{string.Join("\n", result.Select(e => e.ToString()))} \n");

            var childDef = result[0];
            var tagElements = childDef.Element("tags")?.Elements().Select(e => e.Value).ToArray();

            // 應該合併且去重
            Assert.IsNotNull(tagElements);
            Assert.That(tagElements, Contains.Item("common"));
            Assert.That(tagElements, Contains.Item("base"));
            Assert.That(tagElements, Contains.Item("child"));
            Assert.That(tagElements, Contains.Item("specific"));

            // 檢查沒有重複
            Assert.AreEqual(tagElements.Distinct().Count(), tagElements.Length);
        }

        [Test]
        public void Should_HandleMultiLevelInheritance()
        {
            // Arrange
            var logger = new MockLogger();
            var inheritor = new DefinitionInheritor(logger);

            XDocument source = new DefBuilder()
                .WithDef<MockDefinition>("Root.Base", isAbstract: true)
                .Label("根定義")
                .AddProperty("rootValue", "1")

                .WithDef<MockDefinition>("Middle.Base", isAbstract: true)
                .InheritFrom("Root.Base")
                .Label("中間定義")
                .AddProperty("middleValue", "2")

                .WithDef<MockDefinition>("Final.Concrete")
                .InheritFrom("Middle.Base")
                .Label("最終定義")
                .AddProperty("finalValue", "3")
                .Build();

            // Act
            var result = inheritor.ProcessInheritance(source);

            // Assert
            TestContext.WriteLine($"source: \n{source} \n result: \n{string.Join("\n", result.Select(e => e.ToString()))} \n");
            Assert.AreEqual(1, result.Count);

            var finalDef = result[0];
            Assert.AreEqual("Final.Concrete", finalDef.Element(Def.DefName)?.Value);
            Assert.AreEqual("最終定義", finalDef.Element(Def.Label)?.Value);

            // 應該繼承所有層級的屬性
            Assert.AreEqual("1", finalDef.Element("rootValue")?.Value);
            Assert.AreEqual("2", finalDef.Element("middleValue")?.Value);
            Assert.AreEqual("3", finalDef.Element("finalValue")?.Value);
        }

        [Test]
        public void Should_OverrideParentProperty_InMultiLevelInheritance()
        {
            // Arrange
            var logger = new MockLogger();
            var inheritor = new DefinitionInheritor(logger);

            XDocument source = new DefBuilder()
                .WithDef<MockDefinition>("Root.Base", isAbstract: true)
                .Label("根標籤")
                .AddProperty("value", "original")

                .WithDef<MockDefinition>("Middle.Base", isAbstract: true)
                .InheritFrom("Root.Base")
                .AddProperty("value", "override") // 覆蓋父級屬性

                .WithDef<MockDefinition>("Final.Concrete")
                .InheritFrom("Middle.Base")
                .Label("最終標籤")
                .Build();

            // Act
            var result = inheritor.ProcessInheritance(source);

            // Assert
            TestContext.WriteLine($"source: \n{source} \n result: \n{string.Join("\n", result.Select(e => e.ToString()))} \n");
            var finalDef = result[0];

            // 子級的覆蓋應該優先
            Assert.AreEqual("override", finalDef.Element("value")?.Value);
            Assert.AreEqual("最終標籤", finalDef.Element(Def.Label)?.Value); // 最終的覆蓋
        }

        [Test]
        public void Should_HandleComplexNestedElementsInheritance()
        {
            // Arrange
            var logger = new MockLogger();
            var inheritor = new DefinitionInheritor(logger);

            XDocument source = new DefBuilder()
                .WithDef<MockDefinition>("Base.Complex", isAbstract: true)
                .AddNested(DefBuilder.Tree("stats").WithChildren(
                    DefBuilder.Tree("health", "100"),
                    DefBuilder.Tree("armor", "5")
                ))

                .WithDef<MockDefinition>("Child.Complex")
                .InheritFrom("Base.Complex")
                .AddNested(DefBuilder.Tree("stats").WithChildren(
                    DefBuilder.Tree("health", "150"), // 覆蓋父級
                    DefBuilder.Tree("speed", "10")     // 新增屬性
                ))
                .Build();

            // Act
            var result = inheritor.ProcessInheritance(source);

            // Assert
            TestContext.WriteLine($"source: \n{source} \n result: \n{string.Join("\n", result.Select(e => e.ToString()))} \n");
            var childDef = result[0];
            var stats = childDef.Element("stats");

            Assert.IsNotNull(stats);
            Assert.AreEqual("150", stats.Element("health")?.Value); // 子級覆蓋
            Assert.AreEqual("10", stats.Element("speed")?.Value);   // 子級新增
        }

        [Test]
        public void Should_HandleDefExtensionInheritance()
        {
            // Arrange
            var logger = new MockLogger();
            var inheritor = new DefinitionInheritor(logger);

            XDocument source = new DefBuilder()
                .WithDef<MockDefinition>("Base.WithExt", isAbstract: true)
                .AddExtension<MockExtension>(
                    DefBuilder.Tree("baseFlag", "true"),
                    DefBuilder.Tree("value", "10")
                )

                .WithDef<MockDefinition>("Child.WithExt")
                .InheritFrom("Base.WithExt")
                .AddExtension<AnotherMockExtension>(
                    DefBuilder.Tree("childFlag", "false")
                )
                .Build();

            // Act
            var result = inheritor.ProcessInheritance(source);

            // Assert
            TestContext.WriteLine($"source: \n{source} \n result: \n{string.Join("\n", result.Select(e => e.ToString()))} \n");
            var childDef = result[0];
            var extensions = childDef.Element(Def.Extensions);

            Assert.IsNotNull(extensions);
            var extList = extensions.Elements(Def.Li).ToList();
            Assert.AreEqual(2, extList.Count); // 應該有兩個擴展

            // 驗證父級擴展被繼承
            var parentExt = extList.FirstOrDefault(e =>
                e.Attribute(Def.Class)?.Value == typeof(MockExtension).FullName);
            Assert.IsNotNull(parentExt);
            Assert.AreEqual("true", parentExt.Element("baseFlag")?.Value);
            Assert.AreEqual("10", parentExt.Element("value")?.Value);

            // 驗證子級擴展存在
            var childExt = extList.FirstOrDefault(e =>
                e.Attribute(Def.Class)?.Value == typeof(AnotherMockExtension).FullName);
            Assert.IsNotNull(childExt);
            Assert.AreEqual("false", childExt.Element("childFlag")?.Value);
        }

        [Test]
        public void Should_HandleDefComponentInheritance()
        {
            // Arrange
            var logger = new MockLogger();
            var inheritor = new DefinitionInheritor(logger);

            XDocument source = new DefBuilder()
                .WithDef<MockDefinition>("Base.WithComp", isAbstract: true)
                .AddComponent<MockComponent>(
                    DefBuilder.Tree("range", "15"),
                    DefBuilder.Tree("cooldown", "2.0")
                )

                .WithDef<MockDefinition>("Child.WithComp")
                .InheritFrom("Base.WithComp")
                .AddComponent<AnotherMockComponent>()
                .Build();

            // Act
            var result = inheritor.ProcessInheritance(source);

            // Assert
            TestContext.WriteLine($"source: \n{source} \n result: \n{string.Join("\n", result.Select(e => e.ToString()))} \n");

            var childDef = result[0];
            var components = childDef.Element(Def.Components);

            Assert.IsNotNull(components);
            var compList = components.Elements(Def.Li).ToList();
            Assert.AreEqual(2, compList.Count); // 應該有兩個組件

            // 驗證父級組件被繼承
            var parentComp = compList.FirstOrDefault(c =>
                c.Attribute(Def.Class)?.Value == typeof(MockComponent).FullName);
            Assert.IsNotNull(parentComp);
            Assert.AreEqual("15", parentComp.Element("range")?.Value);
            Assert.AreEqual("2.0", parentComp.Element("cooldown")?.Value);

            // 驗證子級組件存在
            var childComp = compList.FirstOrDefault(c =>
                c.Element(Def.Class)?.Value == typeof(AnotherMockComponent).FullName);
            Assert.IsNotNull(childComp);
        }

        [Test]
        public void Should_Not_AffectDeferenceDefinitionTypes()
        {
            // Arrange
            var logger = new MockLogger();
            var inheritor = new DefinitionInheritor(logger);

            // 建立包含不同定義類型的文檔，確保它們不會互相影響
            XDocument source = new XDocument(new XElement(Def.Root,
                new XElement("MockDefinition",
                    new XElement(Def.DefName, "Mock.Parent"),
                    new XAttribute(Def.IsAbstract, "true"),
                    new XElement(Def.Label, "Mock Parent")
                ),
                new XElement("MockDefinition",
                    new XElement(Def.DefName, "Mock.Child"),
                    new XAttribute(Def.Parent, "Mock.Parent"),
                    new XElement(Def.Label, "Mock Child")
                ),
                new XElement("AnotherDefinitionType",
                    new XElement(Def.DefName, "Another.Child"),
                    new XAttribute(Def.Parent, "Mock.Parent"), // 嘗試繼承不同類型的父級
                    new XElement(Def.Label, "Another Child")
                )
            ));

            // Act
            var result = inheritor.ProcessInheritance(source);

            // Assert
            TestContext.WriteLine($"source: \n{source} \n result: \n{string.Join("\n", result.Select(e => e.ToString()))} \n");

            Assert.AreEqual(1, result.Count); // 只有 MockDefinition Mock.Child 會被成功處理

            var mockChild = result.FirstOrDefault(e =>
                e.Name == "MockDefinition" && e.Element(Def.DefName)?.Value == "Mock.Child");

            Assert.IsNotNull(mockChild);

            // MockDefinition 應該成功繼承同類型的父級
            Assert.AreEqual("Mock Child", mockChild.Element(Def.Label)?.Value);

            // 應該有警告日誌表示 AnotherDefinitionType 找不到父級
            Assert.That(logger.Logs.Any(log => log.Message.Contains(DefinitionInheritor.errorParentNotFound)));
        }

        [Test]
        public void Should_DetectAndSkipCircularInheritance()
        {
            // Arrange
            var logger = new MockLogger();
            var inheritor = new DefinitionInheritor(logger);

            // 建立循環繼承：A → B → C → A
            XDocument source = new DefBuilder()
                .WithDef<MockDefinition>("Circular.A")
                    .InheritFrom("Circular.C")
                    .Label("Node A")
                .WithDef<MockDefinition>("Circular.B")
                    .InheritFrom("Circular.A")
                    .Label("Node B")
                .WithDef<MockDefinition>("Circular.C")
                    .InheritFrom("Circular.B")
                    .Label("Node C")
                .Build();

            // Act
            var result = inheritor.ProcessInheritance(source);

            // Assert
            TestContext.WriteLine($"source: \n{source} \n result: \n{string.Join("\n", result.Select(e => e.ToString()))} \n");

            // 循環繼承應該被偵測到，相關的定義應該被跳過
            Assert.AreEqual(0, result.Count, "循環繼承的定義應該被跳過");

            // 應該有錯誤日誌記錄循環繼承
            Assert.That(logger.Logs.Any(log =>
                log.Level == LogLevel.Error &&
                log.Message.Contains(DefinitionInheritor.errorCircularReference)));
        }

        [Test]
        public void Should_Skip_When_ParentMissing()
        {
            // Arrange
            var logger = new MockLogger();
            var inheritor = new DefinitionInheritor(logger);

            XDocument source = new DefBuilder()
                .WithDef<MockDefinition>("Orphan.Child")
                .InheritFrom("Missing.Parent") // 父級不存在
                .Label("孤兒定義")
                .AddProperty("value", "orphan")
                .Build();

            // Act
            var result = inheritor.ProcessInheritance(source);

            // Assert
            TestContext.WriteLine($"source: \n{source} \n result: \n{string.Join("\n", result.Select(e => e.ToString()))} \n");
            // 找不到父級的定義應該被跳過
            Assert.AreEqual(0, result.Count, "找不到父級的定義應該被跳過");

            // 應該有警告日誌記錄找不到父級
            Assert.That(logger.Logs.Any(log =>
                log.Level == LogLevel.Warning &&
                log.Message.Contains(DefinitionInheritor.errorParentNotFound)));

            // 驗證日誌包含正確的定義名稱
            var warningLog = logger.Logs.FirstOrDefault(log =>
                log.Message.Contains("Missing.Parent") &&
                log.Message.Contains("Orphan.Child"));
            Assert.IsNotNull(warningLog, "警告日誌應該包含父級和子級的名稱");
        }


    }
}
