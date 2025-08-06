using System.Xml.Linq;
using NUnit.Framework;
using AngusChangyiMods.Core.Test;

namespace AngusChangyiMods.Core.DefinitionProcessing.Test
{
    [TestFixture]
    [TestOf(typeof(DefinitionVarifier))]
    public class DefinitionVarifierTest
    {
        [Test]
        public void ShouldPass_When_AllFormatsAreValid()
        {
            // Arrange
            var logger = new MockLogger();
            var varifier = new DefinitionVarifier(logger);

            XElement element = new DefBuilder()
                .WithDef<MockDefinition>("MyMod.Valid")
                .InheritFrom("Base.Def") // parent 合法格式
                .AddComponent<MockComponent>(
                    TreeNode.Tree("range", "5")
                )
                .AddExtension<MockExtension>(
                    TreeNode.Tree("flag", "true")
                )
                .Build()
                .Root
                .Element(nameof(MockDefinition));

            // Act
            bool result = varifier.VerifyDefinitions(element);

            // Assert
            TestContext.WriteLine(element);
            Assert.IsTrue(result);
            Assert.IsEmpty(logger.Logs);
        }

        [Test]
        public void ShouldFail_When_ParentFormatIsInvalid()
        {
            // Arrange
            var logger = new MockLogger();
            var varifier = new DefinitionVarifier(logger);

            XElement element = new DefBuilder()
                .WithDef<MockDefinition>("MyMod.Valid")
                .InheritFrom("invalid_parent!") // ❌
                .Build()
                .Root
                .Element(nameof(MockDefinition));

            // Act
            bool result = varifier.VerifyDefinitions(element);

            // Assert
            TestContext.WriteLine(element);
            Assert.IsFalse(result);
            Assert.That(logger.Logs[0].Message, Is.EqualTo(DefinitionVarifier.error_illegalParent).Or.Contains("invalid_parent!"));
        }

        [Test]
        public void ShouldFail_When_ComponentMissingClassOrCompClass()
        {
            // Arrange
            var logger = new MockLogger();
            var varifier = new DefinitionVarifier(logger);

            XElement li = new XElement(XDef.Li,
                new XElement("range", 10) // ❌ 沒有 class 或 compClass
            );
            XElement element = new XElement(nameof(MockDefinition),
                new XElement(XDef.DefName, "MyMod.BadComp"),
                new XElement(XDef.Components, li)
            );

            // Act
            bool result = varifier.VerifyDefinitions(element);

            // Assert
            TestContext.WriteLine(element);
            Assert.IsFalse(result);
            Assert.That(logger.Logs[0].Message, Is.EqualTo(DefinitionVarifier.error_componentMissingClass));
        }

        [Test]
        public void ShouldFail_When_ExtensionMissingClassAttribute()
        {
            // Arrange
            var logger = new MockLogger();
            var varifier = new DefinitionVarifier(logger);

            XElement li = new XElement(XDef.Li,
                new XElement("flag", "true") // ❌ 沒有 class 屬性
            );
            XElement element = new XElement(nameof(MockDefinition),
                new XElement(XDef.DefName, "MyMod.BadExt"),
                new XElement(XDef.Extensions, li)
            );

            // Act
            bool result = varifier.VerifyDefinitions(element);

            // Assert
            TestContext.WriteLine(element);
            Assert.IsFalse(result);
            Assert.That(logger.Logs[0].Message, Is.EqualTo(DefinitionVarifier.error_extensionMissingClass));
        }
    }
}
