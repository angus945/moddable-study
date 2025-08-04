using System.Xml.Linq;
using NUnit.Framework;
using System.Linq;
using AngusChangyiMods.Core.Test;

namespace AngusChangyiMods.Core.DefinitionProcessing.Test
{
    [TestFixture]
    [TestOf(typeof(DefinitionMerger))]
    public class DefinitionMergerTest
    {
        public class MockVarifier : IDefinitionVarifier
        {
            public bool VerifyDefinitions(XElement element)
            {
                return element.Element(Def.DefName) != null;
            }
        }

        [Test]
        public void ShouldMergeDefinitions()
        {
            // Arrange
            var verifier = new MockVarifier();
            var logger = new MockLogger();
            var merger = new DefinitionMerger(verifier, logger);

            var source1 = new DefBuilder()
                .WithDef<MockDefinition>("Merger.TestDefinitionA")
                .AddProperty("stringProp", "Example String AAAAAAA")
                .AddProperty("intProp", "44444")
                .Build();

            var source2 = new DefBuilder()
                .WithDef<MockDefinition>("Merger.TestDefinitionB")
                .AddProperty("stringProp", "Example String")
                .AddProperty("intProp", "42")
                .AddList("listProp", "Item1", "Item2", "Item3")
                .Build();

            var expected = new DefBuilder()
                .WithDef<MockDefinition>("Merger.TestDefinitionA")
                .AddProperty("stringProp", "Example String AAAAAAA")
                .AddProperty("intProp", "44444")
                .WithDef<MockDefinition>("Merger.TestDefinitionB")
                .AddProperty("stringProp", "Example String")
                .AddProperty("intProp", "42")
                .AddList("listProp", "Item1", "Item2", "Item3")
                .Build();

            var mergeTarget = new XDocument(new XElement(Def.Root));

            // Act
            merger.MergeDefinitions(source1, mergeTarget);
            merger.MergeDefinitions(source2, mergeTarget);

            // Assert
            string actual = mergeTarget.ToString();
            string expectedText = expected.ToString();
            TestContext.WriteLine(actual);
            Assert.AreEqual(expectedText, actual);
        }

        [Test]
        public void ShouldOverrideDefinition()
        {
            // Arrange
            var verifier = new MockVarifier();
            var logger = new MockLogger();
            var merger = new DefinitionMerger(verifier, logger);

            var source1 = new DefBuilder()
                .WithDef<MockDefinition>("Merger.OverrideTest")
                .AddProperty("stringProp", "Old")
                .AddProperty("intProp", "123")
                .Build();

            var source2 = new DefBuilder()
                .WithDef<MockDefinition>("Merger.OverrideTest")
                .AddProperty("stringProp", "New")
                .AddProperty("intProp", "999")
                .AddProperty("extra", "value")
                .Build();

            var expected = new DefBuilder()
                .WithDef<MockDefinition>("Merger.OverrideTest")
                .AddProperty("stringProp", "New")
                .AddProperty("intProp", "999")
                .AddProperty("extra", "value")
                .Build();

            var mergeTarget = new XDocument(new XElement(Def.Root));

            // Act
            merger.MergeDefinitions(source1, mergeTarget);
            merger.MergeDefinitions(source2, mergeTarget);

            // Assert
            string actual = mergeTarget.ToString();
            string expectedText = expected.ToString();
            TestContext.WriteLine(actual);
            Assert.AreEqual(expectedText, actual);
            Assert.That(logger.Logs.Any(log => log.Message.Contains("Override Definition")));
        }

        [Test]
        public void ShouldSkipInvalidDefinitions()
        {
            // Arrange
            var verifier = new MockVarifier();
            var logger = new MockLogger();
            var merger = new DefinitionMerger(verifier, logger);

            var source1 = new XDocument(new XElement(Def.Root,
                new XElement("MockDefinition", // ❌ Missing defName
                    new XElement("label", "No defName")
                ),
                new XElement("MockDefinition",
                    new XElement("defName", "Legal.One"),
                    new XElement("stringProp", "A")
                )
            ));

            var source2 = new XDocument(new XElement(Def.Root,
                new XElement("MockDefinition",
                    new XElement("defName", "Legal.Two"),
                    new XElement("stringProp", "B")
                ),
                new XElement("MockDefinition", // ❌ Missing defName
                    new XElement("description", "Invalid again")
                )
            ));

            var expected = new DefBuilder()
                .WithDef<MockDefinition>("Legal.One").AddProperty("stringProp", "A")
                .WithDef<MockDefinition>("Legal.Two").AddProperty("stringProp", "B")
                .Build();

            var mergeTarget = new XDocument(new XElement(Def.Root));

            // Act
            merger.MergeDefinitions(source1, mergeTarget);
            merger.MergeDefinitions(source2, mergeTarget);

            // Assert
            string actual = mergeTarget.ToString();
            string expectedText = expected.ToString();
            TestContext.WriteLine(actual);
            Assert.AreEqual(expectedText, actual);
            Assert.That(logger.Logs.Count(log => log.Message.Contains("Invalid definition")), Is.EqualTo(2));
        }
        
        [Test]
        public void ShouldFail_WhenSourceIsInvalidRoot()
        {
            // Arrange
            var verifier = new MockVarifier();
            var logger = new MockLogger();
            var merger = new DefinitionMerger(verifier, logger);

            var invalidSource = new XDocument(new XElement("NotDefs"));
            var mergeTarget = new XDocument(new XElement(Def.Root));

            // Act
            bool result = merger.MergeDefinitions(invalidSource, mergeTarget);

            // Assert
            TestContext.WriteLine(mergeTarget);
            Assert.IsFalse(result);
            Assert.That(logger.Logs[0].Message, Does.Contain("Invalid source document"));
        }

        [Test]
        public void ShouldFail_WhenMergeTargetIsNull()
        {
            // Arrange
            var verifier = new MockVarifier();
            var logger = new MockLogger();
            var merger = new DefinitionMerger(verifier, logger);

            var source = new XDocument(new XElement(Def.Root));

            // Act
            bool result = merger.MergeDefinitions(source, null);

            // Assert
            Assert.IsFalse(result);
            Assert.That(logger.Logs[0].Message, Does.Contain("Merge target document is null"));
        }

        [Test]
        public void ShouldMergeMultipleTypes()
        {
            // Arrange
            var verifier = new MockVarifier();
            var logger = new MockLogger();
            var merger = new DefinitionMerger(verifier, logger);

            var source = new XDocument(new XElement(Def.Root,
                new XElement("ThingDef", new XElement("defName", "My.Thing")),
                new XElement("PawnKindDef", new XElement("defName", "My.Pawn"))
            ));

            var expected = new XDocument(new XElement(Def.Root,
                new XElement("ThingDef", new XElement("defName", "My.Thing")),
                new XElement("PawnKindDef", new XElement("defName", "My.Pawn"))
            ));

            var mergeTarget = new XDocument(new XElement(Def.Root));

            // Act
            bool result = merger.MergeDefinitions(source, mergeTarget);

            // Assert
            TestContext.WriteLine(mergeTarget);
            Assert.IsTrue(result);
            Assert.AreEqual(expected.ToString(), mergeTarget.ToString());
        }
        
        [Test]
        public void ShouldNotOverrideAcrossTypes()
        {
            // Arrange
            var verifier = new MockVarifier();
            var logger = new MockLogger();
            var merger = new DefinitionMerger(verifier, logger);

            var target = new XDocument(new XElement(Def.Root,
                new XElement("ThingDef", new XElement("defName", "My.SharedName"))
            ));

            var source = new XDocument(new XElement(Def.Root,
                new XElement("PawnKindDef", new XElement("defName", "My.SharedName"))
            ));

            // Act
            bool result = merger.MergeDefinitions(source, target);

            // Assert
            TestContext.WriteLine(target);
            Assert.IsTrue(result);
            Assert.AreEqual(2, target.Root.Elements().Count(), "應該保留兩種不同類型定義");
            Assert.IsEmpty(logger.Logs.Where(l => l.Message.Contains("Override")));
        }
        
        [Test]
        public void ShouldOverrideDuplicateDefsInSource()
        {
            // Arrange
            var verifier = new MockVarifier();
            var logger = new MockLogger();
            var merger = new DefinitionMerger(verifier, logger);

            var source = new DefBuilder()
                .WithDef<MockDefinition>("Dup.Def")
                .AddProperty("stringProp", "First")
                .WithDef<MockDefinition>("Dup.Def")
                .AddProperty("stringProp", "Second")
                .Build();

            var expected = new DefBuilder()
                .WithDef<MockDefinition>("Dup.Def")
                .AddProperty("stringProp", "Second") // 最後一個定義應該被保留
                .Build();

            var target = new XDocument(new XElement(Def.Root));

            // Act
            bool result = merger.MergeDefinitions(source, target);

            // Assert
            TestContext.WriteLine(target);
            Assert.IsTrue(result);
            Assert.AreEqual(1, target.Root.Elements().Count(), "only last definition should remain");
            Assert.AreEqual(expected.ToString(), target.ToString());
            Assert.IsTrue(logger.Logs.Any(log => log.Message.Contains("Override Definition")));
        }

        [Test]
        public void ShouldMergeToNonEmptyTarget()
        {
            // Arrange
            var verifier = new MockVarifier();
            var logger = new MockLogger();
            var merger = new DefinitionMerger(verifier, logger);

            var target = new DefBuilder()
                .WithDef<MockDefinition>("Base.A").AddProperty("val", "1")
                .Build();

            var source = new DefBuilder()
                .WithDef<MockDefinition>("New.B").AddProperty("val", "2")
                .Build();

            var expected = new DefBuilder()
                .WithDef<MockDefinition>("Base.A").AddProperty("val", "1")
                .WithDef<MockDefinition>("New.B").AddProperty("val", "2")
                .Build();

            // Act
            bool result = merger.MergeDefinitions(source, target);

            // Assert
            TestContext.WriteLine(target);
            Assert.IsTrue(result);
            Assert.AreEqual(expected.ToString(), target.ToString());
        }


    }
}
